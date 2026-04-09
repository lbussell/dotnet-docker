#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Compares the working tree to a remote branch and shows which files have changed.

.PARAMETER RemoteBranch
    The remote branch to compare against (e.g., 'origin/main'). Defaults to the upstream tracking branch.

.PARAMETER File
    Optional file path. When specified, shows the full diff for that file instead of the file list.

.EXAMPLE
    ./eng/compare-branch.ps1
    ./eng/compare-branch.ps1 -RemoteBranch origin/main
    ./eng/compare-branch.ps1 -RemoteBranch upstream/nightly -File eng/compare-branch.ps1
#>

param(
    [string]$RemoteBranch,
    [string]$File
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if (-not $RemoteBranch) {
    # Detect the upstream tracking branch
    $RemoteBranch = git rev-parse --abbrev-ref '@{upstream}' 2>$null
    if (-not $RemoteBranch) {
        Write-Error "No remote branch specified and no upstream tracking branch configured. Use -RemoteBranch to specify one."
        exit 1
    }
}

# Verify the remote branch exists
$null = git rev-parse --verify $RemoteBranch 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Error "Remote branch '$RemoteBranch' not found. Run 'git fetch' and try again."
    exit 1
}

if ($File) {
    git --no-pager diff --src-prefix "${RemoteBranch}:" --dst-prefix 'working-tree:' $RemoteBranch -- $File
    exit $LASTEXITCODE
}

$mergeBase = git merge-base HEAD $RemoteBranch
$diffArgs = @('diff', '--name-status', $mergeBase)

$changes = @(git @diffArgs)

if (-not $changes) {
    Write-Host "No differences between working tree and $RemoteBranch."
    exit 0
}

Write-Host "Changed files (working tree vs $RemoteBranch):"
Write-Host ""

foreach ($line in $changes) {
    Write-Host $line
}

Write-Host ""
Write-Host "Total: $($changes.Count) file(s) changed."
Write-Host "To see individual diffs, run: ./eng/compare-branch.ps1 -RemoteBranch $RemoteBranch -File <file>"
