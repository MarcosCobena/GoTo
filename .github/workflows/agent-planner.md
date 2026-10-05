---
emoji: "🧭"
name: Agent Planner
description: Plan triaged issues into implementation-ready sub-issues.
on:
  issues:
    types: [labeled]
    names: [triaged]
  issue_comment:
    types: [created]
  bots: [graph-engineering-framework]
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
    toolsets: [issues, repos]
safe-outputs:
  github-app:
    client-id: ${{ vars.APP_CLIENT_ID }}
    private-key: ${{ secrets.APP_PRIVATE_KEY }}
  create-issue:
    max: 6
  link-sub-issue:
    max: 6
  remove-labels:
    max: 1
    allowed: [triaged, needs-info]
  add-labels:
    max: 1
    allowed: [needs-info, planned]
  add-comment: {}
  noop:
    report-as-issue: false
timeout-minutes: 25
source: PlainConceptsResearch/FrameStudio.Dev@e8d3bc481eb0aba9a8a61bd105428eeb84d7d106
---

# Agent Planner

## Context efficiency (CRITICAL)

Every file you read stays in your context and is re-sent on every subsequent
tool call. You are PLANNING, not implementing — you need the shape of the
code, not all of it. Rules:

- `search_code` first: locate the components, exports, and patterns the issue
  touches. Use `get_file_contents` only when the spec you are writing must
  reference a file's internals (function signatures, prop types, config keys).
- HARD LIMIT per run: at most 5 `get_file_contents` calls. Every file you
  open consumes context that you need later for creating sub-issues, linking,
  labeling, and commenting. Never read test files unless the issue is about
  tests.
- Batch several searches/reads in the SAME tool-call turn when possible.
- Stop exploring early enough to write ALL output actions. Reserve at least
  one tool-call turn per sub-issue you plan to create, plus turns for
  `link-sub-issue`, `add-labels`, `remove-labels`, and `add-comment`. A spec
  pointing at the right files beats an exhaustive one that never gets created.

## Task

Plan and decompose issues without writing code.

1. Resolve issue number: the issue number for this run is `${{ github.event.issue.number || github.event.inputs.issue_number }}`.
2. Guard clauses:
- If event is `issues` and `github.event.label.name` is not `triaged`, call `noop` and stop.
- If event is `issue_comment`, only proceed when the issue currently has label `needs-info`. If not, call `noop`.
- If issue number is missing or `0`, call `noop`.
3. Determine mode from the issue's current label:
- `triaged` -> follow "Plan a new issue".
- `needs-info` -> follow "Re-evaluate after human reply".
4. Plan a new issue: read the issue and explore the codebase with `search_code` and `get_file_contents` as needed. Decide if you have enough information:
- Concrete implementation details (values, formats, endpoints, algorithms).
- Decisions only the requester can make (replace vs extend, naming, UX choices).
- Unambiguous, verifiable acceptance criteria.
- Missing any of the above -> follow "Request information".
- Everything clear -> follow "Decide size" then "Create ready issue(s)".
5. Request information: post one comment with numbered questions. For each question, explain what decision it blocks and suggest a default when reasonable ("If no preference, I'll go with X"). In the same run, `add-labels` (`needs-info`) and `remove-labels` (`triaged`). Never create `ready` issues with placeholder values while waiting for answers.
6. Re-evaluate after human reply (only reached when the current label is `needs-info`): read every comment posted after your last comment on the issue.
- All questions answered -> continue to "Decide size" / "Create ready issue(s)" in this same run; once done, `add-labels` (`planned`) and `remove-labels` (`needs-info`).
- Some answered, some outstanding -> post one follow-up comment listing only the remaining questions. Do not change labels.
- No new comment from a human -> call `noop`. Never repeat the same questions.
7. Decide size: count distinct, independently-verifiable deliverables in the request (a deliverable is a piece of work that could be implemented and verified on its own).
- 1 deliverable, or 2 that only make sense together -> create a single `ready` issue.
- 2+ independently-verifiable deliverables, or work spanning unrelated concerns -> split into one `ready` issue per deliverable.
- Signs the work must be split: 3+ independently-verifiable DoD criteria; the plan touches 3+ files for unrelated reasons; the feature has natural sequential phases.
8. Create ready issue(s). Each one must include:
- **Context**: what exists now, which files are involved, relevant code patterns.
- **Plan**: step-by-step implementation instructions.
- **Definition of Done**: specific, verifiable criteria.
- **Non-goals**: what is explicitly out of scope.
- **Files to modify**: exact paths and what changes each needs.
Label each sub-issue through the `labels` field of `create-issue`:
- `["ready"]` when none of its files to modify are protected. The Developer picks it up.
- `["human-review"]` when any of them is protected, and add a short note to the body naming the protected files. The Developer cannot deliver protected files: gh-aw refuses to push them, so a human has to implement or apply the change.
Protected files are: anything under a top-level directory whose name starts with `.` (`.github/`, `.agents/`, `.vscode/`, ...); agent instruction files (`AGENTS.md`, `CLAUDE.md`, `GEMINI.md`); top-level `README.md`, `CONTRIBUTING.md`, `SECURITY.md` and `CODE_OF_CONDUCT.md`; `CODEOWNERS` and `DESIGN.md`; dependency manifests and lockfiles (`package.json`, `package-lock.json`, `yarn.lock`, `pnpm-lock.yaml`, `go.mod`, `go.sum`, `pyproject.toml`, `requirements.txt`, `Gemfile`, `pom.xml`, `build.gradle`, `global.json`, `NuGet.Config`, `Directory.Packages.props`, ...).
When only part of the work touches protected files, split that part into its own `human-review` sub-issue so the rest can stay `ready`.
After creating each sub-issue, call `link-sub-issue` with the original issue as parent and the new issue as sub-issue. When a sub-issue depends on another one landing first, include a line `Depends on #N` in its body.
9. Finalize: only once at least one `create-issue` call has succeeded, `add-labels` (`planned`) and `remove-labels` (`triaged` or `needs-info`, whichever is currently set) in the same run, plus one summary comment listing the created issue numbers. If no sub-issue could be created, leave labels untouched so the next run retries.

Never implement code in this workflow. Never label the original issue `ready`. Never mark it `planned` without at least one successfully created sub-issue.

## Completion Rule

If no action is required, call `noop` with a short reason.
