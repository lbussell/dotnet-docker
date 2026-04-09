#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Compares the working tree to a remote branch and shows which files have changed.

.PARAMETER RemoteBranch
    The remote branch to compare against (e.g., 'origin/main'). Defaults to the upstream tracking branch.

.PARAMETER DiffFilter
    Optional diff filter: A=Added, D=Deleted, M=Modified, R=Renamed, etc.

.EXAMPLE
    ./eng/compare-branch.ps1
    ./eng/compare-branch.ps1 -RemoteBranch origin/main
    ./eng/compare-branch.ps1 -RemoteBranch origin/release/9.0 -DiffFilter M
#>

param(
    [string]$RemoteBranch,
    [string]$DiffFilter
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

$mergeBase = git merge-base HEAD $RemoteBranch
$diffArgs = @('diff', '--name-status', $mergeBase)
if ($DiffFilter) {
    $diffArgs += "--diff-filter=$DiffFilter"
}

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
Write-Host "To see individual diffs, run: git --no-pager diff $RemoteBranch -- <file>"
