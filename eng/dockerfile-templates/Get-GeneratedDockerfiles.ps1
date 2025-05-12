#!/usr/bin/env pwsh
param(
    [switch]$Validate,
    [string]$Branch,
    [string]$OutputDirectory,
    [string]$CustomImageBuilderArgs
)

Import-Module -force $PSScriptRoot/../DependencyManagement.psm1

function Invoke-GenerateDockerfiles([string] $imageBuilderTag, [string] $SourceBranch) {
    $command = "docker run --rm -v /var/run/docker.sock:/var/run/docker.sock -v ${PWD}:/repo -w /repo ${imageBuilderTag} generateDockerfiles --no-version-logging --var branch=$SourceBranch"
    Write-Host "Executing: $command"
    Invoke-Expression $command
}

# Move to repo root, change this if the script moves.
pushd "$PSScriptRoot/../../"

if (!$Branch) {
    $Branch = Get-Branch
}

$imageBuilderTag = Get-ImageBuilder
Write-Host "ImageBuilder: $imageBuilderTag"
Invoke-GenerateDockerfiles $imageBuilderTag $Branch
