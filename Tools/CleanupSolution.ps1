$TwizzarCodeRoot = Join-Path $PSScriptRoot "\..\Source\" -Resolve

Write-Host "Remove all obj folders"
Get-ChildItem -Filter "obj" -Path $TwizzarCodeRoot -Recurse | Remove-Item -Recurse -Force

Write-Host "Remove all bin folders"
Get-ChildItem -Filter "bin" -Path $TwizzarCodeRoot -Recurse | Remove-Item -Recurse -Force

Write-Host "Remove all lock.json files"
Get-ChildItem -Filter "*.lock.json" -Path $TwizzarCodeRoot -Recurse | Remove-Item -Recurse -Force