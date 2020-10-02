param ($Configuration = "Debug", $OutputDir = ".")

$excludedFiles = @("System.Management.Automation.dll", "JetBrains.Annotations.dll")

# If this script is not running on a build server, remind user to 
# set environment variables so that this script can be debugged
if(-not ($Env:GITVERSION_MajorMinorPatch))
{
    Write-Error "You must set the following environment variables"
    Write-Error "to test this script interactively (values are examples)"
    Write-Host '$Env:GITVERSION_MajorMinorPatch = "1.0.0"'
    Write-Host '$Env:NuGetPreReleaseTag = "ci0030"'
    exit 1
}


Push-Location $PSScriptRoot
cd ..
$rootDir = Get-Location

Push-Location $OutputDir

if(Test-Path cmdlet ) {
    rm cmdlet -Force -Recurse  -ErrorAction Stop
}

mkdir cmdlet | Out-Null
cd cmdlet
mkdir Haipa.ClientRuntime.Configuration | Out-Null
cd Haipa.ClientRuntime.Configuration

mkdir coreclr | Out-Null
mkdir desktop | Out-Null

cp $rootDir\build\Haipa.ClientRuntime.Configuration* .
cp $rootDir\src\Haipa.ClientRuntime.Configuration.Commands\bin\${Configuration}\netcoreapp3.0\* coreclr -Exclude $excludedFiles -Recurse
cp $rootDir\src\Haipa.ClientRuntime.Configuration.Commands\bin\${Configuration}\net472\* desktop  -Exclude $excludedFiles  -Recurse

$config = gc Haipa.ClientRuntime.Configuration.psd1 -Raw
$config = $config.Replace("ModuleVersion = '0.1'", "ModuleVersion = '${Env:GITVERSION_MajorMinorPatch}'");

if(-not [string]::IsNullOrWhiteSpace($Env:GITVERSION_PreReleaseTag)) {
    $config = $config.Replace("# Prerelease = ''", "Prerelease = '-${Env:NuGetPreReleaseTag}'");
}

$config | sc Haipa.ClientRuntime.Configuration.psd1

Pop-Location