#Requires -Version 7.4
<#
    .SYNOPSIS
        Prepare the Powershell module for publication.
    .DESCRIPTION
        This script moves the already built Powershell module to a location
        where the release pipeline will pick it up for publication.
        The name and output location of this script cannot be changed as
        it would break the release pipeline.
#>
[CmdletBinding()]
param (
    [Parameter()]
    [string]
    [ValidateNotNullOrEmpty()]
    $Configuration,
    [Parameter()]
    [string]
    [ValidateScript({ Test-Path $_ }, ErrorMessage = "The path '{0}' is invalid.")]
    $ArtifactDirectory
)

$ErrorActionPreference = 'Stop'
$moduleName = "Eryph.ClientRuntime.Configuration"

$repositoryPath = Resolve-Path (Join-Path $PSScriptRoot "..")
$targetPath = Join-Path $ArtifactDirectory "cmdlet"

if (Test-Path $targetPath ) {
    Remove-Item $targetPath -Force -Recurse
}
$null = New-Item -ItemType Directory $targetPath

$modulePath = Join-Path $repositoryPath "src" "$moduleName.Commands" "bin" $Configuration "PsModule" 
Copy-Item $modulePath\* $targetPath -Recurse
