#Requires -Version 7.4
<#
    .SYNOPSIS
        Populates the Powershell module.
    .DESCRIPTION
        This script populates the Powershell module with the assemblies
        from the build output. It is intended to be called by MSBuild
        during the normal build process. The script will be called once
        for each target framework!
#>
[CmdletBinding()]
param(
    [Parameter()]
    [string]
    [ValidateScript({ $_ -match '[a-zA-Z\.]+' }, ErrorMessage = "The module name '{0}' is invalid.")]
    $ModuleName,
    [Parameter()]
    [string]
    [ValidateScript({ Test-Path $_ }, ErrorMessage = "The path '{0}' is invalid.")]
    $OutputDirectory,
    [Parameter()]
    [string]
    [ValidateScript({ Test-Path $_ }, ErrorMessage = "The path '{0}' is invalid.")]
    $TargetPath,
    [Parameter()]
    [string]
    [ValidateScript({ $_ -match 'net\d+\.?\d+' }, ErrorMessage = "The target framework '{0}' is invalid.")]
    $TargetFramework
)

$PSNativeCommandUseErrorActionPreference = $true
$ErrorActionPreference = 'Stop'

$excludedFiles = @("System.Management.Automation.dll", "JetBrains.Annotations.dll")

$modulePath = Join-Path $OutputDirectory "PsModule" $ModuleName
$isWindowsPowershell = $TargetFramework -like 'net4*'
$moduleAssemblyPath = Join-Path $modulePath ($isWindowsPowershell  ? 'desktop' : 'coreclr')

# Copy the assemblies from the build output
if (Test-Path $moduleAssemblyPath) {
    Remove-Item -Path $moduleAssemblyPath -Force -Recurse
}
$null = New-Item -ItemType Directory -Path $moduleAssemblyPath
$targetDirectory = (Get-Item $TargetPath).Directory.FullName
Copy-Item -Path (Join-Path $targetDirectory "*") -Destination $moduleAssemblyPath -Exclude $excludedFiles -Recurse
