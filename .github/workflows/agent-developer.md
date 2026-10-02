---
emoji: "🛠️"
name: Agent Developer
description: Implement ready issues and create/update PRs using Copilot harness.
on:
  issues:
    types: [labeled]
    names: [ready, changes-requested]
  bots: [framestudiodevghapp]
  workflow_dispatch:
    inputs:
      issue_number:
        description: Issue number
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
tools:
  github:
    mode: gh-proxy
    toolsets: [issues, pull_requests, repos]
  bash:
    - "npm *"
    - "node *"
    - "npx *"
    - "cat *"
    - "jq *"
safe-outputs:
  github-app:
    client-id: ${{ vars.APP_CLIENT_ID }}
    private-key: ${{ secrets.APP_PRIVATE_KEY }}
    permissions:
      administration: read
  create-pull-request:
    draft: false
    allowed-files:
      - "src/**"
      - "public/**"
      - "*.json"
      - "*.mjs"
      - "*.ts"
      - "*.tsx"
      - "*.md"
  push-to-pull-request-branch:
    max: 2
  add-labels:
    max: 2
    allowed: [in-review]
  remove-labels:
    max: 2
    allowed: [ready, changes-requested]
  add-comment:
    max: 2
  noop:
    report-as-issue: false
timeout-minutes: 40
source: PlainConceptsResearch/FrameStudio.Dev@e8d3bc481eb0aba9a8a61bd105428eeb84d7d106
---

# Agent Developer

## Task

Implement issue specs directly in the repository.

1. Resolve issue number using `${{ github.event.issue.number || github.event.inputs.issue_number }}`.
2. Guard clauses:
- If this is an `issues` event and label is neither `ready` nor `changes-requested`, call `noop` and stop.
- If issue number is missing or `0`, call `noop` and stop.
3. Read the issue's current labels.
- If neither `ready` nor `changes-requested` is currently present, call `noop` and stop.
4. Determine mode by checking for an open PR on branch `agents/issue-<N>` for this issue number:
- PR exists -> re-entry mode.
- No PR exists -> fresh mode.
5. Fresh mode:
- Read the parent issue spec and modify only the files required by that spec.
- If the issue is already implemented in the repository, add a comment with evidence and call `noop` instead of opening an empty PR.
- Validate before proposing changes:

```bash
npm ci
npm run build
npm run lint
```

6. Fresh mode PR handling:
- New work: use `create-pull-request` and include `Closes #<N>` in the body.
- When the PR is ready, in the same run call `add-labels` on the original issue number with labels `[in-review]` and `remove-labels` on the original issue number with labels `[ready, changes-requested]` to prevent redispatch loops.
7. Re-entry mode (`changes-requested`):
- Read the feedback context and fix only the requested changes.
- Validate again with install/build/lint commands.
- Push fixes to the same PR branch with `push-to-pull-request-branch` (never open a second PR).
- In the same run call `add-labels` on the original issue number with labels `[in-review]` and `remove-labels` on the original issue number with labels `[changes-requested]` to return the issue to review.

## Completion Rule

If no action is required, call `noop` with a short reason.
