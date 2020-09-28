Push-Location $PSScriptRoot

Get-ChildItem -Path *.psm1 | Foreach-Object{ .\SignScript.ps1 $_.FullName }
