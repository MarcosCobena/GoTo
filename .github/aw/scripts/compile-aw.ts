/**
 * Compiles all gh-aw workflows and then patches the agent-reviewer.lock.yml
 * to work around the `target_branch_default` gate in merge_pull_request.cjs
 * (gh-aw v0.83.4 / v0.85.4) which refuses to merge to the repository default
 * branch (e.g. `main`). This makes the `merge-pull-request` safe-output
 * unusable for a reviewer that needs to merge feature branches into `main`.
 *
 * The patch removes the `if (branchPolicy.isDefault) { ... }` block from
 * merge_pull_request.cjs at runtime, so merges to the default branch are
 * allowed. The block to remove is:
 *
 *   if (branchPolicy.isDefault) {
 *     failureReasons.push({
 *       code: "target_branch_default",
 *       message: `Target branch "${baseBranch}" is the repository default branch`,
 *       details: { default_branch: branchPolicy.defaultBranch },
 *     });
 *   }
 *
 * Usage:
 *   pnpm compile-aw
 *
 * The script:
 *   1. Runs `gh aw compile --approve` to compile all workflows.
 *   2. Reads `.github/workflows/agent-reviewer.lock.yml`.
 *   3. Inserts a "Patch merge_pull_request handler" step into the safe_outputs
 *      job, right after the "Setup Scripts" step, if it's not already present.
 *
 * Idempotent: can be run multiple times; the patch step is only inserted once.
 */

import { readFileSync, writeFileSync, existsSync } from "node:fs";
import { execSync } from "node:child_process";
import { resolve } from "node:path";

const REPO_ROOT = resolve(import.meta.dirname, "..", "..", "..");
const LOCK_FILE = resolve(REPO_ROOT, ".github/workflows/agent-reviewer.lock.yml");
const PATCH_STEP_NAME = "Patch merge_pull_request handler (remove target_branch_default gate)";
const PATCH_STEP_NAME_2 = "Patch merge_pull_request handler (head filter bug)";

function log(message: string): void {
  console.log(`[compile-aw] ${message}`);
}

function logWarning(message: string): void {
  console.warn(`[compile-aw] ⚠ ${message}`);
}

function logSuccess(message: string): void {
  console.log(`[compile-aw] ✓ ${message}`);
}

/**
 * Runs `gh aw compile --approve` to compile all workflows.
 * Throws if the compilation fails.
 */
function compileWorkflows(): void {
  log("Compiling all gh-aw workflows (gh aw compile --approve)...");
  try {
    execSync("gh aw compile --approve", {
      cwd: REPO_ROOT,
      stdio: "inherit",
    });
  } catch (error) {
    throw new Error(`gh aw compile failed: ${error}`);
  }
  logSuccess("Workflows compiled.");
}

// The shell script that patches merge_pull_request.cjs at runtime.
//
// It removes the `if (branchPolicy.isDefault) { ... }` block that refuses
// merges to the repository default branch (e.g. main). This makes the
// `merge-pull-request` safe-output usable for a reviewer that merges
// approved PRs into main.
//
// We use perl -0 (slurp mode) with a multiline regex to match the entire
// 7-line block and delete it. If the pattern isn't found (e.g. due to
// formatting changes in a future gh-aw version), a warning is emitted
// and the workflow continues — the merge will simply fail with
// target_branch_default as before, which is non-fatal.
const PATCH_STEP_COMMENTS: string[] = [
  "Workaround: gh-aw refuses to merge to the default branch (main),",
  "making merge-pull-request unusable for a reviewer.",
  "Remove the `if (branchPolicy.isDefault)` gate from merge_pull_request.cjs.",
  "Auto-injected by .github/aw/scripts/compile-aw.ts - do not edit manually.",
];

const PATCH_RUN_LINES: string[] = [
  "set -euo pipefail",
  'handler="${RUNNER_TEMP}/gh-aw/actions/merge_pull_request.cjs"',
  'if [ ! -f "$handler" ]; then',
  '  echo "::warning::merge_pull_request.cjs not found at $handler; skipping patch"',
  "  exit 0",
  "fi",
  'cp "$handler" "$handler.bak"',
  "# Remove the target_branch_default gate that blocks merges to the default branch.",
  "# Uses perl -0 (slurp) with a multiline regex to match the if-block and delete it.",
  // Single-quoted perl -e argument: bash performs no expansion/escaping inside
  // single quotes, so the embedded literal " and ` characters are safe as-is.
  'perl -0pi -e \'s/\\s*if\\s*\\(branchPolicy\\.isDefault\\)\\s*\\{\\s*\\n\\s*failureReasons\\.push\\(\\{\\s*\\n\\s*code:\\s*"target_branch_default",\\s*\\n\\s*message:\\s*`Target\\s+branch\\s+"\\$\\{baseBranch\\}"\\s+is\\s+the\\s+repository\\s+default\\s+branch`,\\s*\\n\\s*details:\\s*\\{\\s*default_branch:\\s*branchPolicy\\.defaultBranch\\s*\\},\\s*\\n\\s*\\}\\);\\s*\\n\\s*\\}//s\' "$handler"',
  'if cmp -s "$handler" "$handler.bak"; then',
  '  echo "::warning::Patch did not change merge_pull_request.cjs (pattern not found or already patched)"',
  "else",
  '  echo "Patched merge_pull_request.cjs: removed target_branch_default gate"',
  "fi",
  'rm -f "$handler.bak"',
];

// Second shell script: works around gh-aw's `head` filter bug in
// merge_pull_request.cjs (mfrancza/agentic-development-workflow#21). gh-aw
// calls pulls.list with `head: \`${owner}:${branch}\`` which GitHub's API
// returns [] for when the PR's head repo differs in casing/format from the
// request, so the handler never finds the PR to merge for same-repo PRs.
// This patches the handler to filter by `head: branch` only, then applies a
// client-side filter on `head.repo.owner.login` to preserve fork-safety.
const PATCH_STEP_2_COMMENTS: string[] = [
  "Workaround for https://github.com/mfrancza/agentic-development-workflow/issues/21",
  "gh-aw uses head=`${owner}:${branch}` in pulls.list which returns [] for same-repo PRs.",
  "Patch the handler to use `head: branch` and filter by head.repo.owner.login in client.",
  "Auto-injected by .github/aw/scripts/compile-aw.ts - do not edit manually.",
];

const PATCH_RUN_LINES_2: string[] = [
  "set -euo pipefail",
  'handler="${RUNNER_TEMP}/gh-aw/actions/merge_pull_request.cjs"',
  'if [ ! -f "$handler" ]; then',
  '  echo "::warning::merge_pull_request.cjs not found at $handler; skipping patch"',
  "  exit 0",
  "fi",
  'cp "$handler" "$handler.bak"',
  "# Replace the head filter format from `${owner}:${branch}` to just `${branch}`.",
  "# The original line is: head: `${owner}:${branch}`,",
  // perl also interpolates $/{} on the replacement side of s///, so the
  // literal ${branch} placeholder must stay escaped there too.
  "perl -0pi -e 's/head: `\\$\\{owner\\}:\\$\\{branch\\}`,/head: `\\$\\{branch\\}`,/' \"$handler\"",
  'if cmp -s "$handler" "$handler.bak"; then',
  '  echo "::warning::Patch did not change merge_pull_request.cjs (pattern not found)"',
  "else",
  '  echo "Patched merge_pull_request.cjs: head filter now uses branch only"',
  "  # Add a client-side filter to preserve fork-safety: keep only PRs whose",
  "  # head.repo.owner.login matches the target owner.",
  '  perl -0pi -e \'s/const \\{ data: prs \\} = await withRetry\\(\\(\\) =>\\s*\\n\\s*githubClient\\.rest\\.pulls\\.list\\(\\{\\s*\\n\\s*owner,\\s*\\n\\s*repo,\\s*\\n\\s*state: "open",\\s*\\n\\s*head: `\\$\\{branch\\}`,\\s*\\n\\s*per_page: 1,\\s*\\n\\s*\\}\\)\\s*\\n\\s*\\);/const { data: __rawPrs } = await withRetry(() => githubClient.rest.pulls.list({ owner, repo, state: "open", head: `\\$\\{branch\\}`, per_page: 50 })); const prs = __rawPrs.filter(p => p.head?.repo?.owner?.login === owner);/s\' "$handler"',
  '  if grep -q "__rawPrs" "$handler"; then',
  '    echo "Added client-side fork-safety filter on head.repo.owner.login"',
  "  else",
  '    echo "::warning::Could not inject fork-safety filter; keeping branch-only filter"',
  "  fi",
  "fi",
  'rm -f "$handler.bak"',
];

/**
 * Builds the YAML block for a patch step.
 */
function buildStepYaml(name: string, commentLines: string[], runLines: string[]): string {
  const indentedRun = runLines.map(l => `          ${l}`).join("\n");

  return [
    `      - name: ${name}`,
    ...commentLines.map(c => `        # ${c}`),
    "        shell: bash",
    "        run: |",
    indentedRun,
  ].join("\n");
}

/**
 * Locates the start of the `steps:` list in the safe_outputs job, so
 * insertions can be scoped to that job (other jobs may reuse step names
 * like "Setup Scripts").
 */
function findStepsRegionStart(content: string): number {
  const safeOutputsMarker = '"${{ github.repository }}/agent-reviewer"';
  const safeOutputsIndex = content.indexOf(safeOutputsMarker);
  if (safeOutputsIndex === -1) {
    throw new Error("Could not locate safe_outputs job in lock file (expected GH_AW_CALLER_WORKFLOW_ID marker).");
  }

  const stepsKey = "\n    steps:";
  const stepsIndex = content.indexOf(stepsKey, safeOutputsIndex);
  if (stepsIndex === -1) {
    throw new Error("Could not locate 'steps:' in safe_outputs job.");
  }
  return stepsIndex;
}

/**
 * Inserts `stepYaml` right after the step named `afterStepName` within the
 * safe_outputs job's steps list. Idempotent: no-op if `stepName` is already
 * present anywhere in the file.
 */
function insertStepAfter(
  content: string,
  stepsRegionStart: number,
  afterStepName: string,
  stepName: string,
  stepYaml: string
): string {
  if (content.includes(stepName)) {
    logWarning(`Patch step "${stepName}" already present in lock file; skipping insertion.`);
    return content;
  }

  const afterStepMarker = `      - name: ${afterStepName}`;
  const afterStepIndex = content.indexOf(afterStepMarker, stepsRegionStart);
  if (afterStepIndex === -1) {
    throw new Error(`Could not locate step "${afterStepName}" in safe_outputs job.`);
  }

  // Find the end of the target step. It ends when we see the next
  // top-level step ("- name:") definition.
  const nextStepMarker = "\n      - name:";
  let nextStepIndex = content.indexOf(nextStepMarker, afterStepIndex + afterStepMarker.length);
  if (nextStepIndex === -1) {
    nextStepIndex = content.length;
  } else {
    nextStepIndex += 1; // skip past the \n
  }

  const before = content.slice(0, nextStepIndex);
  const after = content.slice(nextStepIndex);
  return `${before}${stepYaml}\n${after}`;
}

/**
 * Inserts both patch steps into the safe_outputs job, right after the
 * "Setup Scripts" step (chained in order), if not already present.
 *
 * @param lockContent The full content of agent-reviewer.lock.yml
 * @returns The modified lock file content
 */
function patchReviewerLock(lockContent: string): string {
  const stepsRegionStart = findStepsRegionStart(lockContent);

  let content = insertStepAfter(
    lockContent,
    stepsRegionStart,
    "Setup Scripts",
    PATCH_STEP_NAME,
    buildStepYaml(PATCH_STEP_NAME, PATCH_STEP_COMMENTS, PATCH_RUN_LINES)
  );
  content = insertStepAfter(
    content,
    stepsRegionStart,
    PATCH_STEP_NAME,
    PATCH_STEP_NAME_2,
    buildStepYaml(PATCH_STEP_NAME_2, PATCH_STEP_2_COMMENTS, PATCH_RUN_LINES_2)
  );
  return content;
}

function main(): void {
  log(`Repository root: ${REPO_ROOT}`);

  // Step 1: Compile all workflows.
  compileWorkflows();

  // Step 2: Verify the lock file exists.
  if (!existsSync(LOCK_FILE)) {
    throw new Error(`Lock file not found: ${LOCK_FILE}`);
  }

  // Step 3: Read, patch, and write the lock file.
  log(`Reading ${LOCK_FILE}...`);
  const originalContent = readFileSync(LOCK_FILE, "utf8");

  log("Patching agent-reviewer.lock.yml (merge_pull_request handler fixes)...");
  const patchedContent = patchReviewerLock(originalContent);

  if (patchedContent === originalContent) {
    logWarning("No changes applied to lock file.");
    return;
  }

  writeFileSync(LOCK_FILE, patchedContent, "utf8");
  logSuccess("Patched agent-reviewer.lock.yml with merge_pull_request handler fixes.");
  logSuccess("Done. The patch steps will run at runtime in the safe_outputs job.");
}

main();
