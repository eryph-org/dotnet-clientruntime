#Requires -Version 7.4
<#
    .SYNOPSIS
        Test the Powershell module.
    .DESCRIPTION
        This script tests the fully packaged Powershell module.
        It is intended to be called by MSBuild during the normal build
        process.
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
    $OutputDirectory
)

$PSNativeCommandUseErrorActionPreference = $true
$ErrorActionPreference = 'Stop'

$modulePath = Join-Path $OutputDirectory "PsModule" $ModuleName

# Verify that all Cmdlets are exposed in the manifest. We must load the modules
# in separate Powershell processes to avoid conflicts.
$moduleCmdlets = (powershell.exe -Command "[array](Import-Module -Scope Local $modulePath -PassThru).ExportedCmdlets.Keys -join ','") -split ','
$assemblyCmdlets = (powershell.exe -Command "[array](Import-Module -Scope Local $(Join-Path $modulePath "desktop" "$ModuleName.Commands.dll") -PassThru).ExportedCmdlets.Keys -join ','") -split ','
$missingCmdlets = [Linq.Enumerable]::Except($assemblyCmdlets, $moduleCmdlets)
if ($missingCmdlets.Count -gt 0) {
    throw "The following Cmdlets are not exposed in the module manifest when checking with Windows Powershell: $($missingCmdlets -join ', ')"
}

$moduleCmdlets = (pwsh.exe -Command "[array](Import-Module -Scope Local $modulePath -PassThru).ExportedCmdlets.Keys -join ','") -split ','
$assemblyCmdlets = (pwsh.exe -Command "[array](Import-Module -Scope Local $(Join-Path $modulePath "coreclr" "$ModuleName.Commands.dll") -PassThru).ExportedCmdlets.Keys -join ','") -split ','
$missingCmdlets = [Linq.Enumerable]::Except($assemblyCmdlets, $moduleCmdlets)
if ($missingCmdlets.Count -gt 0) {
    throw "The following Cmdlets are not exposed in the module manifest when checking with Powershell 7: $($missingCmdlets -join ', ')"
}
