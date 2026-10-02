// Scenario definitions for testing .github/workflows/agent-planner.md via
// `gh aw trial` (see .github/tests/agent-planner/run.js).
//
// Each scenario is a sequence of "turns" executed against the same GitHub
// issue:
//   - The first turn creates the issue (title/body/initialLabels).
//   - Later turns optionally post a comment before re-running the trial
//     against the same issue (used for the needs-info re-evaluation cases).
// Every turn has an `expect` block describing the safe outputs the planner
// must produce for that turn.

const clearIssueBody = [
  'We need a "Copy to clipboard" button on each image card in the gallery.',
  "",
  "- Add a button overlay on each thumbnail in src/components/ImageGallery.tsx.",
  "- Clicking it copies the image's data URL to the clipboard using the",
  "  navigator.clipboard.writeText API.",
  "- Show a small 'Copied!' tooltip for 2 seconds after a successful copy.",
  "",
  "Definition of Done:",
  "- Button appears on hover over each gallery thumbnail.",
  "- Clicking it copies the correct image data URL.",
  "- A 'Copied!' tooltip appears and disappears after ~2 seconds.",
  "- npm run build and npm run lint pass.",
].join("\n");

const largeIssueBody = [
  "Add multi-language support (i18n) to the app.",
  "",
  "- Extract all user-facing strings from src/components/*.tsx into a",
  "  translation-friendly format.",
  "- Add a language switcher dropdown somewhere in the top-level layout",
  "  (src/app/layout.tsx) with at least English and Spanish.",
  "- Persist the selected language across page reloads (localStorage is fine).",
  "",
  "Definition of Done:",
  "- No hardcoded user-facing strings remain in the affected components.",
  "- Switching languages updates all visible text without a full reload.",
  "- The chosen language survives a browser refresh.",
  "- npm run build and npm run lint pass.",
].join("\n");

const ambiguousIssueBody = [
  "The image generation experience feels clunky, can we make it better?",
  "",
  "Not sure exactly what to prioritize, just generally improve it.",
].join("\n");

const answeredFollowUpComment = [
  "Answers to your questions:",
  "",
  "1. Add a loading spinner overlay on ImageGeneration.tsx while the fal.ai",
  "   request is in flight.",
  "2. Add a 'Cancel' button next to the spinner that aborts the in-flight",
  "   request (use an AbortController with the fal client call).",
  "3. Definition of Done: spinner is visible for the duration of the",
  "   request; clicking Cancel stops the request and hides the spinner;",
  "   npm run build and npm run lint pass. No automated tests required.",
].join("\n");

module.exports = [
  {
    name: "clear-bounded-issue",
    turns: [
      {
        title: "Add copy-to-clipboard button to gallery images",
        body: clearIssueBody,
        initialLabels: ["triaged"],
        expect: {
          addLabels: ["planned"],
          removeLabels: ["triaged"],
          minIssuesCreated: 1,
          comment: true,
        },
      },
    ],
  },
  {
    name: "large-multi-deliverable-issue",
    turns: [
      {
        title: "Add multi-language support (i18n)",
        body: largeIssueBody,
        initialLabels: ["triaged"],
        expect: {
          addLabels: ["planned"],
          removeLabels: ["triaged"],
          minIssuesCreated: 2,
          comment: true,
        },
      },
    ],
  },
  {
    name: "ambiguous-issue-needs-info",
    turns: [
      {
        title: "Improve image generation UX",
        body: ambiguousIssueBody,
        initialLabels: ["triaged"],
        expect: {
          addLabels: ["needs-info"],
          removeLabels: ["triaged"],
          comment: true,
        },
      },
    ],
  },
  {
    name: "needs-info-answered",
    turns: [
      {
        title: "Improve image generation UX",
        body: ambiguousIssueBody,
        initialLabels: ["triaged"],
        expect: {
          addLabels: ["needs-info"],
          removeLabels: ["triaged"],
          comment: true,
        },
      },
      {
        comment: answeredFollowUpComment,
        expect: {
          addLabels: ["planned"],
          removeLabels: ["needs-info"],
          minIssuesCreated: 1,
          comment: true,
        },
      },
    ],
  },
  {
    name: "needs-info-no-reply",
    turns: [
      {
        title: "Improve image generation UX",
        body: ambiguousIssueBody,
        initialLabels: ["triaged"],
        expect: {
          addLabels: ["needs-info"],
          removeLabels: ["triaged"],
          comment: true,
        },
      },
      {
        comment: null,
        expect: {
          noop: true,
        },
      },
    ],
  },
];
