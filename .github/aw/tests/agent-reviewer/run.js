#!/usr/bin/env node
// Test runner for .github/workflows/agent-reviewer.md.
//
// Usage: node .github/tests/agent-reviewer/run.js [--fresh] [--keep-repo] [--only <scenario-name>]
//   --fresh       Delete and recreate the host repo before running.
//   --keep-repo   Do not delete the host repo when the run finishes
//                 (useful for inspecting a failure).
//   --only <name> Run a single scenario by its `name` (see scenarios.js).
//
// Creates (or reuses) a private GitHub repo, seeds it from FrameStudio.Dev's
// source via --clone-repo, then runs each scenario from scenarios.js through
// `gh aw trial` and checks the resulting safe outputs against expectations.
// Since `gh aw trial` doesn't apply safe outputs to the host repo, label
// changes are synced onto the real issue between turns (see syncSafeOutputs).

const { execFileSync, spawn } = require("node:child_process");
const fs = require("node:fs");
const path = require("node:path");

const scenarios = require("./scenarios");

const SOURCE_REPO = execFileSync("gh", ["repo", "view", "--json", "nameWithOwner", "-q", ".nameWithOwner"], { encoding: "utf8" }).trim();
const SOURCE_WORKFLOW_PATH = path.join(__dirname, "..", "..", "..", "workflows", "agent-reviewer.md");
const TRIAL_WORKFLOW_PATH = path.join(__dirname, "agent-reviewer-trial.generated.md");
const TRIAL_LOCK_PATH = path.join(__dirname, "agent-reviewer-trial.generated.lock.yml");
const TRIAL_TIMEOUT_MINUTES = 10;

const SAFE_OUTPUT_TYPES = new Set([
  "submit_pull_request_review",
  "merge_pull_request",
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

// Without "Allow GitHub Actions to create and approve pull requests" enabled,
// the submit_pull_request_review safe output (APPROVE event) fails with:
//   "GitHub Actions is not permitted to approve pull requests."
// gh aw's ensureTrialRepository normally prompts interactively for this, but
// we create the repo ourselves, so we must set it programmatically.
function enableWorkflowPermissions(slug) {
  console.log(`Enabling workflow permissions on ${slug}...`);
  // Write the JSON body to a temp file because `gh api --input` needs a file
  // path (or stdin pipe) and the boolean field requires proper JSON, not -f strings.
  const tmpFile = path.join(require("os").tmpdir(), `gh-aw-perms-${Date.now()}.json`);
  try {
    fs.writeFileSync(tmpFile, JSON.stringify({
      default_workflow_permissions: "write",
      can_approve_pull_request_reviews: true,
    }));
    sh("gh", [
      "api", "-X", "PUT",
      `repos/${slug}/actions/permissions/workflow`,
      "--input", tmpFile,
    ]);
  } catch (err) {
    console.warn(`Could not set workflow permissions: ${err.message}`);
  } finally {
    try { fs.unlinkSync(tmpFile); } catch { /* ignore */ }
  }
}

function ensureHostRepo(slug, fresh) {
  if (fresh && repoExists(slug)) {
    console.log(`Deleting existing host repo ${slug} (--fresh)...`);
    sh("gh", ["repo", "delete", slug, "--yes"]);
  }
  if (!repoExists(slug)) {
    console.log(`Creating host repo ${slug}...`);
    // No --add-readme: seedHostRepo pushes the source repo content as the
    // first commit on main, so branches start from the correct base.
    sh("gh", ["repo", "create", slug, "--private"]);
  } else {
    console.log(`Reusing existing host repo ${slug}.`);
  }
  // Allow GitHub Actions to create and approve pull requests — required for
  // submit_pull_request_review safe outputs (APPROVE event) to succeed.
  // Without this, the agent's approval is rejected with "GitHub Actions is
  // not permitted to approve pull requests."
  enableWorkflowPermissions(slug);
  seedHostRepo(slug);
}

// gh aw trial --clone-repo pushes FrameStudio.Dev's content to the host repo's
// main branch, but that happens *after* we create branches/PRs. When the host
// repo is fresh (only has the auto-generated README), branches are based on an
// empty main, and the subsequent seed force-push changes the base, which closes
// PRs or makes them conflicting. Pre-seeding avoids this by pushing the source
// repo's content to main *before* we create any branches.
function seedHostRepo(slug) {
  // Check if main already has more than just the initial README commit.
  // If the repo has src/ or package.json, it's already seeded.
  try {
    const tree = sh("gh", ["api", `repos/${slug}/git/trees/main`, "--jq", ".tree[]?.path"]);
    const entries = tree.trim().split("\n").filter(Boolean);
    if (entries.some((e) => e === "src" || e === "package.json")) {
      console.log("Host repo already seeded with source content.");
      return;
    }
  } catch {
    // May fail on empty repos; proceed with seeding.
  }

  console.log(`Seeding host repo ${slug} with content from ${SOURCE_REPO}...`);
  const tmpDir = path.join(require("os").tmpdir(), `gh-aw-seed-${Date.now()}`);
  const token = sh("gh", ["auth", "token"]).trim();
  try {
    // Clone source repo with authenticated URL to avoid Windows Credential
    // Manager prompts.  --filter=blob:none gives a full commit history
    // (required for push to succeed — GitHub rejects shallow pushes) while
    // still being fast because tree/blob data is fetched lazily on checkout.
    const sourceGitUrl = `https://x-access-token:${token}@github.com/${SOURCE_REPO}.git`;
    sh("git", ["clone", "--filter=blob:none", sourceGitUrl, tmpDir]);
    // Add host repo as a remote with the same token
    const hostGitUrl = `https://x-access-token:${token}@github.com/${slug}.git`;
    sh("git", ["-C", tmpDir, "remote", "add", "host", hostGitUrl]);
    // Push main to host (first commit — no force needed since repo is empty)
    sh("git", ["-C", tmpDir, "push", "host", "HEAD:main"]);
    console.log("Host repo seeded successfully.");
  } finally {
    // Clean up temp clone
    try {
      require("fs").rmSync(tmpDir, { recursive: true, force: true });
    } catch {
      // ignore cleanup errors
    }
  }
}

// `gh repo create` only seeds GitHub's default label set; agent-reviewer.md's
// safe-outputs (and every scenario's initialLabels) reference custom labels
// that must exist before `gh issue create`/`gh issue edit --add-label` can use them.
const REQUIRED_LABELS = [
  { name: "ready", color: "1d76db" },
  { name: "in-review", color: "5319e7" },
  { name: "changes-requested", color: "d876e3" },
  { name: "approved", color: "0e8a16" },
  { name: "human-review", color: "fbca04" },
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

// Without --clone-repo, gh aw trial doesn't disable workflows in the host repo
// (Step 2.8 in RunWorkflowTrials only runs when cloneRepoSlug is set). We must
// disable them ourselves to prevent carry-over workflows (e.g. deploy.yml with
// push:main triggers) from firing during trial execution.
//
// IMPORTANT: The trial workflow compiled by `gh aw trial` may have the same
// display name ("Agent Reviewer") as the original workflow shipped in the
// source repo. We must not disable it. Since `gh aw trial` compiles and
// commits the trial workflow AFTER we call disable, we use a different
// strategy: record the IDs of all currently-existing workflows, then
// re-enable the trial one after disabling. But that's fragile. Instead,
// we extract the `name:` from the trial YAML and add it to the keep set.
function disableAllWorkflowsExcept(slug, keepWorkflowNames) {
  console.log(`Disabling workflows in ${slug} (keeping: ${keepWorkflowNames.join(", ")})...`);
  let workflows;
  try {
    workflows = JSON.parse(
      sh("gh", ["workflow", "list", "--repo", slug, "--json", "name,state,id", "--all"])
    );
  } catch {
    console.warn("Could not list workflows — skipping disable step.");
    return;
  }
  const keepSet = new Set(keepWorkflowNames);
  for (const wf of workflows) {
    if (keepSet.has(wf.name)) {
      // Ensure it's enabled (may have been disabled from a prior test run)
      if (wf.state !== "active") {
        try {
          sh("gh", ["workflow", "enable", String(wf.id), "--repo", slug]);
          console.log(`  Re-enabled workflow: ${wf.name} (#${wf.id})`);
        } catch (err) {
          console.warn(`  Could not enable workflow ${wf.name}: ${err.message}`);
        }
      }
      continue;
    }
    if (wf.state === "disabled_manually" || wf.state === "disabled_inactivity") continue;
    try {
      sh("gh", ["workflow", "disable", String(wf.id), "--repo", slug]);
      console.log(`  Disabled workflow: ${wf.name} (#${wf.id})`);
    } catch (err) {
      console.warn(`  Could not disable workflow ${wf.name}: ${err.message}`);
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

function addComment(slug, prNumber, body) {
  console.log(`Posting comment on PR #${prNumber}...`);
  sh("gh", ["pr", "comment", String(prNumber), "--repo", slug, "--body", body]);
}

function createBranch(slug, branchName) {
  console.log(`Creating branch "${branchName}" in ${slug}...`);
  const sha = sh("gh", ["api", `repos/${slug}/git/ref/heads/main`, "--jq", ".object.sha"]).trim();
  sh("gh", [
    "api",
    "-X", "POST",
    `repos/${slug}/git/refs`,
    "-f", `ref=refs/heads/${branchName}`,
    "-f", `sha=${sha}`,
  ]);
  console.log(`Created branch "${branchName}" at ${sha.slice(0, 7)}.`);
}

function createPR(slug, title, body, head, base) {
  console.log(`Creating PR "${title}" (${head} -> ${base}) in ${slug}...`);
  const url = sh("gh", [
    "pr", "create",
    "--repo", slug,
    "--title", title,
    "--body", body,
    "--head", head,
    "--base", base,
  ]).trim();
  const match = url.match(/\/pull\/(\d+)\s*$/);
  if (!match) {
    throw new Error(`Could not parse PR number from: ${url}`);
  }
  const prNumber = Number(match[1]);
  console.log(`Created PR #${prNumber}.`);
  return prNumber;
}

function commitFile(slug, branchName, filePath, content, message) {
  console.log(`Committing ${filePath} to branch "${branchName}" in ${slug}...`);
  const encodedContent = Buffer.from(content).toString("base64");
  const args = [
    "api", "-X", "PUT",
    `repos/${slug}/contents/${filePath}`,
    "-f", `message=${message}`,
    "-f", `content=${encodedContent}`,
    "-f", `branch=${branchName}`,
  ];
  try {
    sh("gh", args);
  } catch (err) {
    // File may already exist — need its SHA to update
    if (!/422/.test(err.message)) throw err;
    const fileSha = sh("gh", [
      "api",
      `repos/${slug}/contents/${filePath}?ref=${branchName}`,
      "--jq", ".sha",
    ]).trim();
    sh("gh", [
      "api", "-X", "PUT",
      `repos/${slug}/contents/${filePath}`,
      "-f", `message=${message}`,
      "-f", `content=${encodedContent}`,
      "-f", `branch=${branchName}`,
      "-f", `sha=${fileSha}`,
    ]);
  }
  console.log(`Committed ${filePath} to branch ${branchName}.`);
}

// `gh aw trial` reads the workflow path directly from local disk and compiles +
// commits ITS OWN lock file into the host repo. If that lock file keeps the real
// triggers (e.g. `pull_request:`), any real event we make for scenario setup can
// also fire a native run there - and toggling the workflow enabled/disabled around
// each dispatch to avoid that proved racy in practice (GitHub's disable API can
// report success several seconds before it actually stops evaluating incoming
// events). Instead, trial always runs against a derived copy with all non-
// `workflow_dispatch` triggers stripped, so there's nothing left for a native
// event to fire. The real agent-reviewer.md (used for production) is untouched.
function buildTrialOnlyWorkflow() {
  const content = fs.readFileSync(SOURCE_WORKFLOW_PATH, "utf8");
  // Only strip non-workflow_dispatch triggers under the `on:` block.
  // The previous regex (^  (?!workflow_dispatch:)\w+:...) was too greedy
  // and also removed `network:`, `tools:`, etc. from the `engine:` section.
  const stripped = content.replace(
    /^on:\r?\n((?:  .+\r?\n)*)/m,
    (match, onBlock) => {
      const trimmed = onBlock.replace(
        /^  (?!workflow_dispatch:)\w+:\r?\n(?:    .+\r?\n)*/gm,
        ""
      );
      return "on:\n" + trimmed;
    }
  );
  if (stripped === content) {
    throw new Error(
      `Expected to strip non-workflow_dispatch triggers from ${SOURCE_WORKFLOW_PATH}, but nothing changed - check its on: block structure.`
    );
  }
  // Rename the workflow so it doesn't clash with the source repo's "Agent Reviewer"
  // workflow (which gets seeded into the host repo). Without this rename, both
  // workflows have the same display name and disableAllWorkflowsExcept can't
  // distinguish them.
  const renamed = stripped.replace(
    /(^|\n)name:\s*['"]?Agent Reviewer['"]?\s*(?:\r?\n)/,
    "$1name: Agent Reviewer Trial\n"
  );
  fs.writeFileSync(TRIAL_WORKFLOW_PATH, renamed === stripped ? stripped : renamed);
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

// When gh aw trial --json produces empty stdout (e.g. because safe output
// handlers like submit_pull_request_review or merge_pull_request failed in
// trial context), we fall back to downloading the safeoutputs.jsonl artifact
// from the GH Actions run. This file contains the raw safe output messages
// produced by the agent, regardless of whether the SSH handler succeeded.
function extractSafeOutputsFromRun(slug, runId) {
  if (!runId) return [];
  try {
    // Find the agent artifact which contains safeoutputs.jsonl
    const artifacts = JSON.parse(
      sh("gh", ["api", `repos/${slug}/actions/runs/${runId}/artifacts`, "--jq", ".artifacts"])
    );
    const agentArtifact = artifacts.find((a) => a.name === "agent");
    if (!agentArtifact) {
      console.warn(`No 'agent' artifact found in run ${runId}`);
      return [];
    }
    // Download the artifact zip to a temp directory
    const tmpDir = path.join(require("os").tmpdir(), `gh-aw-artifact-${Date.now()}`);
    fs.mkdirSync(tmpDir, { recursive: true });
    try {
      sh("gh", ["run", "download", String(runId), "--repo", slug, "--name", "agent", "--dir", tmpDir]);
      const jsonlPath = path.join(tmpDir, "safeoutputs.jsonl");
      if (!fs.existsSync(jsonlPath)) {
        console.warn(`safeoutputs.jsonl not found in agent artifact`);
        return [];
      }
      const lines = fs.readFileSync(jsonlPath, "utf8").trim().split("\n").filter(Boolean);
      const found = [];
      for (const line of lines) {
        try {
          const obj = JSON.parse(line);
          if (typeof obj.type === "string" && SAFE_OUTPUT_TYPES.has(obj.type)) {
            found.push(obj);
          }
        } catch { /* skip malformed lines */ }
      }
      if (found.length > 0) {
        console.log(`Extracted ${found.length} safe output(s) from run artifact.`);
      }
      return found;
    } finally {
      try { require("fs").rmSync(tmpDir, { recursive: true, force: true }); } catch { /* ignore */ }
    }
  } catch (err) {
    console.warn(`Could not extract safe outputs from run artifact: ${err.message}`);
    return [];
  }
}

// Parse the run ID from gh aw trial's stderr output, which includes a line like:
//   i Workflow run started with ID: 30996386750 (https://github.com/...)
function parseRunIdFromStderr(stderr) {
  const match = stderr.match(/workflow run started with id:\s*(\d+)/i);
  return match ? match[1] : null;
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
// evaluated add_labels/remove_labels/replace_label safe outputs to the real
// issue. This keeps the host repo's label state consistent with what production
// execution of the workflow would have applied, which subsequent turns'
// guard clauses (e.g. "only proceed if labeled needs-info") rely on.
function syncSafeOutputs(slug, issueNumber, safeOutputs) {
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

async function runTrial(slug, prNumber, watchForSeedPush) {
  return runTrialCommand(slug, prNumber, watchForSeedPush);
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

function runTrialCommandAsync(slug, prNumber) {
  return new Promise((resolve, reject) => {
    const child = spawn(
      "gh",
      [
        "aw",
        "trial",
        TRIAL_WORKFLOW_PATH,
        "--host-repo",
        slug,
        // No --clone-repo: seedHostRepo() already pushed the source repo
        // content to main before we create branches/PRs.  Having gh aw
        // clone again would be redundant AND triggers Windows Credential
        // Manager prompts (gh aw's internal git clone uses unauthenticated
        // HTTPS URLs).
        // Pass the full PR URL as --trigger-context so that gh-aw populates
        // the pull_request context (not just github.event.inputs.issue_number).
        // Without the full URL, the submit_pull_request_review SSH handler
        // fails with "not running in pull request context".
        "--trigger-context",
        `https://github.com/${slug}/pull/${prNumber}`,
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
      // gh aw trial may exit 1 when safe outputs can't be applied in trial
      // context (e.g. "not running in pull request context"), even though the
      // agent produced correct outputs. Always resolve so we can extract them.
      resolve({ stdout, stderr, exitCode: code });
    });
  });
}

async function runTrialCommand(slug, prNumber, watchForSeedPush) {
  console.log(`Running gh aw trial against PR #${prNumber} (timeout ${TRIAL_TIMEOUT_MINUTES}m)...`);
  // The seed/commit push (and the deploy.yml risk it carries) only happens on the
  // first trial call of the session; later calls find the host repo already up to
  // date and push nothing, so there's nothing for deploy.yml to react to.
  const stopWatching = watchForSeedPush ? watchAndCancelWorkflow(slug, WATCHED_WORKFLOW) : () => {};
  try {
    const result = await runTrialCommandAsync(slug, prNumber);
    if (result.exitCode !== 0) {
      console.warn(`gh aw trial exited with code ${result.exitCode} (safe outputs may not have been applied in trial context)`);
    }
    console.log("Trial finished.");
    // Debug: log what gh aw trial --json produced
    console.log(`[DEBUG] stdout length: ${result.stdout.length}`);
    if (result.stdout.length > 0) {
      console.log(`[DEBUG] stdout first 2000 chars:\n${result.stdout.slice(0, 2000)}`);
    } else {
      console.log("[DEBUG] stdout is empty!");
    }
    console.log(`[DEBUG] stderr length: ${result.stderr.length}`);
    if (result.stderr.length > 0) {
      console.log(`[DEBUG] stderr first 2000 chars:\n${result.stderr.slice(0, 2000)}`);
    }
    // gh aw trial --json may output JSON to stdout OR stderr; try both.
    // If that fails (e.g. because safe output handlers failed in trial context
    // causing --json to produce empty output), fall back to extracting safe
    // outputs from the GH Actions run's agent artifact (safeoutputs.jsonl).
    const rawOutput = result.stdout || result.stderr;
    let safeOutputs = extractSafeOutputs(rawOutput);
    if (safeOutputs.length === 0) {
      console.log("[DEBUG] No safe outputs from --json output, trying run artifact fallback...");
      const runId = parseRunIdFromStderr(result.stderr);
      if (runId) {
        console.log(`[DEBUG] Found run ID ${runId} in stderr, downloading agent artifact...`);
        safeOutputs = extractSafeOutputsFromRun(slug, runId);
      } else {
        console.warn("[DEBUG] Could not find run ID in stderr output.");
      }
    }
    return safeOutputs;
  } finally {
    stopWatching();
  }
}

function evaluate(safeOutputs, expect) {
  const reasons = [];
  const has = (type) => safeOutputs.some((o) => o.type === type);

  if (expect.noop) {
    if (!has("noop")) reasons.push("expected a noop, got none");
    if (has("add_comment")) reasons.push("expected noop but a comment was posted");
    if (has("submit_pull_request_review")) reasons.push("expected noop but a review was submitted");
    return { pass: reasons.length === 0, reasons };
  }

  if (expect.review) {
    if (!has("submit_pull_request_review")) {
      reasons.push(`expected a review (${expect.review}), got none`);
    } else {
      const reviews = safeOutputs.filter((o) => o.type === "submit_pull_request_review");
      const matchingReview = reviews.some((o) => o.event === expect.review);
      if (!matchingReview) {
        const actualEvents = reviews.map((o) => o.event).filter(Boolean).join(", ");
        reasons.push(`expected review event ${expect.review}, got: ${actualEvents || "no event field"}`);
      }
    }
  }

  if (expect.merge && !has("merge_pull_request")) {
    reasons.push("expected PR to be merged, but merge_pull_request not found");
  }

  if (expect.addLabels && expect.addLabels.length > 0) {
    const addOps = safeOutputs.filter((o) => o.type === "add_labels");
    for (const expectedLabel of expect.addLabels) {
      const found = addOps.some((op) =>
        (op.labels || []).map(normalizeLabel).includes(expectedLabel)
      );
      if (!found) {
        reasons.push(`expected add_labels to include "${expectedLabel}", not found`);
      }
    }
  }

  if (expect.removeLabels && expect.removeLabels.length > 0) {
    const removeOps = safeOutputs.filter((o) => o.type === "remove_labels");
    for (const expectedLabel of expect.removeLabels) {
      const found = removeOps.some((op) =>
        (op.labels || []).map(normalizeLabel).includes(expectedLabel)
      );
      if (!found) {
        reasons.push(`expected remove_labels to include "${expectedLabel}", not found`);
      }
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
  const hostRepo = `${login}/gh-aw-reviewer-tests`;

  const activeScenarios = only ? scenarios.filter((s) => s.name === only) : scenarios;
  if (only && activeScenarios.length === 0) {
    throw new Error(`No scenario named "${only}" found in scenarios.js.`);
  }

  ensureHostRepo(hostRepo, fresh);
  ensureLabels(hostRepo);
  // Build the trial-only workflow FIRST so that disableAllWorkflowsExcept can
  // read its `name:` field and avoid disabling it by mistake (GitHub Actions
  // registers workflows by their YAML `name:`, not the filename).
  buildTrialOnlyWorkflow();
  // Without --clone-repo, gh aw trial doesn't disable workflows. We must do it
  // ourselves to prevent carry-over workflows (deploy.yml, etc.) from firing
  // during trial execution. The only workflow that should stay enabled is the
  // one compiled from our trial-only .md.
  disableAllWorkflowsExcept(hostRepo, ["Agent Reviewer Trial"]);

  const results = [];
  let isFirstTrial = true;
  try {
    for (const scenario of activeScenarios) {
      let issueNumber = null;
      let prNumber = null;
      for (const [index, turn] of scenario.turns.entries()) {
        console.log(`\n=== Scenario "${scenario.name}" - turn ${index + 1}/${scenario.turns.length} ===`);
        try {
          if (index === 0) {
            issueNumber = createIssue(hostRepo, turn.title, turn.body, turn.initialLabels || []);
            if (!turn.expect.noop) {
              const branchName = `test/${issueNumber}`;
              createBranch(hostRepo, branchName);
              const files = turn.commitFiles || (turn.commitFile ? [turn.commitFile] : []);
              for (const f of files) {
                commitFile(hostRepo, branchName, f.path, f.content, f.message);
              }
              if (turn.createConflict) {
                const conflictPath = turn.commitFile ? turn.commitFile.path : files[0].path;
                commitFile(hostRepo, "main", conflictPath, "const CONFLICT_VERSION = 'main';\n", "Conflicting change on main");
              }
              prNumber = createPR(hostRepo, turn.title, `Closes #${issueNumber}`, branchName, "main");
            } else {
              prNumber = 0;
            }
          } else if (turn.comment) {
            addComment(hostRepo, prNumber, turn.comment);
          }
          const safeOutputs = await runTrial(hostRepo, prNumber, isFirstTrial);
          isFirstTrial = false;
          const { pass, reasons } = evaluate(safeOutputs, turn.expect);
          console.log(pass ? "Result: PASS" : `Result: FAIL - ${reasons.join("; ")}`);
          results.push({ scenario: scenario.name, turn: index + 1, pass, reasons });
          try {
            syncSafeOutputs(hostRepo, issueNumber, safeOutputs);
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
