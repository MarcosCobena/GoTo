#!/usr/bin/env node
// Test runner for .github/workflows/agent-planner.md.
//
// Usage: node .github/tests/agent-planner/run.js [--fresh] [--keep-repo] [--only <scenario-name>]
//   --fresh       Delete and recreate the host repo before running.
//   --keep-repo   Do not delete the host repo when the run finishes
//                 (useful for inspecting a failure).
//   --only <name> Run a single scenario by its `name` (see scenarios.js).
//
// Creates (or reuses) a private GitHub repo, seeds it from FrameStudio.Dev's
// source via --clone-repo, then runs each scenario from scenarios.js through
// `gh aw trial` and checks the resulting safe outputs against expectations.
// Since `gh aw trial` doesn't apply safe outputs to the host repo, label
// changes are synced onto the real issue between turns (see syncLabels).

const { execFileSync, spawn } = require("node:child_process");
const fs = require("node:fs");
const path = require("node:path");

const scenarios = require("./scenarios");

const SOURCE_REPO = execFileSync("gh", ["repo", "view", "--json", "nameWithOwner", "-q", ".nameWithOwner"], { encoding: "utf8" }).trim();
const SOURCE_WORKFLOW_PATH = path.join(__dirname, "..", "..", "..", "workflows", "agent-planner.md");
const TRIAL_WORKFLOW_PATH = path.join(__dirname, "agent-planner-trial.generated.md");
const TRIAL_LOCK_PATH = path.join(__dirname, "agent-planner-trial.generated.lock.yml");
const TRIAL_TIMEOUT_MINUTES = 10;

const SAFE_OUTPUT_TYPES = new Set([
  "create_issue",
  "link_sub_issue",
  "add_labels",
  "remove_labels",
  "add_comment",
  "noop",
]);

function parseArgs(argv) {
  const onlyIndex = argv.indexOf("--only");
  return {
    fresh: argv.includes("--fresh"),
    keepRepo: argv.includes("--keep-repo"),
    only: onlyIndex === -1 ? null : argv[onlyIndex + 1],
  };
}

function sh(command, args) {
  return execFileSync(command, args, {
    encoding: "utf8",
    stdio: ["ignore", "pipe", "pipe"],
  });
}

function ghAuthenticatedLogin() {
  try {
    return sh("gh", ["api", "user", "--jq", ".login"]).trim();
  } catch (err) {
    throw new Error(
      "gh is not authenticated. Run `gh auth login` first.\n" + err.message
    );
  }
}

function repoExists(slug) {
  try {
    sh("gh", ["repo", "view", slug, "--json", "name"]);
    return true;
  } catch {
    return false;
  }
}

function ensureHostRepo(slug, fresh) {
  if (fresh && repoExists(slug)) {
    console.log(`Deleting existing host repo ${slug} (--fresh)...`);
    sh("gh", ["repo", "delete", slug, "--yes"]);
  }
  if (!repoExists(slug)) {
    console.log(`Creating host repo ${slug}...`);
    sh("gh", ["repo", "create", slug, "--private"]);
  } else {
    console.log(`Reusing existing host repo ${slug}.`);
  }
}

// `gh repo create` only seeds GitHub's default label set; agent-planner.md's
// safe-outputs (and every scenario's initialLabels) reference custom labels
// that must exist before `gh issue create`/`gh issue edit --add-label` can use them.
const REQUIRED_LABELS = [
  { name: "triaged", color: "5319e7" },
  { name: "needs-info", color: "d876e3" },
  { name: "planned", color: "0e8a16" },
  { name: "ready", color: "1d76db" },
];

function ensureLabels(slug) {
  for (const { name, color } of REQUIRED_LABELS) {
    try {
      sh("gh", ["label", "create", name, "--repo", slug, "--color", color]);
    } catch (err) {
      if (!/already exists/i.test(err.message)) throw err;
    }
  }
}

function createIssue(slug, title, body, labels) {
  console.log(`Creating issue "${title}" (labels: ${labels.join(", ") || "none"})...`);
  const args = ["issue", "create", "--repo", slug, "--title", title, "--body", body];
  for (const label of labels) {
    args.push("--label", label);
  }
  const url = sh("gh", args).trim();
  const match = url.match(/\/issues\/(\d+)\s*$/);
  if (!match) {
    throw new Error(`Could not parse issue number from: ${url}`);
  }
  const issueNumber = Number(match[1]);
  console.log(`Created issue #${issueNumber}.`);
  return issueNumber;
}

function addComment(slug, issueNumber, body) {
  console.log(`Posting comment on issue #${issueNumber}...`);
  sh("gh", ["issue", "comment", String(issueNumber), "--repo", slug, "--body", body]);
}

// `gh aw trial` reads the workflow path directly from local disk and compiles +
// commits ITS OWN lock file into the host repo. If that lock file keeps the real
// `issues:`/`issue_comment:` triggers, any real label/comment mutation we make for
// scenario setup can also fire a native run there - and toggling the workflow
// enabled/disabled around each dispatch to avoid that proved racy in practice
// (GitHub's disable API can report success several seconds before it actually
// stops evaluating incoming events). Instead, trial always runs against a derived
// copy with the native triggers stripped, keeping only `workflow_dispatch` - so
// there's nothing left for a native event to fire. The real agent-planner.md
// (used for production) is untouched.
function buildTrialOnlyWorkflow() {
  const content = fs.readFileSync(SOURCE_WORKFLOW_PATH, "utf8");
  const stripped = content.replace(/^  issues:\r?\n(?:    .+\r?\n)+/m, "").replace(/^  issue_comment:\r?\n(?:    .+\r?\n)+/m, "");
  if (stripped === content) {
    throw new Error(
      `Expected to strip issues:/issue_comment: triggers from ${SOURCE_WORKFLOW_PATH}, but nothing changed - check its on: block structure.`
    );
  }
  fs.writeFileSync(TRIAL_WORKFLOW_PATH, stripped);
}

function cleanupTrialOnlyWorkflow() {
  for (const file of [TRIAL_WORKFLOW_PATH, TRIAL_LOCK_PATH]) {
    try {
      fs.unlinkSync(file);
    } catch {
      // already absent, nothing to clean up
    }
  }
}

function collectSafeOutputs(node, found) {
  if (Array.isArray(node)) {
    for (const item of node) collectSafeOutputs(item, found);
    return;
  }
  if (node && typeof node === "object") {
    if (typeof node.type === "string" && SAFE_OUTPUT_TYPES.has(node.type)) {
      found.push(node);
    }
    for (const value of Object.values(node)) collectSafeOutputs(value, found);
  }
}

function latestTrialArtifact() {
  const dir = path.join(process.cwd(), "trials");
  if (!fs.existsSync(dir)) return null;
  const files = fs
    .readdirSync(dir)
    .filter((f) => f.endsWith(".json"))
    .map((f) => ({ f, mtime: fs.statSync(path.join(dir, f)).mtimeMs }))
    .sort((a, b) => b.mtime - a.mtime);
  return files.length ? path.join(dir, files[0].f) : null;
}

// NOTE: the exact JSON shape of `gh aw trial --json` output is unconfirmed
// (see spec's "Known open questions"). This function searches the whole
// parsed tree for objects with a recognized safe-output `type`, and falls
// back to the newest file under trials/ if stdout wasn't parseable JSON or
// contained nothing recognizable. Adjust after the calibration run in Task 4
// if real output doesn't match.
function extractSafeOutputs(rawStdout) {
  const found = [];
  let parsed = null;
  try {
    parsed = JSON.parse(rawStdout);
  } catch {
    // stdout wasn't pure JSON; fall through to the trials/ artifact below.
  }
  if (parsed) collectSafeOutputs(parsed, found);
  if (found.length === 0) {
    const artifact = latestTrialArtifact();
    if (artifact) {
      try {
        collectSafeOutputs(JSON.parse(fs.readFileSync(artifact, "utf8")), found);
      } catch (err) {
        console.warn(`Could not parse trial artifact ${artifact}: ${err.message}`);
      }
    }
  }
  return found;
}

// gh-aw's add_labels/remove_labels tool schema encourages (and sometimes requires)
// structured label entries ({name, rationale, confidence}) instead of plain strings;
// normalize either shape down to just the label name.
function normalizeLabel(entry) {
  return typeof entry === "string" ? entry : entry && entry.name;
}

// `gh aw trial` never mutates the host repo, so after each turn we apply the
// evaluated add_labels/remove_labels safe outputs to the real issue. This
// keeps the host repo's label state consistent with what production
// execution of the workflow would have applied, which subsequent turns'
// guard clauses (e.g. "only proceed if labeled needs-info") rely on.
function syncLabels(slug, issueNumber, safeOutputs) {
  const labelsFor = (type) =>
    safeOutputs
      .filter((o) => o.type === type)
      .flatMap((o) => o.labels || (o.label ? [o.label] : []))
      .map(normalizeLabel)
      .filter(Boolean);
  const addLabels = labelsFor("add_labels");
  const removeLabels = labelsFor("remove_labels");
  if (addLabels.length === 0 && removeLabels.length === 0) return;

  console.log(`Syncing labels on issue #${issueNumber}: +[${addLabels.join(", ")}] -[${removeLabels.join(", ")}]`);
  const args = ["issue", "edit", String(issueNumber), "--repo", slug];
  for (const label of addLabels) args.push("--add-label", label);
  for (const label of removeLabels) args.push("--remove-label", label);
  sh("gh", args);
}

async function runTrial(slug, issueNumber, watchForSeedPush) {
  return runTrialCommand(slug, issueNumber, watchForSeedPush);
}

// `gh aw trial`'s clone-repo seed push carries over deploy.yml (push:main trigger),
// which fires a real run before it can be disabled (it isn't a recognized workflow
// in the host repo until after that same push). Run the trial in the background and
// poll for a deploy.yml run to appear, cancelling it the instant it does. The seed/
// commit push only happens early in `gh aw trial`'s own execution, so watching is
// capped to a short window instead of the whole (up to 10-minute) trial duration —
// polling for the full duration burns through the GitHub API rate limit fast.
const WATCHED_WORKFLOW = "deploy.yml";
const CANCELLABLE_STATUSES = new Set(["queued", "in_progress", "requested", "waiting"]);

function watchAndCancelWorkflow(slug, workflowFile, intervalMs = 4000, maxDurationMs = 90000) {
  const seen = new Set();
  const timer = setInterval(() => {
    let runs;
    try {
      runs = JSON.parse(
        sh("gh", ["run", "list", "--repo", slug, "--workflow", workflowFile, "--json", "databaseId,status", "--limit", "5"])
      );
    } catch {
      return; // workflow not registered yet, or a transient API error; retry next tick
    }
    for (const run of runs) {
      if (seen.has(run.databaseId) || !CANCELLABLE_STATUSES.has(run.status)) continue;
      seen.add(run.databaseId);
      try {
        sh("gh", ["run", "cancel", String(run.databaseId), "--repo", slug]);
        console.log(`Cancelled unwanted ${workflowFile} run #${run.databaseId}`);
      } catch (err) {
        console.warn(`Could not cancel ${workflowFile} run #${run.databaseId}: ${err.message}`);
      }
    }
  }, intervalMs);
  const stopTimeout = setTimeout(() => clearInterval(timer), maxDurationMs);
  return () => {
    clearInterval(timer);
    clearTimeout(stopTimeout);
  };
}

function runTrialCommandAsync(slug, issueNumber) {
  return new Promise((resolve, reject) => {
    const child = spawn(
      "gh",
      [
        "aw",
        "trial",
        TRIAL_WORKFLOW_PATH,
        "--host-repo",
        slug,
        "--clone-repo",
        SOURCE_REPO,
        "--trigger-context",
        `https://github.com/${slug}/issues/${issueNumber}`,
        "-y",
        "--json",
        "--timeout",
        String(TRIAL_TIMEOUT_MINUTES),
      ],
      { stdio: ["ignore", "pipe", "pipe"] }
    );
    let stdout = "";
    let stderr = "";
    child.stdout.on("data", (chunk) => (stdout += chunk));
    child.stderr.on("data", (chunk) => (stderr += chunk));
    child.on("error", reject);
    child.on("close", (code) => {
      if (code !== 0) {
        reject(new Error(`Command failed: gh aw trial ... (exit ${code})\n${stderr}`));
        return;
      }
      resolve(stdout);
    });
  });
}

async function runTrialCommand(slug, issueNumber, watchForSeedPush) {
  console.log(`Running gh aw trial against issue #${issueNumber} (timeout ${TRIAL_TIMEOUT_MINUTES}m)...`);
  // The seed/commit push (and the deploy.yml risk it carries) only happens on the
  // first trial call of the session; later calls find the host repo already up to
  // date and push nothing, so there's nothing for deploy.yml to react to.
  const stopWatching = watchForSeedPush ? watchAndCancelWorkflow(slug, WATCHED_WORKFLOW) : () => {};
  try {
    const stdout = await runTrialCommandAsync(slug, issueNumber);
    console.log("Trial finished.");
    return extractSafeOutputs(stdout);
  } finally {
    stopWatching();
  }
}

function evaluate(safeOutputs, expect) {
  const reasons = [];
  const has = (type) => safeOutputs.some((o) => o.type === type);
  const labelsFor = (type) =>
    safeOutputs
      .filter((o) => o.type === type)
      .flatMap((o) => o.labels || (o.label ? [o.label] : []))
      .map(normalizeLabel)
      .filter(Boolean);

  if (expect.noop) {
    if (!has("noop")) reasons.push("expected a noop, got none");
    if (has("add_comment")) reasons.push("expected noop but a comment was posted");
    return { pass: reasons.length === 0, reasons };
  }

  for (const label of expect.addLabels || []) {
    if (!labelsFor("add_labels").includes(label)) {
      reasons.push(`expected add_labels to include "${label}"`);
    }
  }
  for (const label of expect.removeLabels || []) {
    if (!labelsFor("remove_labels").includes(label)) {
      reasons.push(`expected remove_labels to include "${label}"`);
    }
  }
  if (expect.minIssuesCreated) {
    const created = safeOutputs.filter((o) => o.type === "create_issue").length;
    if (created < expect.minIssuesCreated) {
      reasons.push(
        `expected at least ${expect.minIssuesCreated} create_issue call(s), got ${created}`
      );
    }
  }
  if (expect.comment && !has("add_comment")) {
    reasons.push("expected a comment to be posted");
  }
  return { pass: reasons.length === 0, reasons };
}

async function main() {
  const { fresh, keepRepo, only } = parseArgs(process.argv.slice(2));
  const login = ghAuthenticatedLogin();
  const hostRepo = `${login}/gh-aw-planner-tests`;

  const activeScenarios = only ? scenarios.filter((s) => s.name === only) : scenarios;
  if (only && activeScenarios.length === 0) {
    throw new Error(`No scenario named "${only}" found in scenarios.js.`);
  }

  ensureHostRepo(hostRepo, fresh);
  ensureLabels(hostRepo);
  buildTrialOnlyWorkflow();

  const results = [];
  let isFirstTrial = true;
  try {
    for (const scenario of activeScenarios) {
      let issueNumber = null;
      for (const [index, turn] of scenario.turns.entries()) {
        console.log(`\n=== Scenario "${scenario.name}" - turn ${index + 1}/${scenario.turns.length} ===`);
        try {
          if (index === 0) {
            issueNumber = createIssue(hostRepo, turn.title, turn.body, turn.initialLabels || []);
          } else if (turn.comment) {
            addComment(hostRepo, issueNumber, turn.comment);
          }
          const safeOutputs = await runTrial(hostRepo, issueNumber, isFirstTrial);
          isFirstTrial = false;
          const { pass, reasons } = evaluate(safeOutputs, turn.expect);
          console.log(pass ? "Result: PASS" : `Result: FAIL - ${reasons.join("; ")}`);
          results.push({ scenario: scenario.name, turn: index + 1, pass, reasons });
          try {
            syncLabels(hostRepo, issueNumber, safeOutputs);
          } catch (err) {
            console.warn(`Could not sync labels on issue #${issueNumber}: ${err.message}`);
          }
        } catch (err) {
          console.log(`Result: FAIL - ${err.message}`);
          results.push({
            scenario: scenario.name,
            turn: index + 1,
            pass: false,
            reasons: [err.message],
          });
        }
      }
    }
  } finally {
    cleanupTrialOnlyWorkflow();
    if (!keepRepo) {
      console.log(`Deleting host repo ${hostRepo}...`);
      try {
        sh("gh", ["repo", "delete", hostRepo, "--yes"]);
      } catch (err) {
        console.warn(`Could not delete ${hostRepo}: ${err.message}`);
      }
    } else {
      console.log(`Keeping host repo ${hostRepo} (--keep-repo).`);
    }
  }

  console.log("\nResults:");
  let failed = 0;
  for (const r of results) {
    const status = r.pass ? "PASS" : "FAIL";
    if (!r.pass) failed += 1;
    console.log(`  [${status}] ${r.scenario} (turn ${r.turn})`);
    for (const reason of r.reasons) console.log(`         - ${reason}`);
  }
  console.log(`\n${results.length - failed}/${results.length} passed.`);
  process.exitCode = failed > 0 ? 1 : 0;
}

main().catch((err) => {
  console.error(err);
  process.exitCode = 1;
});
