param ($Configuration = "Debug", $OutputDir = ".")

$cmdletName = "Eryph.ClientRuntime.Configuration"

# $Env:GITVERSION_MajorMinorPatch = "0.7.1"
# $Env:GITVERSION_NuGetPreReleaseTag = "ci0030"

# If this script is not running on a build server, remind user to 
# set environment variables so that this script can be debugged
if(-not ($Env:GITVERSION_MajorMinorPatch))
{
    Write-Error "You must set the following environment variables"
    Write-Error "to test this script interactively (values are examples)"
    Write-Host '$Env:GITVERSION_MajorMinorPatch = "1.0.0"'
    Write-Host '$Env:GITVERSION_NuGetPreReleaseTag = "ci0030"'
    exit 1
}

if ($OutputDir -eq ".") {
    $OutputDir = Resolve-Path (Join-path $PSScriptRoot "..")
}

$rootDir = Resolve-Path (Join-Path $PSScriptRoot "..")
$cmdletPath = Join-Path $OutputDir cmdlet

if (Test-Path cmdletPath ) {
    Remove-Item cmdletPath -Force -Recurse -ErrorAction Stop
}
$null = New-Item -ItemType Directory $cmdletPath

$buildOutputPath = Join-Path $rootDir src "$cmdletName.Commands" cmdlet
Copy-Item $buildOutputPath\* $cmdletPath -Recurse
