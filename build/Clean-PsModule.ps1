#Requires -Version 7.4
<#
    .SYNOPSIS
        Remove the Powershell module from the build output.
    .DESCRIPTION
        This script removes the Powershell module from the build output.
        It is intended to be called by MSBuild during the normal clean process.
        The script might be called once for each target framework.
#>
[CmdletBinding()]
param(
    [Parameter()]
    [string]
    [ValidateNotNullOrEmpty()]
    $OutputDirectory
)

$PSNativeCommandUseErrorActionPreference = $true
$ErrorActionPreference = 'Stop'

$modulePath = Join-Path $OutputDirectory "PsModule"

if (Test-Path $modulePath) {
    # This script might be called in parallel for each target framework.
    # Hence, we ignore errors as files might have been removed by another instance.
    Remove-Item -Path $modulePath -Force -Recurse -ErrorAction SilentlyContinue
}
