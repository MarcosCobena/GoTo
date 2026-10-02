// Scenario definitions for testing .github/workflows/agent-developer.md via
// .github/tests/agent-developer/run.js.
//
// Each scenario is a sequence of turns against the same issue unless the turn
// explicitly requests dispatch without trigger context.

const readyIssueBody = [
  "Implement a small UX tweak in the gallery.",
  "",
  "Context:",
  "- The image gallery is in src/components/ImageGallery.tsx.",
  "",
  "Plan:",
  "- Add a keyboard shortcut hint text under the gallery title.",
  "- Keep the change scoped to this file only.",
  "",
  "Definition of Done:",
  "- Hint text is visible under the gallery title.",
  "- Build and lint pass.",
  "",
  "Non-goals:",
  "- No new dependencies.",
  "",
  "Files to modify:",
  "- src/components/ImageGallery.tsx",
].join("\n");

const changesRequestedFollowUp = [
  "Please adjust the previous implementation:",
  "",
  "1. Make the hint text more concise.",
  "2. Keep existing typography classes unchanged.",
  "",
  "No additional files should be touched.",
].join("\n");

const alreadyImplementedBody = [
  "Add a Feedback link to the bottom-left of the main page.",
  "",
  "Context:",
  "- The page layout is in src/app/page.tsx.",
  "",
  "Plan:",
  "- Add an anchor to https://github.com/EvergineTeam/FrameStudio/issues/new/choose",
  "- Position it bottom-left and keep style aligned with existing version badge.",
  "",
  "Definition of Done:",
  "- Link exists with target _blank and rel noopener noreferrer.",
  "",
  "Files to modify:",
  "- src/app/page.tsx",
].join("\n");

module.exports = [
  {
    name: "ready-happy-path",
    turns: [
      {
        title: "Ready issue: small gallery tweak",
        body: readyIssueBody,
        initialLabels: ["ready"],
        expect: {
          createPr: true,
          pushToExistingPr: false,
          addLabels: ["in-review"],
          removeLabels: ["ready"],
          comment: false,
          noop: false,
          maxPrCreated: 1,
        },
      },
    ],
  },
  {
    name: "changes-requested-reentry",
    turns: [
      {
        title: "Ready issue that will require a follow-up",
        body: readyIssueBody,
        initialLabels: ["ready"],
        expect: {
          createPr: true,
          pushToExistingPr: false,
          addLabels: ["in-review"],
          removeLabels: ["ready"],
          comment: false,
          noop: false,
          maxPrCreated: 1,
        },
      },
      {
        simulateChangesRequested: true,
        reviewComment: changesRequestedFollowUp,
        expect: {
          createPr: false,
          pushToExistingPr: true,
          addLabels: ["in-review"],
          removeLabels: ["changes-requested"],
          comment: false,
          noop: false,
          maxPrCreated: 0,
        },
      },
    ],
  },
  {
    name: "already-implemented-no-empty-pr",
    turns: [
      {
        title: "Ready issue already implemented in source",
        body: alreadyImplementedBody,
        initialLabels: ["ready"],
        expect: {
          createPr: false,
          pushToExistingPr: false,
          addLabels: [],
          removeLabels: [],
          comment: true,
          noop: true,
          maxPrCreated: 0,
        },
      },
    ],
  },
  {
    name: "missing-issue-number",
    turns: [
      {
        dispatchWithoutContext: true,
        expect: {
          createPr: false,
          pushToExistingPr: false,
          addLabels: [],
          removeLabels: [],
          comment: false,
          noop: true,
          maxPrCreated: 0,
        },
      },
    ],
  },
  {
    name: "wrong-trigger-label",
    turns: [
      {
        title: "Issue has non-dispatch label",
        body: readyIssueBody,
        initialLabels: ["triaged"],
        expect: {
          createPr: false,
          pushToExistingPr: false,
          addLabels: [],
          removeLabels: [],
          comment: false,
          noop: true,
          maxPrCreated: 0,
        },
      },
    ],
  },
];
