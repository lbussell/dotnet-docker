$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 2.0

<#
    .SYNOPSIS
        Gets the ImageBuilder tag and makes sure the image is present locally.
#>
function Get-ImageBuilder() {
    $imageNameVars = & $PSScriptRoot/Get-ImageNameVars.ps1

    $imageBuilderName = $imageNameVars["imageNames.imageBuilderName"]

    Pull-Image $imageBuilderName
    Write-Host "Using ImageBuilder tag: $imageBuilderName"
    return $imageBuilderName
}

<#
    .SYNOPSIS
        Pulls the specified docker image if it is not already present locally.
#>
function Pull-Image([string] $imageTag) {
    & docker inspect ${imageTag} | Out-Null
    if (-not $?) {
        Write-Output "An image tagged '${imageTag}' was not found locally."
        Write-Output "Pulling ${imageTag}..."
        & $PSScriptRoot/Invoke-WithRetry.ps1 "docker pull ${imageTag}"
    }
}
