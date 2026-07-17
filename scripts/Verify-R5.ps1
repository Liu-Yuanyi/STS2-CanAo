$ErrorActionPreference = "Stop"

$Root = Split-Path $PSScriptRoot -Parent
$SourceRoot = Join-Path $Root "src\CanAoNative"
$ManifestPath = Join-Path $Root "packaging\CanAoNative.json"
$CardsLocZh = Join-Path $Root "godot\CanAoNative\localization\zhs\cards.json"
$CardsLocEn = Join-Path $Root "godot\CanAoNative\localization\eng\cards.json"
$PowersLocZh = Join-Path $Root "godot\CanAoNative\localization\zhs\powers.json"
$PowersLocEn = Join-Path $Root "godot\CanAoNative\localization\eng\powers.json"

$StrictUtf8 = [System.Text.UTF8Encoding]::new($false, $true)
$Utf8NoBom = [System.Text.UTF8Encoding]::new($false)

function Read-Utf8Strict([string]$Path) {
    if (-not (Test-Path $Path)) {
        throw ("Required file missing: {0}" -f $Path)
    }

    try {
        return [System.IO.File]::ReadAllText($Path, $StrictUtf8)
    }
    catch {
        throw ("File is not valid UTF-8: {0}`n{1}" -f $Path, $_)
    }
}

function Get-NormalizedTextSha256([string]$Path) {
    $Text = Read-Utf8Strict $Path
    $Normalized = $Text.Replace("`r`n", "`n").Replace("`r", "`n")
    $Bytes = $Utf8NoBom.GetBytes($Normalized)
    $Sha = [System.Security.Cryptography.SHA256]::Create()

    try {
        $HashText = [BitConverter]::ToString($Sha.ComputeHash($Bytes))
        return $HashText.Replace("-", "").ToLowerInvariant()
    }
    finally {
        $Sha.Dispose()
    }
}

$RequiredFiles = @(
    "Cards\ShiWeiCard.cs",
    "Cards\ZanBiFengMangCard.cs",
    "Powers\TemporaryFengWeiPower.cs",
    "Rules\FengWei\FengWeiService.cs",
    "Patches\CanAoPowerIconPatch.cs"
)

foreach ($RelativePath in $RequiredFiles) {
    $Path = Join-Path $SourceRoot $RelativePath
    if (-not (Test-Path $Path)) {
        throw ("R5 source file missing: {0}" -f $Path)
    }
}

# These gameplay-critical files must remain text-identical to the uploaded,
# user-validated R4 baseline. Newline style is normalized before hashing.
$VerifiedR4Hashes = [ordered]@{
    "Cards\FeatherRanksCard.cs" = "b85c4b790e6b4eb0dddb38a88e94b15c7bdb3568749cef003a65bbdd6d039141"
    "Cards\SacrificialPreparationCard.cs" = "d4b3ed638d6e3bfc9248323cf3afcdf5501d7e40623ed5c700dba1e79473dfee"
    "Cards\StarMoonStrike.cs" = "b38ef7bbc64acac8906377d59836c90acc0b3240d931f5725d0a3c4a19957dcd"
    "Cards\YuHuoBannerCard.cs" = "9892d7f9bc9ca5791b8774715575325aaba35a0c1efe0b3b79b7df5d9682cd4a"
    "Powers\FengWeiPower.cs" = "1d3df1fdfd1a7f272ca3c9e7fe44a38c0b7b6ebde470fda8b07010c542eb716b"
    "Powers\YuHuoBannerPower.cs" = "2f33f4294354c90ae8941ec5208597d4d5f54562d76637ca04ff0226d99ad50a"
    "Powers\YuHuoBannerTemporaryStrengthPower.cs" = "66e32ad32cc12d7fc4bc415e2b19a2b6c65fe1e62809fd45f1538747d3054d27"
    "Rules\CanAoCombatRules.cs" = "81a632d2e0724613cf77c297deafbd25c329935b49083a2175472318d0b69578"
    "Rules\YuHuo\YuHuoCombatState.cs" = "3f12ec54d3d35da3c94d5427653dca446b439d389ec877c3fadc1dce66d123c1"
    "Rules\YuHuo\YuHuoResolver.cs" = "2e63f9cbb43fcd449d63d70d6f922c4fa91f951aa12c837cc69d436470c1468e"
    "Rules\YuHuo\YuHuoService.cs" = "bca86dcb0ed4a5caa7c0c9945eac654e90278da406ae4cb4c4c94be3f38edbe3"
    "Patches\YuHuoExhaustPatch.cs" = "82441b3866502fc8087dfeec059f15eb3ab36b112911a2240a2a457a648e292b"
}

foreach ($Entry in $VerifiedR4Hashes.GetEnumerator()) {
    $Path = Join-Path $SourceRoot $Entry.Key
    $Actual = Get-NormalizedTextSha256 $Path

    if ($Actual -ne $Entry.Value) {
        throw (
            "Verified R4 source changed unexpectedly: {0}`nExpected: {1}`nActual:   {2}" -f
            $Entry.Key,
            $Entry.Value,
            $Actual
        )
    }
}

$SourceFiles = Get-ChildItem $SourceRoot -Recurse -Filter "*.cs"
$SourceText = ($SourceFiles | ForEach-Object {
    Read-Utf8Strict $_.FullName
}) -join "`n"

$RequiredMarkers = @(
    "CANAO_NATIVE_R5_VERIFIED_R4_BASE_20260717",
    "class TemporaryFengWeiPower",
    "class FengWeiService",
    "GetEffectiveAmount",
    "GainPermanent",
    "ModifyTemporary",
    "class ShiWeiCard",
    "class ZanBiFengMangCard",
    "CanAoPowerIconPatch.TryApply",
    "CANAO_POWER_ICON_PATCH_APPLIED",
    "new CardsVar(2)",
    "ref Task __result"
)

foreach ($Marker in $RequiredMarkers) {
    if (-not $SourceText.Contains($Marker)) {
        throw ("R5 source marker missing: {0}" -f $Marker)
    }
}

$ForbiddenRegexPatterns = @(
    "TaskHelper\.RunSafely",
    "\basync\s+void\b",
    "\.Wait\s*\(",
    "\.Result\b",
    "ModelDb\.Inject",
    "\bInjectModels\b",
    "static\s+int\s+ExtraTriggers",
    "HashSet<CardModel>\s+TemporaryYuHuo",
    "PowerVar<TemporaryFengWeiPower>"
)

foreach ($Pattern in $ForbiddenRegexPatterns) {
    if ($SourceText -match $Pattern) {
        throw ("Forbidden R5 source pattern found: {0}" -f $Pattern)
    }
}

$CardKeys = @(
    "SHI_WEI_CARD.title",
    "SHI_WEI_CARD.description",
    "ZAN_BI_FENG_MANG_CARD.title",
    "ZAN_BI_FENG_MANG_CARD.description"
)

foreach ($LocPath in @($CardsLocZh, $CardsLocEn)) {
    $Loc = (Read-Utf8Strict $LocPath) | ConvertFrom-Json

    foreach ($Key in $CardKeys) {
        if ($null -eq $Loc.PSObject.Properties[$Key]) {
            throw ("Card localization key missing in {0}: {1}" -f
                $LocPath, $Key)
        }
    }
}

$PowerKeys = @(
    "TEMPORARY_FENG_WEI_POWER.title",
    "TEMPORARY_FENG_WEI_POWER.description"
)

foreach ($LocPath in @($PowersLocZh, $PowersLocEn)) {
    $Loc = (Read-Utf8Strict $LocPath) | ConvertFrom-Json

    foreach ($Key in $PowerKeys) {
        if ($null -eq $Loc.PSObject.Properties[$Key]) {
            throw ("Power localization key missing in {0}: {1}" -f
                $LocPath, $Key)
        }
    }
}

$Manifest = (Read-Utf8Strict $ManifestPath) | ConvertFrom-Json

if ($Manifest.version -ne "0.0.5") {
    throw ("Unexpected R5 manifest version: {0}" -f $Manifest.version)
}

if ($Manifest.min_game_version -ne "0.108.0") {
    throw ("Unexpected minimum game version: {0}" -f
        $Manifest.min_game_version)
}

Write-Host "Verified R4 baseline hashes passed."
Write-Host "R5 UTF-8, source, manifest and localization verification passed."
