---
emoji: "🏷️"
name: Agent Triager
description: Triage issues labeled needs-triage and route them to triaged or human-review.
on:
  issues:
    types: [labeled]
    names: [needs-triage]
  bots: [graph-engineering-framework]
  workflow_dispatch:
    inputs:
      issue:
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
    toolsets: [issues]
safe-outputs:
  github-app:
    client-id: ${{ vars.APP_CLIENT_ID }}
    private-key: ${{ secrets.APP_PRIVATE_KEY }}
  remove-labels:
    max: 1
  add-labels:
    max: 1
  add-comment:
    max: 1
  noop:
    report-as-issue: false
timeout-minutes: 15
source: PlainConceptsResearch/FrameStudio.Dev@e8d3bc481eb0aba9a8a61bd105428eeb84d7d106
---

# Agent Triager

## Task

Validate and triage issues.

1. Read the issue.
2. Remove label `needs-triage`
3. Add one of the following labels:
  - `triaged` when the request is clear, bounded, safe, and has a verifiable DoD
  - `human-review` when the request is dangerous, contradictory, too vague, or missing objective acceptance criteria
4. Add one concise comment with the decision and concrete rationale.

Do not modify code.

## Completion Rule

If no action is required, call `noop` with a short reason.
