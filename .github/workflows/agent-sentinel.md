---
emoji: "🛡️"
name: Agent Sentinel
description: Run scheduled quality audits and raise focused follow-up issues.
on:
  schedule:
    - cron: "daily"
  workflow_dispatch:
    inputs:
      mission:
        description: Sentinel mission (bugs, tests, simplify)
        required: false
        type: choice
        default: ""
        options:
          - ""
          - bugs
          - tests
          - simplify
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
    toolsets: [issues, repos]
  bash:
    - "npm *"
    - "node *"
    - "npx *"
    - "cat *"
    - "jq *"
    - "rg *"
safe-outputs:
  create-issue:
    max: 2
    labels:
      - sentinel
      - needs-triage
  noop:
    report-as-issue: false
timeout-minutes: 30
source: PlainConceptsResearch/FrameStudio.Dev@e8d3bc481eb0aba9a8a61bd105428eeb84d7d106
---

# Agent Sentinel

## Task

Perform a lightweight health audit mission.

1. Determine mission:
- For `workflow_dispatch`, use `github.event.inputs.mission` when provided.
- Otherwise choose one from `bugs`, `tests`, `simplify`.
2. Use targeted repository reads/searches, not full scans.
3. Before creating a new issue, check for duplicates among open and closed issues labeled `sentinel`.
4. Create at most 2 high-confidence findings per run. Use clear evidence and measurable DoD.
5. Mission guidance:
- `bugs`: objective defect with expected vs actual behavior.
- `tests`: concrete missing tests with explicit success criteria.
- `simplify`: meaningful code reduction with concrete duplicated/dead code evidence.
6. If no high-confidence findings exist, call `noop` and explain briefly.

## Completion Rule

If no action is required, call `noop` with a short reason.
