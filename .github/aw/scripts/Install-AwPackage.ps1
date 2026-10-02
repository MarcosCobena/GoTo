<#
.SYNOPSIS
    Install or update the Loop Engineering Workflows gh-aw package in a target repository.

.DESCRIPTION
    Install mode (default):
      1. cd into the target repo
      2. gh aw init (if not already initialized)
      3. Commit the init files
      4. gh aw add-wizard <source-repo>@<source-ref>
      5. After the wizard merges the PR, pull latest
      6. Trigger aw-bootstrap-resources.yml to copy auxiliary resources
      7. Wait for the bootstrap workflow to complete

    Update mode (-Update):
      1. cd into the target repo
      2. Check working directory is clean
      3. gh aw add --force (overwrites existing workflows, creates a PR)
      4. Merge the PR and pull latest
      5. Trigger aw-bootstrap-resources.yml to update resources
      6. Wait for the bootstrap workflow to complete

    The GitHub App (APP_CLIENT_ID + APP_PRIVATE_KEY) must be installed on
    both the source and target repositories.

.PARAMETER SourceRepo
    Source repository in owner/repo format (e.g. PlainConceptsResearch/FrameStudio.Dev).

.PARAMETER SourceRef
    Source branch, tag, or SHA (e.g. feat/aw-package-installer).

.PARAMETER TargetRepoPath
    Absolute path to the target repository working copy (e.g. D:\Projects\Multiverse\Nexus).

.PARAMETER SkipInit
    Skip the 'gh aw init' step if the repo is already initialized.

.PARAMETER Update
    Update an existing installation: use 'gh aw add --force' to overwrite
    workflows and re-run the bootstrap to refresh resources.

.EXAMPLE
    .\Install-AwPackage.ps1 `
        -SourceRepo PlainConceptsResearch/FrameStudio.Dev `
        -SourceRef feat/aw-package-installer `
        -TargetRepoPath D:\Projects\Multiverse\Nexus

.EXAMPLE
    .\Install-AwPackage.ps1 `
        -SourceRepo PlainConceptsResearch/FrameStudio.Dev `
        -SourceRef feat/aw-package-installer `
        -TargetRepoPath D:\Projects\Multiverse\Nexus `
        -Update
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$SourceRepo,

    [Parameter(Mandatory)]
    [string]$SourceRef,

    [Parameter(Mandatory)]
    [string]$TargetRepoPath,

    [switch]$SkipInit,

    [switch]$Update
)

$ErrorActionPreference = 'Stop'

function Invoke-Step {
    param([string]$Description, [scriptblock]$Action)
    Write-Host "`n=== $Description ===" -ForegroundColor Cyan
    & $Action
    if ($LASTEXITCODE -ne 0 -and $null -ne $LASTEXITCODE) {
        throw "Step failed: $Description (exit code: $LASTEXITCODE)"
    }
}

# ── 0. Validate prerequisites ──────────────────────────────────────────────

Invoke-Step 'Checking prerequisites' {
    foreach ($cmd in 'gh', 'git') {
        if (-not (Get-Command $cmd -ErrorAction SilentlyContinue)) {
            throw "'$cmd' not found. Please install it and ensure it's on PATH."
        }
    }
    Write-Host "gh: $(gh --version 2>&1 | Select-Object -First 1)"
    Write-Host "git: $(git --version 2>&1)"
    if ($Update) {
        Write-Host 'Mode: UPDATE' -ForegroundColor Yellow
    } else {
        Write-Host 'Mode: INSTALL' -ForegroundColor Yellow
    }
}

# ── 1. Switch to target repo ───────────────────────────────────────────────

Invoke-Step "Switching to target repo: $TargetRepoPath" {
    if (-not (Test-Path $TargetRepoPath)) {
        throw "Target repo path does not exist: $TargetRepoPath"
    }
    Set-Location $TargetRepoPath
    $remote = git remote get-url origin 2>&1
    Write-Host "Remote: $remote"
    Write-Host "Branch: $(git branch --show-current)"
}

# ── 2. Ensure working directory is clean ───────────────────────────────────

Invoke-Step 'Checking working directory is clean' {
    $status = git status --porcelain 2>&1
    if ($status) {
        Write-Host $status
        throw "Working directory is not clean. Please commit or stash changes first."
    }
    Write-Host 'Working directory is clean.'
}

# ── 3. gh aw init (install mode only, if needed) ───────────────────────────

if ($Update) {
    Write-Host "`n=== Skipping gh aw init (update mode) ===" -ForegroundColor Yellow
}
elseif ($SkipInit) {
    Write-Host "`n=== Skipping gh aw init (--SkipInit) ===" -ForegroundColor Yellow
}
else {
    $needsInit = -not (
        (Test-Path '.gitattributes') -and
        (Test-Path '.github/mcp.json') -and
        (Test-Path '.github/workflows/copilot-setup-steps.yml')
    )

    if ($needsInit) {
        Invoke-Step 'Running gh aw init' {
            gh aw init
        }

        Invoke-Step 'Committing init files' {
            git add -A
            git commit -m 'init: gh-aw setup'
            git push # FIXME add private key for SSH remotes
        }
    }
    else {
        Write-Host "`n=== Skipping gh aw init (already initialized) ===" -ForegroundColor Yellow
    }
}

# ── 4. Install or update workflows ─────────────────────────────────────────

if ($Update) {
    Invoke-Step "Running gh aw add --force (non-interactive update)" {
        Write-Host "Source: $SourceRepo@$SourceRef" -ForegroundColor Yellow
        Write-Host ''

        gh aw add "$SourceRepo@$SourceRef" --force
    }
}
else {
    Invoke-Step "Running gh aw add-wizard (interactive)" {
        Write-Host "Source: $SourceRepo@$SourceRef" -ForegroundColor Yellow
        Write-Host ''
        Write-Host 'The wizard will:'
        Write-Host '  1. Create a PR with workflow files'
        Write-Host '  2. Offer to merge the PR'
        Write-Host '  3. Execute config steps (GitHub App, Copilot auth, etc.)'
        Write-Host ''
        Write-Host 'After the wizard merges the PR, it will run the config steps'
        Write-Host 'and show the handoff message.'
        Write-Host ''

        gh aw add-wizard "$SourceRepo@$SourceRef" # gh aw add-wizard ..\FrameStudio\aw.yml (además ayer usamos gh aw add, que es para actualizar)
        # Ha fallado al hacer push de la rama, lo he hecho a mano, PR y merge. Variable y secret a mano también
    }
}

# ── 5. Pull latest (wizard/add may have merged PR) ─────────────────────────

Invoke-Step 'Pulling latest changes' {
    $defaultBranch = gh repo view --json defaultBranchRef --jq '.defaultBranchRef.name' 2>&1
    if (-not $defaultBranch) { $defaultBranch = 'main' }
    Write-Host "Default branch: $defaultBranch"
    git checkout $defaultBranch
    git pull origin $defaultBranch
}

# ── 6. Trigger aw-bootstrap-resources.yml ──────────────────────────────────

Invoke-Step 'Triggering aw-bootstrap-resources.yml' {
    $targetRepo = gh repo view --json nameWithOwner --jq '.nameWithOwner' 2>&1
    Write-Host "Target repo: $targetRepo"

    gh workflow run aw-bootstrap-resources.yml `
        --repo $targetRepo `
        -f "source-repo=$SourceRepo" `
        -f "source-ref=$SourceRef" # Falla por OAuth, no sé cómo solucionarlo. A mano

    Write-Host 'Workflow triggered. Waiting for it to start...'
    Start-Sleep -Seconds 5

    $run = gh run list `
        --workflow=aw-bootstrap-resources.yml `
        --repo $targetRepo `
        --limit 1 `
        --json databaseId,status,createdAt `
        --jq '.[0].databaseId' 2>&1

    if (-not $run) {
        throw 'Could not find the workflow run. Check the Actions tab manually.'
    }

    Write-Host "Run ID: $run"
    Write-Host 'Watching run (this may take a few minutes)...'
    gh run watch $run --repo $targetRepo

    if ($LASTEXITCODE -ne 0) {
        throw 'Bootstrap workflow failed. Check the logs: gh run view $run --repo $targetRepo --log'
    }
}

# ── 7. Done ────────────────────────────────────────────────────────────────

Write-Host ''
Write-Host '============================================' -ForegroundColor Green
if ($Update) {
    Write-Host '  Update complete!' -ForegroundColor Green
} else {
    Write-Host '  Installation complete!' -ForegroundColor Green
}
Write-Host '============================================' -ForegroundColor Green
Write-Host ''
Write-Host 'Next steps:'
Write-Host '  1. Install dependencies:  pnpm install'
Write-Host '  2. Compile workflows:     pnpm compile-aw'
Write-Host '  3. Start the loop:         label an issue with "needs-triage"'
Write-Host ''
