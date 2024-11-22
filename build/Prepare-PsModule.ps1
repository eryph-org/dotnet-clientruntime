#Requires -Version 7.4
<#
    .SYNOPSIS
        Prepares the Powershell module.
    .DESCRIPTION
        This script prepares the Powershell module for distribution.
        It is intended to be called by MSBuild during the normal build
        process. The script prepares the module metadata (e.g. the manifest).
#>
[CmdletBinding()]
param(
    [Parameter()]
    [string]
    [ValidateScript({ $_ -match '[a-zA-Z\.]+' }, ErrorMessage = "The module name '{0}' is invalid.")]
    $ModuleName,
    [Parameter()]
    [string]
    [ValidateScript({ Test-Path $_ -IsValid }, ErrorMessage = "The path '{0}' is invalid.")]
    $OutputDirectory,
    [Parameter()]
    [string]
    [ValidateScript({ $_ -match '\d+\.\d+\.\d+' }, ErrorMessage = "The version '{0}' is invalid.")]
    $MajorMinorPatch,
    [Parameter()]
    [string]
    $NuGetPreReleaseTag
)

$PSNativeCommandUseErrorActionPreference = $true
$ErrorActionPreference = 'Stop'

$modulePath = Join-Path $OutputDirectory "PsModule" $ModuleName

# Prepare the output directory
if (Test-Path $modulePath) {
    Remove-Item -Path $modulePath -Force -Recurse
}

$null = New-Item -ItemType Directory -Path $modulePath

$isPrerelease = -not [string]::IsNullOrWhiteSpace($NuGetPreReleaseTag)

# Prepare the module manifest
$config = Get-Content (Join-Path $PSScriptRoot "$ModuleName.psd1") -Raw
$config = $config.Replace("ModuleVersion = '0.1'", "ModuleVersion = '$MajorMinorPatch'");
if ($isPrerelease) {
    $config = $config.Replace("# Prerelease = ''", "Prerelease = '$NuGetPreReleaseTag'");
}
Set-Content -Path (Join-Path $modulePath "$ModuleName.psd1") -Value $config
Copy-Item -Path (Join-Path $PSScriptRoot "$ModuleName.psm1") -Destination $modulePath
