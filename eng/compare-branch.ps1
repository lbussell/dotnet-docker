#!/usr/bin/env pwsh

param(
    [string]$RemoteBranch,
    [string]$File,
    [Alias('h')]
    [switch]$Help
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if ($Help) {
    $scriptName = $MyInvocation.MyCommand.Name
    Write-Host "Compare the working tree to a remote branch."
    Write-Host ""
    Write-Host "Usage: $scriptName <branch> [<file>] [-Help]"
    Write-Host ""
    Write-Host "Arguments:"
    Write-Host "  <branch>  Remote branch to compare against (required)"
    Write-Host "  <file>    Show the full diff for a specific file"
    Write-Host ""
    Write-Host "Examples:"
    Write-Host "  $scriptName upstream/main                        List changed files vs upstream/main"
    Write-Host "  $scriptName upstream/nightly eng/some-file.ps1   Show diff for a specific file"
    exit 0
}

if (-not $RemoteBranch) {
    Write-Error "No remote branch specified. Usage: $($MyInvocation.MyCommand.Name) <branch> [<file>]"
    exit 1
}

# Verify the remote branch exists
$null = git rev-parse --verify $RemoteBranch 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Error "Remote branch '$RemoteBranch' not found. Run 'git fetch' and try again."
    exit 1
}

# Show the full diff for a single file
if ($File) {
    git --no-pager diff --src-prefix "${RemoteBranch}:" --dst-prefix 'working-tree:' $RemoteBranch -- $File
    exit $LASTEXITCODE
}

# Find the common ancestor and list changed files between it and the working tree
$mergeBase = git merge-base HEAD $RemoteBranch
$diffArgs = @('diff', '--name-status', $mergeBase)

$changes = @(git @diffArgs)

if (-not $changes) {
    Write-Host "No differences between working tree and $RemoteBranch."
    exit 0
}

# Print the changed file list with a summary
Write-Host "Changed files (working tree vs $RemoteBranch):"
Write-Host ""

foreach ($line in $changes) {
    Write-Host $line
}

Write-Host ""
Write-Host "Total: $($changes.Count) file(s) changed."
Write-Host "To see individual diffs, run: $($MyInvocation.MyCommand.Name) -RemoteBranch $RemoteBranch -File <file>"
