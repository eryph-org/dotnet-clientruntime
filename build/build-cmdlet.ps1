#Requires -Version 7.4
[CmdletBinding()]
param (
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
$cmdletName = "Eryph.ClientRuntime.Configuration"

if ($OutputDir -eq ".") {
    $OutputDir = Resolve-Path (Join-path $PSScriptRoot "..")
}

$rootDir = Resolve-Path (Join-Path $PSScriptRoot "..")
$cmdletPath = Join-Path $OutputDir "cmdlet"

if (Test-Path $cmdletPath ) {
    Remove-Item $cmdletPath -Force -Recurse -ErrorAction Stop
}
$null = New-Item -ItemType Directory $cmdletPath

$buildOutputPath = Join-Path $rootDir "src" "$cmdletName.Commands" "cmdlet"
Copy-Item $buildOutputPath\* $cmdletPath -Recurse
