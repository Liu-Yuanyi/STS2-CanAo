param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

$Root = Split-Path $PSScriptRoot -Parent
$Project = Join-Path $Root "src\CanAoNative\CanAoNative.csproj"

# Determine GameDir
$GameDir = $env:STS2_GAME_DIR
if ([string]::IsNullOrWhiteSpace($GameDir)) {
    $GameDir = "E:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2"
}

if (-not (Test-Path $GameDir)) {
    throw ("Game directory does not exist: {0}" -f $GameDir)
}

Write-Host "GameDir: $GameDir"

dotnet clean $Project -c $Configuration
dotnet build $Project -c $Configuration "-p:GameDir=$GameDir"

if ($LASTEXITCODE -ne 0) {
    throw "Build failed."
}

$TargetFramework = "net9.0"
$BuiltDll = Join-Path $Root (
    "src\CanAoNative\bin\{0}\{1}\CanAoNative.dll" -f
    $Configuration,
    $TargetFramework
)

if (-not (Test-Path $BuiltDll)) {
    throw ("Built DLL not found: {0}" -f $BuiltDll)
}

$BuiltHash = (Get-FileHash $BuiltDll -Algorithm SHA256).Hash
Write-Host "Build succeeded."
Write-Host "Built DLL: $BuiltDll"
Write-Host "SHA256: $BuiltHash"
