param(
    [string]$GameDir = $env:STS2_GAME_DIR,
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",
    [string]$LogPath
)

$ErrorActionPreference = "Stop"

$Root = Split-Path $PSScriptRoot -Parent

if ([string]::IsNullOrWhiteSpace($GameDir)) {
    $GameDir = "E:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2"
}

$BuiltDll = Join-Path $Root (
    "src\CanAoNative\bin\{0}\net9.0\CanAoNative.dll" -f
    $Configuration
)
$StagedDll = Join-Path $Root "build\mods\CanAoNative\CanAoNative.dll"
$InstalledDll = Join-Path $GameDir "mods\CanAoNative\CanAoNative.dll"

$Items = @(
    @{ Label = "Built"; Path = $BuiltDll },
    @{ Label = "Staged"; Path = $StagedDll },
    @{ Label = "Installed"; Path = $InstalledDll }
)

$Rows = foreach ($Item in $Items) {
    if (-not (Test-Path $Item.Path)) {
        throw ("{0} DLL missing: {1}" -f $Item.Label, $Item.Path)
    }

    $File = Get-Item $Item.Path
    $Hash = (Get-FileHash $Item.Path -Algorithm SHA256).Hash

    [PSCustomObject]@{
        Label = $Item.Label
        Path = $File.FullName
        Length = $File.Length
        LastWriteTime = $File.LastWriteTime
        SHA256 = $Hash
    }
}

$Rows | Format-Table -AutoSize

$UniqueHashes = $Rows.SHA256 | Sort-Object -Unique
if ($UniqueHashes.Count -ne 1) {
    throw "Built, staged and installed DLL hashes differ."
}

if (-not [string]::IsNullOrWhiteSpace($LogPath)) {
    if (-not (Test-Path $LogPath)) {
        throw ("Log file not found: {0}" -f $LogPath)
    }

    $RequiredMarker = "CANAO_NATIVE_R6_STARMOON_EVENTS_20260717"
    $LogText = Get-Content $LogPath -Raw -Encoding UTF8

    if (-not $LogText.Contains($RequiredMarker)) {
        throw ("Runtime log does not contain build marker: {0}" -f $RequiredMarker)
    }

    $ForbiddenLogPatterns = @(
        "NullReferenceException",
        "STARMOON_FAILED",
        "YUHUO_RESOLVE_FAILED",
        "YUHUO_FALLBACK_EXHAUST_FAILED",
        "Exception thrown when calling mod initializer"
    )

    foreach ($Pattern in $ForbiddenLogPatterns) {
        if ($LogText.Contains($Pattern)) {
            throw ("Runtime log contains failure marker: {0}" -f $Pattern)
        }
    }

    Write-Host "Runtime log marker and failure scan passed."
}

Write-Host "Deployment verification passed."
