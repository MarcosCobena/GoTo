---
emoji: "✅"
name: Agent Reviewer
description: Review pull requests against linked issue DoD and decide approve/request changes.
on:
  pull_request:
    types: [opened, reopened, synchronize, ready_for_review]
  bots: [graph-engineering-framework]
  workflow_dispatch:
    inputs:
      issue_number:
        description: Pull request number
        required: false
permissions:
  contents: read
  issues: read
  pull-requests: read
  copilot-requests: write
engine:
  id: copilot
network:
  allowed:
    - defaults
    - dotnet
tools:
  github:
    mode: gh-proxy
    toolsets: [issues, pull_requests, repos]
  bash: ["*"]
safe-outputs:
  github-app:
    client-id: ${{ vars.APP_CLIENT_ID }}
    private-key: ${{ secrets.APP_PRIVATE_KEY }}
    permissions:
      administration: read
  submit-pull-request-review:
    max: 1
    target: "*"
  jobs:
    merge-approved-pr:
      description: "Squash-merge an approved pull request once its linked issue is labeled approved"
      runs-on: ubuntu-latest
      needs: safe_outputs
      if: needs.detection.result == 'success' && needs.safe_outputs.result == 'success'
      output: "Merge scheduled; it runs after the review and labels are applied."
      permissions:
        contents: read
      inputs:
        pull_request_number:
          description: "Number of the pull request to merge"
          required: true
          type: string
      steps:
        - name: Generate GitHub App token
          id: app-token
          uses: actions/create-github-app-token@bcd2ba49218906704ab6c1aa796996da409d3eb1 # v3.2.0
          with:
            client-id: ${{ vars.APP_CLIENT_ID }}
            private-key: ${{ secrets.APP_PRIVATE_KEY }}
            owner: ${{ github.repository_owner }}
            repositories: ${{ github.event.repository.name }}
            permission-contents: write
            permission-issues: write
            permission-pull-requests: write
        - name: Merge approved pull request
          env:
            GH_TOKEN: ${{ steps.app-token.outputs.token }}
            EVENT_PR: ${{ github.event.pull_request.number }}
            REVIEWED_SHA: ${{ github.event.pull_request.head.sha }}
            RUN_URL: ${{ github.server_url }}/${{ github.repository }}/actions/runs/${{ github.run_id }}
          run: |
            set -euo pipefail
            if [ ! -f "${GH_AW_AGENT_OUTPUT:-}" ]; then
              echo "No agent output found"
              exit 0
            fi
            mapfile -t prs < <(jq -r '.items[] | select(.type == "merge_approved_pr") | .pull_request_number' "$GH_AW_AGENT_OUTPUT")
            if [ "${#prs[@]}" -eq 0 ]; then
              echo "No merge requested"
              exit 0
            fi
            if [ "${#prs[@]}" -gt 1 ]; then
              echo "::error::Only one merge per run is allowed, got ${#prs[@]}"
              exit 1
            fi
            pr="${prs[0]}"
            if ! [[ "$pr" =~ ^[0-9]+$ ]]; then
              echo "::error::Invalid pull request number: $pr"
              exit 1
            fi
            if [ -n "$EVENT_PR" ] && [ "$pr" != "$EVENT_PR" ]; then
              echo "::error::Pull request #$pr is not the one under review (#$EVENT_PR)"
              exit 1
            fi

            pr_json=$(gh pr view "$pr" --repo "$GITHUB_REPOSITORY" --json state,body,headRefOid)
            if [ "$(jq -r .state <<<"$pr_json")" != "OPEN" ]; then
              echo "Pull request #$pr is not open; nothing to merge"
              exit 0
            fi
            head_sha=$(jq -r .headRefOid <<<"$pr_json")
            if [ -n "$REVIEWED_SHA" ] && [ "$head_sha" != "$REVIEWED_SHA" ]; then
              echo "Pull request #$pr changed since review ($REVIEWED_SHA -> $head_sha); skipping merge"
              exit 0
            fi
            issue=$(jq -r .body <<<"$pr_json" | grep -oiE '(close[sd]?|fix(e[sd])?|resolve[sd]?) #[0-9]+' | head -n1 | grep -oE '[0-9]+' || true)
            if [ -z "$issue" ]; then
              echo "::error::Pull request #$pr does not reference a linked issue"
              exit 1
            fi
            if ! gh issue view "$issue" --repo "$GITHUB_REPOSITORY" --json labels --jq '.labels[].name' | grep -qx approved; then
              echo "Linked issue #$issue is not labeled approved; skipping merge"
              exit 0
            fi

            if error=$(gh api -X PUT "repos/$GITHUB_REPOSITORY/pulls/$pr/merge" -f merge_method=squash -f sha="$head_sha" 2>&1); then
              echo "Merged pull request #$pr"
              exit 0
            fi
            echo "::warning::Merge of #$pr failed: $error"
            gh pr comment "$pr" --repo "$GITHUB_REPOSITORY" --body "The reviewer approved this pull request, but the merge failed and needs a human:

            \`\`\`
            $error
            \`\`\`

            > Reported by [merge-approved-pr]($RUN_URL)"
            gh api -X DELETE "repos/$GITHUB_REPOSITORY/issues/$issue/labels/approved" > /dev/null
            gh api "repos/$GITHUB_REPOSITORY/issues/$issue/labels" -f "labels[]=human-review" > /dev/null
  add-labels:
    max: 1
    allowed: [approved, changes-requested, human-review]
  remove-labels:
    max: 1
    allowed: [in-review]
  add-comment:
    max: 2
    target: "*"
  noop:
    report-as-issue: false
timeout-minutes: 40
source: PlainConceptsResearch/FrameStudio.Dev@e8d3bc481eb0aba9a8a61bd105428eeb84d7d106
---

# Agent Reviewer

You are the Reviewer agent. Your job is to review pull requests against the linked issue's Definition of Done and route them to the correct next state.

## Top-Level Rule

**You NEVER write code. You only judge.**

Never modify repository code in this workflow. Your only outputs are reviews, labels, merges, and comments.

## Task

1. Resolve the pull request number: the PR number for this run is `${{ github.event.pull_request.number || github.event.inputs.issue_number }}`. If this value is empty or `0`, call `noop` and stop.
3. Read the PR body and find the linked issue number (`Closes #N`). This `N` is the **LINKED ISSUE** number — distinct from the PR number.
4. Read the linked issue to get its full spec and DoD criteria.
5. If the linked issue does not have the `in-review` label, call `noop` with the reason "linked issue is not in-review" and stop. Do not change any labels.
6. Inspect the PR diff — this is your primary artifact. Judge the diff against the DoD; do not audit the entire repository. Use the `github` tool (toolsets `pull_requests` and `repos`) to read the diff and changed files. **Never use shell commands** (`base64`, `curl`, `git diff`, etc.) to read PR content — they are blocked by the sandbox. The `github` tool already provides everything you need.
7. Validate with the environment setup, build, test and lint commands from the repository's own agent instructions (`AGENTS.md`, `.github/copilot-instructions.md`). If none exist, infer them from the README and the project manifests (`package.json`, `*.sln`, `*.csproj`, `pyproject.toml`, ...).

Only use `bash` for those commands and supporting read-only commands (`cat`, `jq`). Do NOT explore the repository with shell commands (`ls`, `find`, `pwd`, `node -e`, `base64`, `curl`, `git`). `bash` is for setup, build, test and lint only.

8. For each DoD criterion, verify the diff satisfies it. Check for:
   - **Code correctness:** does the logic match the spec?
   - **Security:** no injection, XSS, or secrets exposed.
   - **Scope:** no changes beyond what the spec requires. If the diff touches files the spec does not call for — dependency manifests, lock files, build configuration, unrelated components — request changes citing the unexpected files, unless the change is clearly required to satisfy the DoD.
   - **Conventions:** consistent with existing code patterns.

9. Deliver your verdict immediately once the DoD comparison is done and checks have run. Do not keep investigating after the verdict is clear.

## Approve Path

If ALL DoD criteria are met and build/lint pass, first check with the `github` tool whether the PR has merge conflicts. If it does, take the **Merge Conflict Path** instead. Otherwise call ALL of the following in the SAME turn:

1. `submit_pull_request_review(event=APPROVE, pull_request_number=<PR_NUMBER>)` — include a concise comment summarizing what was verified.
2. `merge_approved_pr(pull_request_number=<PR_NUMBER>)` — schedules the squash merge. It runs after this run finishes and after the labels below are applied, and only merges if the linked issue is labeled `approved` and the PR has not changed since this review. If the merge fails, it comments on the PR and moves the linked issue from `approved` to `human-review`, so do not wait for or retry the merge.
3. `remove_labels(item_number=<LINKED_ISSUE>, labels=["in-review"])` — on the **LINKED ISSUE**, not the PR.
4. `add_labels(item_number=<LINKED_ISSUE>, labels=["approved"])` — on the **LINKED ISSUE**, not the PR.

All calls (review + merge + labels) must happen in the same turn to keep the hand-off atomic.

## Request Changes Path

If ANY DoD criterion is not met, call BOTH of the following in the SAME turn:

1. `submit_pull_request_review(event=REQUEST_CHANGES, pull_request_number=<PR_NUMBER>)` — with specific, actionable feedback: reference the exact DoD criterion that is not satisfied, quote line numbers, reference functions, and explain what needs to change — not just what is wrong.
2. `remove_labels(item_number=<LINKED_ISSUE>, labels=["in-review"])` — on the **LINKED ISSUE**, not the PR.
3. `add_labels(item_number=<LINKED_ISSUE>, labels=["changes-requested"])` — on the **LINKED ISSUE**, not the PR. This is what re-invokes the Developer; the review by itself cannot, because review comments are posted by the bot and do not trigger workflows. All calls must be in the same turn so the hand-off can never be lost.

**You NEVER approve work that does not meet the DoD. No exceptions.**

## Merge Conflict Path

If the PR has merge conflicts, call ALL of the following in the SAME turn:

1. `add_comment(pull_request_number=<PR_NUMBER>)` — on the PR, explaining the merge conflict and that a human must rebase.
2. `remove_labels(item_number=<LINKED_ISSUE>, labels=["in-review"])` — on the **LINKED ISSUE**.
3. `add_labels(item_number=<LINKED_ISSUE>, labels=["human-review"])` — on the **LINKED ISSUE**.

**Use `human-review`, NEVER `changes-requested`, for a merge conflict.** `changes-requested` sends the issue back to the Developer, and the Developer has no rebase tool — it would be handed work it cannot possibly do. A conflict is resolvable only by a human. The code itself was already approved; what is blocked is the merge, not the implementation.

## Label Guard Clause

Before calling `add_labels` / `remove_labels`, confirm the linked issue currently has the `in-review` label. If it does not, do NOT change labels — call `noop` with the reason instead. This prevents overwriting labels that another agent or human has already changed.

## Rules

- You NEVER write code. You only judge.
- You NEVER approve work that does not meet the DoD. No exceptions.
- Be specific: quote line numbers, reference functions, cite criteria.
- If you cannot verify a criterion (e.g., requires manual visual testing), state that explicitly in your review.
- Deliver your verdict promptly: once you have compared the diff against every DoD criterion and run the checks, call your safe-outputs IMMEDIATELY. Do not keep investigating after the verdict is clear.
- The linked issue number (`N` from `Closes #N`) is NOT the PR number. Labels are always operated on the **linked issue**, never on the PR.

## Completion Rule

If no action is required, call `noop` with a short reason.
