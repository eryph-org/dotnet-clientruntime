#Requires -Version 7.4
[CmdletBinding()]
param (
    [Parameter()]
    [string]
    [ValidateNotNullOrEmpty()]
    $ModuleName = "Eryph.ClientRuntime.Configuration",
    [Parameter()]
    [string]
    [ValidateNotNullOrEmpty()]
    $Configuration = "Debug",
    [Parameter()]
    [string]
    [ValidateNotNullOrEmpty()]
    $OutputDir = "."
)

$ErrorActionPreference = 'Stop'

if ($OutputDir -eq ".") {
    $OutputDir = Resolve-Path (Join-path $PSScriptRoot "..")
}

$rootDir = Resolve-Path (Join-Path $PSScriptRoot "..")
$modulePath = Join-Path $OutputDir "cmdlet"

if (Test-Path $modulePath ) {
    Remove-Item $modulePath -Force -Recurse -ErrorAction Stop
}
$null = New-Item -ItemType Directory $modulePath

$buildOutputPath = Join-Path $rootDir "src" "$ModuleName.Commands" "bin" "Module"
Copy-Item $buildOutputPath\* $modulePath -Recurse
