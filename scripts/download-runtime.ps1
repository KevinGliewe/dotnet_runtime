
param($Architecture="Local", $Platform="Local")

# Beckup the current directory
$BackupPath = Get-Location

# Checnge directory to the repo root
Set-Location $PSScriptRoot/../

Write-Output "Using repo root : '$(Get-Location)'"

# Load config
$RuntimeSettings = Get-Content -Path ./.config/runtime.json | ConvertFrom-Json

Write-Output "Version: '$($RuntimeSettings.version)'"

# Restore the tools (runtimedl)
dotnet tool restore

Write-Output "Downloading runtime"
dotnet runtimedl --version-pattern "$($RuntimeSettings.version)" --output "bin" --platform $Platform --architecture $Architecture # --download false


# Restore the current directory
Set-Location $BackupPath