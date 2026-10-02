#!/usr/bin/env node
// Test runner for .github/workflows/agent-developer.md.
//
// Usage: node .github/tests/agent-developer/run.js [--fresh] [--keep-repo] [--only <scenario-name>]
//   --fresh       Delete and recreate the host repo before running.
//   --keep-repo   Do not delete the host repo when the run finishes.
//   --only <name> Run a single scenario by its `name` (see scenarios.js).

const { execFileSync, spawn } = require("node:child_process");
const fs = require("node:fs");
const os = require("node:os");
const path = require("node:path");

const scenarios = require("./scenarios");

const SOURCE_REPO = execFileSync("gh", ["repo", "view", "--json", "nameWithOwner", "-q", ".nameWithOwner"], { encoding: "utf8" }).trim();
const SOURCE_WORKFLOW_PATH = path.join(__dirname, "..", "..", "..", "workflows", "agent-developer.md");
const TRIAL_WORKFLOW_PATH = path.join(__dirname, "agent-developer-trial.generated.md");
const TRIAL_LOCK_PATH = path.join(__dirname, "agent-developer-trial.generated.lock.yml");
const TRIAL_TIMEOUT_MINUTES = 10;

const SAFE_OUTPUT_TYPES = new Set([
  "create_pull_request",
  "push_to_pull_request_branch",
  "add_labels",
  "remove_labels",
  "add_comment",
  "noop",
]);

const REQUIRED_LABELS = [
  { name: "ready", color: "1d76db" },
  { name: "changes-requested", color: "d93f0b" },
  { name: "in-review", color: "0e8a16" },
  { name: "triaged", color: "5319e7" },
];

function parseArgs(argv) {
  const onlyIndex = argv.indexOf("--only");
  return {
    fresh: argv.includes("--fresh"),
    keepRepo: argv.includes("--keep-repo"),
    only: onlyIndex === -1 ? null : argv[onlyIndex + 1],
  };
}

function makeHostRepoSlug(login, fresh) {
  if (!fresh) {
    return `${login}/gh-aw-developer-tests`;
  }

  const stamp = new Date().toISOString().replace(/[-:TZ.]/g, "").slice(0, 14);
  return `${login}/gh-aw-developer-tests-${stamp}`;
}

function sh(command, args, options = {}) {
  return execFileSync(command, args, {
    encoding: "utf8",
    stdio: ["ignore", "pipe", "pipe"],
    ...options,
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

function ensureWorkflowPermissions(slug) {
  let current = null;
  try {
    current = JSON.parse(
      sh("gh", ["api", `repos/${slug}/actions/permissions/workflow`])
    );
  } catch (err) {
    throw new Error(
      `Could not read Actions workflow permissions for ${slug}: ${err.message}`
    );
  }

  const needsWrite = current.default_workflow_permissions !== "write";
  const needsPrApproval = current.can_approve_pull_request_reviews !== true;
  if (!needsWrite && !needsPrApproval) {
    return;
  }

  console.log(
    `Updating Actions workflow permissions on ${slug} to allow PR creation...`
  );
  sh("gh", [
    "api",
    "--method",
    "PUT",
    `repos/${slug}/actions/permissions/workflow`,
    "-f",
    "default_workflow_permissions=write",
    "-F",
    "can_approve_pull_request_reviews=true",
  ]);
}

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

function buildTrialOnlyWorkflow() {
  const content = fs.readFileSync(SOURCE_WORKFLOW_PATH, "utf8");
  const stripped = content.replace(/^  issues:\r?\n(?:    .+\r?\n)+/m, "");
  if (stripped === content) {
    throw new Error(
      `Expected to strip issues: trigger from ${SOURCE_WORKFLOW_PATH}, but nothing changed.`
    );
  }
  fs.writeFileSync(TRIAL_WORKFLOW_PATH, stripped);
}

function cleanupTrialOnlyWorkflow() {
  for (const file of [TRIAL_WORKFLOW_PATH, TRIAL_LOCK_PATH]) {
    try {
      fs.unlinkSync(file);
    } catch {
      // already absent
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

function extractSafeOutputs(rawStdout) {
  const found = [];
  let parsed = null;
  try {
    parsed = JSON.parse(rawStdout);
  } catch {
    // fallback to artifact
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

function normalizeLabel(entry) {
  return typeof entry === "string" ? entry : entry && entry.name;
}

function labelsForType(safeOutputs, type) {
  return safeOutputs
    .filter((o) => o.type === type)
    .flatMap((o) => o.labels || (o.label ? [o.label] : []))
    .map(normalizeLabel)
    .filter(Boolean);
}

function syncLabels(slug, issueNumber, safeOutputs) {
  const addLabels = labelsForType(safeOutputs, "add_labels");
  const removeLabels = labelsForType(safeOutputs, "remove_labels");
  if (addLabels.length === 0 && removeLabels.length === 0) return;

  console.log(
    `Syncing labels on issue #${issueNumber}: +[${addLabels.join(", ")}] -[${removeLabels.join(", ")}]`
  );
  const args = ["issue", "edit", String(issueNumber), "--repo", slug];
  for (const label of addLabels) args.push("--add-label", label);
  for (const label of removeLabels) args.push("--remove-label", label);
  sh("gh", args);
}

const WATCHED_WORKFLOW = "deploy.yml";
const CANCELLABLE_STATUSES = new Set(["queued", "in_progress", "requested", "waiting"]);

function watchAndCancelWorkflow(slug, workflowFile, intervalMs = 4000, maxDurationMs = 90000) {
  const seen = new Set();
  const timer = setInterval(() => {
    let runs;
    try {
      runs = JSON.parse(
        sh("gh", [
          "run",
          "list",
          "--repo",
          slug,
          "--workflow",
          workflowFile,
          "--json",
          "databaseId,status",
          "--limit",
          "5",
        ])
      );
    } catch {
      return;
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

function runTrialCommandAsync(slug, issueNumber, dispatchWithoutContext) {
  return new Promise((resolve, reject) => {
    const args = [
      "aw",
      "trial",
      TRIAL_WORKFLOW_PATH,
      "--host-repo",
      slug,
      "--clone-repo",
      SOURCE_REPO,
      "-y",
      "--json",
      "--timeout",
      String(TRIAL_TIMEOUT_MINUTES),
    ];
    if (!dispatchWithoutContext) {
      args.push("--trigger-context", `https://github.com/${slug}/issues/${issueNumber}`);
    }

    const child = spawn("gh", args, { stdio: ["ignore", "pipe", "pipe"] });
    let stdout = "";
    let stderr = "";
    child.stdout.on("data", (chunk) => (stdout += chunk));
    child.stderr.on("data", (chunk) => (stderr += chunk));
    child.on("error", reject);
    child.on("close", (code) => {
      if (code !== 0) {
        reject(new Error(`Command failed: gh ${args.join(" ")} (exit ${code})\n${stderr}`));
        return;
      }
      resolve(stdout);
    });
  });
}

async function runTrial(slug, issueNumber, options) {
  const { watchForSeedPush, dispatchWithoutContext } = options;
  console.log(
    dispatchWithoutContext
      ? `Running gh aw trial without trigger context (timeout ${TRIAL_TIMEOUT_MINUTES}m)...`
      : `Running gh aw trial against issue #${issueNumber} (timeout ${TRIAL_TIMEOUT_MINUTES}m)...`
  );
  const stopWatching = watchForSeedPush ? watchAndCancelWorkflow(slug, WATCHED_WORKFLOW) : () => {};
  try {
    let stdout = null;
    const maxAttempts = 3;
    for (let attempt = 1; attempt <= maxAttempts; attempt += 1) {
      try {
        stdout = await runTrialCommandAsync(slug, issueNumber, dispatchWithoutContext);
        break;
      } catch (err) {
        const message = String(err && err.message ? err.message : err);
        const isWorkflow404 = /workflow .*\.lock\.yml not found on the default branch/i.test(message);
        if (!isWorkflow404 || attempt === maxAttempts) {
          throw err;
        }
        console.warn(
          `Trial trigger returned transient workflow 404 (attempt ${attempt}/${maxAttempts}); retrying...`
        );
      }
    }
    console.log("Trial finished.");
    return extractSafeOutputs(stdout || "");
  } finally {
    stopWatching();
  }
}

function defaultBranch(slug) {
  return sh("gh", ["repo", "view", slug, "--json", "defaultBranchRef", "--jq", ".defaultBranchRef.name"]).trim();
}

function ensureReentryState(slug, issueNumber, reviewComment) {
  const baseBranch = defaultBranch(slug);
  const branch = `agents/issue-${issueNumber}`;
  const tempRoot = fs.mkdtempSync(path.join(os.tmpdir(), "gh-aw-dev-reentry-"));
  const repoDir = path.join(tempRoot, "repo");

  try {
    sh("gh", ["repo", "clone", slug, repoDir, "--", "--depth", "1"]);

    sh("git", ["-C", repoDir, "config", "user.email", "gh-aw-trial-bot@example.com"]);
    sh("git", ["-C", repoDir, "config", "user.name", "gh-aw-trial-bot"]);

    sh("git", ["-C", repoDir, "checkout", "-B", branch, `origin/${baseBranch}`]);

    const markerDir = path.join(repoDir, ".agents", "memory");
    fs.mkdirSync(markerDir, { recursive: true });
    const markerPath = path.join(markerDir, `reentry-issue-${issueNumber}.md`);
    fs.writeFileSync(
      markerPath,
      `Temporary marker to simulate an existing PR branch for issue #${issueNumber}.\n`
    );

    sh("git", ["-C", repoDir, "add", path.relative(repoDir, markerPath)]);
    sh("git", ["-C", repoDir, "commit", "-m", `test: simulate reentry for issue ${issueNumber}`]);
    sh("git", ["-C", repoDir, "push", "-u", "origin", branch, "--force"]);

    try {
      sh("gh", [
        "pr",
        "create",
        "--repo",
        slug,
        "--base",
        baseBranch,
        "--head",
        branch,
        "--title",
        `Test re-entry PR for issue #${issueNumber}`,
        "--body",
        `Synthetic PR used by agent-developer trial tests for issue #${issueNumber}.`,
      ]);
    } catch (err) {
      if (!/already exists/i.test(err.message)) throw err;
    }
  } finally {
    try {
      fs.rmSync(tempRoot, { recursive: true, force: true });
    } catch {
      // best effort cleanup
    }
  }

  const issueArgs = ["issue", "edit", String(issueNumber), "--repo", slug, "--add-label", "changes-requested"];
  try {
    issueArgs.push("--remove-label", "in-review");
    sh("gh", issueArgs);
  } catch {
    // ignore if in-review was missing
    sh("gh", ["issue", "edit", String(issueNumber), "--repo", slug, "--add-label", "changes-requested"]);
  }

  if (reviewComment) {
    addComment(slug, issueNumber, reviewComment);
  }
}

function evaluate(safeOutputs, expect) {
  const reasons = [];
  const count = (type) => safeOutputs.filter((o) => o.type === type).length;
  const has = (type) => count(type) > 0;

  const createdPrCount = count("create_pull_request");
  const pushedPrCount = count("push_to_pull_request_branch");
  const addLabels = labelsForType(safeOutputs, "add_labels");
  const removeLabels = labelsForType(safeOutputs, "remove_labels");
  const hasComment = has("add_comment");
  const hasNoop = has("noop");

  if (expect.createPr && createdPrCount < 1) {
    reasons.push("expected create_pull_request but none was produced");
  }
  if (!expect.createPr && createdPrCount > 0) {
    reasons.push(`expected no create_pull_request, got ${createdPrCount}`);
  }
  if (typeof expect.maxPrCreated === "number" && createdPrCount > expect.maxPrCreated) {
    reasons.push(
      `expected at most ${expect.maxPrCreated} create_pull_request call(s), got ${createdPrCount}`
    );
  }

  if (expect.pushToExistingPr && pushedPrCount < 1) {
    reasons.push("expected push_to_pull_request_branch but none was produced");
  }
  if (!expect.pushToExistingPr && pushedPrCount > 0) {
    reasons.push(`expected no push_to_pull_request_branch, got ${pushedPrCount}`);
  }

  for (const label of expect.addLabels || []) {
    if (!addLabels.includes(label)) {
      reasons.push(`expected add_labels to include \"${label}\"`);
    }
  }
  for (const label of expect.removeLabels || []) {
    if (!removeLabels.includes(label)) {
      reasons.push(`expected remove_labels to include \"${label}\"`);
    }
  }

  if (expect.comment && !hasComment) reasons.push("expected a comment to be posted");
  if (!expect.comment && hasComment) reasons.push("expected no comment but add_comment was produced");

  if (expect.noop && !hasNoop) reasons.push("expected noop but none was produced");
  if (!expect.noop && hasNoop) reasons.push("expected no noop but noop was produced");

  if (expect.noop) {
    const writes = createdPrCount + pushedPrCount + addLabels.length + removeLabels.length;
    if (writes > 0) {
      reasons.push("expected noop path without mutating actions");
    }
  }

  return { pass: reasons.length === 0, reasons };
}

async function main() {
  const { fresh, keepRepo, only } = parseArgs(process.argv.slice(2));
  const login = ghAuthenticatedLogin();
  const hostRepo = makeHostRepoSlug(login, fresh);

  const activeScenarios = only ? scenarios.filter((s) => s.name === only) : scenarios;
  if (only && activeScenarios.length === 0) {
    throw new Error(`No scenario named \"${only}\" found in scenarios.js.`);
  }

  ensureHostRepo(hostRepo, fresh);
  ensureWorkflowPermissions(hostRepo);
  ensureLabels(hostRepo);
  buildTrialOnlyWorkflow();

  const results = [];
  let isFirstTrial = true;
  try {
    for (const scenario of activeScenarios) {
      let issueNumber = null;
      for (const [index, turn] of scenario.turns.entries()) {
        console.log(`\n=== Scenario \"${scenario.name}\" - turn ${index + 1}/${scenario.turns.length} ===`);
        try {
          const dispatchWithoutContext = Boolean(turn.dispatchWithoutContext);

          if (index === 0 && !dispatchWithoutContext) {
            issueNumber = createIssue(hostRepo, turn.title, turn.body, turn.initialLabels || []);
          } else if (index > 0 && turn.comment && issueNumber) {
            addComment(hostRepo, issueNumber, turn.comment);
          }

          if (turn.simulateChangesRequested) {
            if (!issueNumber) {
              throw new Error("simulateChangesRequested requires an existing issue number");
            }
            ensureReentryState(hostRepo, issueNumber, turn.reviewComment);
          }

          const safeOutputs = await runTrial(hostRepo, issueNumber, {
            watchForSeedPush: isFirstTrial,
            dispatchWithoutContext,
          });
          isFirstTrial = false;

          const { pass, reasons } = evaluate(safeOutputs, turn.expect);
          console.log(pass ? "Result: PASS" : `Result: FAIL - ${reasons.join("; ")}`);
          results.push({ scenario: scenario.name, turn: index + 1, pass, reasons });

          if (!dispatchWithoutContext && issueNumber) {
            try {
              syncLabels(hostRepo, issueNumber, safeOutputs);
            } catch (err) {
              console.warn(`Could not sync labels on issue #${issueNumber}: ${err.message}`);
            }
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
