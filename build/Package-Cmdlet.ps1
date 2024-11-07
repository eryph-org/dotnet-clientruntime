#Requires -Version 7.4
<#
    .SYNOPSIS
        Package the the Powershell module.
    .DESCRIPTION
        This script packages the Powershell module for distribution. It is intended
        to be called by MSBuild during the normal build process. The script will
        be called once for each target framework.
#>
[CmdletBinding()]
param(
    [Parameter()]
    [string]
    $TargetPath,
    [Parameter()]
    [string]
    $TargetFramework,
    [Parameter()]
    [string]
    $OutputDirectory,
    [Parameter()]
    [string]
    $MajorMinorPatch,
    [Parameter()]
    [string]
    $NuGetPreReleaseTag,
    [Parameter()]
    [switch]
    $Clean
)

$ErrorActionPreference = 'Stop'

$cmdletName = "Eryph.ClientRuntime.Configuration"
$excludedFiles = @("System.Management.Automation.dll", "JetBrains.Annotations.dll")

if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    throw "The OutputDirectory parameter is missing."
}

if ($Clean) {
    return
}

$cmdletPath = Join-Path $OutputDirectory "cmdlet" $cmdletName
$isWindowsPowershell = $TargetFramework -like 'net4*'
$cmdletAssemblyPath = Join-Path $cmdletPath ($isWindowsPowershell  ? 'desktop' : 'coreclr')
# if ($Clean) {
#     if (Test-Path $cmdletAssemblyPath) {
#         Remove-Item -Path $cmdletAssemblyPath -Force -Recurse
#     }
#     return
# }

if ([string]::IsNullOrWhiteSpace($TargetFramework)) {
    throw "The TargetFramework parameter is missing."
}


if ([string]::IsNullOrWhiteSpace($TargetPath)) {
    throw "The TargetPath parameter is missing."
}

if ([string]::IsNullOrWhiteSpace($MajorMinorPatch)) {
    throw "The MajorMinorPatch parameter is missing."
}

# Prepare the output directory
$cmdletPath = Join-Path $OutputDirectory "cmdlet" $cmdletName
if (-not (Test-Path $cmdletPath)) {
    $null = New-Item -ItemType Directory -Path $cmdletPath
}

$targetDirectory = (Get-Item $TargetPath).Directory.FullName
Write-Output "Target Directory: $targetDirectory"

# Copy the build output
if (Test-Path $cmdletAssemblyPath) {
    Remove-Item -Path $cmdletAssemblyPath -Force -Recurse
}
$null = New-Item -ItemType Directory -Path $cmdletAssemblyPath
Copy-Item -Path (Join-Path $targetDirectory "*") -Destination $cmdletAssemblyPath -Exclude $excludedFiles -Recurse

# Prepare the module manifest
$config = Get-Content (Join-Path $PSScriptRoot "$cmdletName.psd1") -Raw
$config = $config.Replace("ModuleVersion = '0.1'", "ModuleVersion = '$MajorMinorPatch'");
if (-not [string]::IsNullOrWhiteSpace($NuGetPreReleaseTag)) {
    $config = $config.Replace("# Prerelease = ''", "Prerelease = '$NuGetPreReleaseTag'");
}
Set-Content -Path (Join-Path $cmdletPath "$cmdletName.psd1") -Value $config
Copy-Item -Path (Join-Path $PSScriptRoot "$cmdletName.psm1") -Destination $cmdletPath

# Verify that all Cmdlets are exposed in the manifest
$powershell = $isWindowsPowershell ? 'powershell.exe' : 'pwsh.exe'
$moduleCmdlets = (& $powershell -Command "[array](Import-Module -Scope Local $cmdletPath -PassThru).ExportedCmdlets.Keys -join ','") -split ','
$assemblyCmdlets = (& $powershell -Command "[array](Import-Module -Scope Local $TargetPath -PassThru).ExportedCmdlets.Keys -join ','") -split ','
Write-Output "Module Cmdlets: $moduleCmdlets"
Write-Output "Assembly Cmdlets: $assemblyCmdlets"
$missingCmdlets = [Linq.Enumerable]::Except($assemblyCmdlets, $moduleCmdlets)
if ($missingCmdlets.Count -gt 0) {
    throw "The following Cmdlets are not exposed in the module manifest: $($missingCmdlets -join ', ')"
}
