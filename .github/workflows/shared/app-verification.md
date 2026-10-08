---
# Shared component: lets an agent run the app it builds and check it.
# Web apps are driven with playwright-cli; desktop apps run on a virtual X display.
# gh-aw only runs the agent job on Linux, so desktop apps must run on Linux.
tools:
  playwright:
    mode: cli
network:
  allowed:
    - local
    - playwright
steps:
  - name: Install virtual display tooling
    run: |
      sudo apt-get update
      sudo apt-get install -y --no-install-recommends fonts-dejavu-core imagemagick x11-utils xdotool xvfb
safe-outputs:
  upload-artifact:
    max-uploads: 1
    retention-days: 14
---

## Running the app

Building and passing tests does not prove the app works. When the repository's agent instructions (`AGENTS.md`, `.github/copilot-instructions.md`) have a "Run the app" section, follow it to start the app and check it before reporting the work as done.

- Start the app yourself from `bash`, in the background, and wait until it is ready. Services started outside your sandbox are not reachable.
- Web apps: drive a browser with `playwright-cli` against the local URL: `playwright-cli open <url>`, `playwright-cli screenshot --filename=<file>.png`, `playwright-cli snapshot` to list elements with their refs, and `playwright-cli click <ref>` with a ref from that snapshot (clicking by visible text is not supported). Run `playwright-cli --help` if a command is rejected.
- Desktop apps: start a display with `Xvfb :99 -screen 0 1280x800x24 &` and run the app with `DISPLAY=:99`. Capture with `DISPLAY=:99 import -window root <file>.png` and send input with `DISPLAY=:99 xdotool ...`.
- When the expected behaviour involves interaction, interact (click, type) and take a screenshot after each meaningful step.
- Save every screenshot under `/tmp/gh-aw/agent/app-verification/` and upload that folder with `upload_artifact`, so a human can look at them.

Images you open may not reach you: in this environment the `view` tool can report success without showing you the picture. Do not claim anything about what a screenshot shows unless you actually saw it. Base your verdict on checks you can run:

- The process starts and stays running, and its output has no errors.
- For web apps, the URL answers `200` (`curl -s -o /dev/null -w '%{http_code}' <url>`), and the browser console has no errors, if `playwright-cli --help` lists a way to read it.
- The screenshot is not blank: `identify -format '%[fx:standard_deviation]' <file>.png` must be greater than `0`. A blank page or empty window gives `0`.
- Screenshots taken before and after an interaction differ (`compare -metric AE <before>.png <after>.png null: 2>&1` must not be `0`) when the interaction should change the screen.
- Any checks the "Run the app" section asks for.

Stop every process you started when you are done. Report which checks you ran and their results. If you could not start or check the app, say so explicitly. Never describe something you did not check as verified.
