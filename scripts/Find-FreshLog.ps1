$ErrorActionPreference = "Stop"

$LogRoot = Join-Path $env:APPDATA "SlayTheSpire2"

$FreshLog = Get-ChildItem $LogRoot `
    -Recurse `
    -File `
    -Filter "godot*.log" `
    -ErrorAction SilentlyContinue |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1

if (-not $FreshLog) {
    throw "No log files found."
}

$FreshLog | Format-List FullName, Length, CreationTime, LastWriteTime

Write-Host ""
Write-Host "=== CanAoNative R5 markers and failures ==="
Select-String `
    -Path $FreshLog.FullName `
    -Pattern "CANAO_NATIVE_R5_VERIFIED_R4_BASE_20260717",
             "CanAoNative",
             "YUHUO_RESOLVE_FAILED",
             "YUHUO_FALLBACK_EXHAUST_FAILED",
             "NullReferenceException",
             "Exception thrown when calling mod initializer",
             "Fatal",
             "\[ERROR\]" `
    -Context 2,6
