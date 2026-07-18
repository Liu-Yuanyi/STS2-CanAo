param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

$Root = Split-Path $PSScriptRoot -Parent
$Project = Join-Path $Root "src\CanAoNative\CanAoNative.csproj"
$Manifest = Join-Path $Root "packaging\CanAoNative.json"
$GameDir = $env:STS2_GAME_DIR

if ([string]::IsNullOrWhiteSpace($GameDir)) {
    $GameDir = "E:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2"
}

if (-not (Test-Path $GameDir)) {
    throw ("Game directory does not exist: {0}" -f $GameDir)
}

& (Join-Path $PSScriptRoot "Verify-NoBaseLib.ps1")
& (Join-Path $PSScriptRoot "Verify-R8.ps1")

Get-Process SlayTheSpire2, Godot -ErrorAction SilentlyContinue |
    Stop-Process -Force

dotnet clean $Project -c $Configuration
dotnet build $Project -c $Configuration "-p:GameDir=$GameDir"

if ($LASTEXITCODE -ne 0) {
    throw "Build failed."
}

$TargetFramework = "net9.0"
$BuiltDir = Join-Path $Root (
    "src\CanAoNative\bin\{0}\{1}" -f
    $Configuration,
    $TargetFramework
)
$BuiltDll = Join-Path $BuiltDir "CanAoNative.dll"
$BuiltPdb = Join-Path $BuiltDir "CanAoNative.pdb"

if (-not (Test-Path $BuiltDll)) {
    throw ("Built DLL not found: {0}" -f $BuiltDll)
}

$StageDir = Join-Path $Root "build\mods\CanAoNative"
$InstallDir = Join-Path $GameDir "mods\CanAoNative"

Remove-Item $StageDir -Recurse -Force -ErrorAction SilentlyContinue
New-Item $StageDir -ItemType Directory -Force | Out-Null

Copy-Item $BuiltDll (Join-Path $StageDir "CanAoNative.dll") -Force
Copy-Item $Manifest (Join-Path $StageDir "CanAoNative.json") -Force

if (Test-Path $BuiltPdb) {
    Copy-Item $BuiltPdb (Join-Path $StageDir "CanAoNative.pdb") -Force
}

$PckTool = Join-Path $PSScriptRoot "godotpcktool.exe"
$BuiltPck = Join-Path $StageDir "CanAoNative.pck"

if (-not (Test-Path $PckTool)) {
    throw ("PCK tool not found: {0}" -f $PckTool)
}

Push-Location $Root
try {
    & $PckTool `
        --pack $BuiltPck `
        --action add `
        --file "godot/CanAoNative" `
        --remove-prefix "godot/" `
        --set-godot-version 4.5.0 `
        --quieter
}
finally {
    Pop-Location
}

if ($LASTEXITCODE -ne 0 -or -not (Test-Path $BuiltPck)) {
    throw "PCK packing failed."
}

Remove-Item $InstallDir -Recurse -Force -ErrorAction SilentlyContinue
New-Item $InstallDir -ItemType Directory -Force | Out-Null
Copy-Item (Join-Path $StageDir "*") $InstallDir -Recurse -Force

$StagedDll = Join-Path $StageDir "CanAoNative.dll"
$InstalledDll = Join-Path $InstallDir "CanAoNative.dll"

$BuiltHash = (Get-FileHash $BuiltDll -Algorithm SHA256).Hash
$StagedHash = (Get-FileHash $StagedDll -Algorithm SHA256).Hash
$InstalledHash = (Get-FileHash $InstalledDll -Algorithm SHA256).Hash

Write-Host ("Built DLL:       {0}" -f $BuiltDll)
Write-Host ("Staged DLL:      {0}" -f $StagedDll)
Write-Host ("Installed DLL:   {0}" -f $InstalledDll)
Write-Host ("Built SHA256:    {0}" -f $BuiltHash)
Write-Host ("Staged SHA256:   {0}" -f $StagedHash)
Write-Host ("Installed SHA256:{0}" -f $InstalledHash)

if ($BuiltHash -ne $StagedHash -or $BuiltHash -ne $InstalledHash) {
    throw "Built, staged and installed DLL hashes do not match."
}

$Unexpected = Get-ChildItem $InstallDir -File |
    Where-Object {
        $_.Name -in @(
            "sts2.dll",
            "0Harmony.dll",
            "GodotSharp.dll",
            "Alchyr.Sts2.BaseLib.dll"
        )
    }

if ($Unexpected) {
    $Unexpected | Format-Table FullName
    throw "Unexpected runtime assemblies were installed beside the mod."
}

Write-Host ""
Write-Host "Deployment verified."
Write-Host "Expected runtime log marker:"
Write-Host "  CANAO_NATIVE_R8_EDICT_SYSTEM_20260717"
Write-Host ""
Get-ChildItem $InstallDir |
    Select-Object Name, Length, LastWriteTime
