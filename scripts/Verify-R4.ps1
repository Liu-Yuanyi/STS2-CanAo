$ErrorActionPreference = "Stop"

Write-Warning "Verify-R4.ps1 is obsolete in the R5 workspace. Running Verify-R5.ps1 instead."
& (Join-Path $PSScriptRoot "Verify-R5.ps1")
