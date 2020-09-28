param ($Configuration = "Debug", $OutputDir = ".")

$excludedFiles = @("System.Management.Automation.dll", "JetBrains.Annotations.dll")

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

cp ..\..\build\Haipa.ClientRuntime.Configuration* .
cp ..\..\src\Haipa.ClientRuntime.Configuration.Commands\bin\${Configuration}\netcoreapp3.0\* coreclr -Exclude $excludedFiles -Recurse
cp ..\..\src\Haipa.ClientRuntime.Configuration.Commands\bin\${Configuration}\net472\* desktop  -Exclude $excludedFiles  -Recurse

Pop-Location