$ErrorActionPreference = "Stop"

$Root = Split-Path $PSScriptRoot -Parent
$SourceRoot = Join-Path $Root "src\CanAoNative"
$ManifestPath = Join-Path $Root "packaging\CanAoNative.json"
$CardsLocZh = Join-Path $Root "godot\CanAoNative\localization\zhs\cards.json"
$CardsLocEn = Join-Path $Root "godot\CanAoNative\localization\eng\cards.json"

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
    "Cards\ZhengZhaoCard.cs",
    "Cards\YuHuoStrikeCard.cs",
    "Cards\FenGaoJiGuiCard.cs",
    "Cards\QingGongCard.cs",
    "Cards\FengGuZaiRanCard.cs",
    "Rules\Exhaust\ExhaustRecord.cs",
    "Rules\Exhaust\IExhaustEvents.cs",
    "Rules\Exhaust\ExhaustCombatState.cs",
    "Rules\Exhaust\ExhaustListenerRegistry.cs",
    "Rules\Exhaust\ExhaustService.cs",
    "Rules\CanAoHoverTips.cs",
    "Rules\YuHuo\YuHuoDisplay.cs",
    "Patches\YuHuoHoverTipPatch.cs"
)

foreach ($RelativePath in $RequiredFiles) {
    $Path = Join-Path $SourceRoot $RelativePath
    if (-not (Test-Path $Path)) {
        throw ("R7 source file missing: {0}" -f $Path)
    }
}

# Preserve the already user-validated R5/R6 gameplay core. R7 intentionally
# changes CanAoCombatRules and ModEntry, so those files are validated
# structurally below rather than frozen here. The eight card files below were
# intentionally re-touched in the FIX2 text-convention pass (ExtraHoverTips /
# library visibility only); their hashes were refreshed at that point.
$VerifiedHashes = [ordered]@{
    "Cards\FeatherRanksCard.cs" = "b85c4b790e6b4eb0dddb38a88e94b15c7bdb3568749cef003a65bbdd6d039141"
    "Cards\SacrificialPreparationCard.cs" = "c885a537e62f8f119fed33b32e8c383f2ea7ac31a8538d9991813152570336fb"
    "Cards\StarMoonStrike.cs" = "b43d3e2ca0e7aa0eef27510e35c555ee664e23ecdedbf1cdaa45a9c3f33d489e"
    "Cards\YuHuoBannerCard.cs" = "9e943bab9fd839962dcabb18b435ccee87c60271011daf185e19827c73bbb01e"
    "Cards\ShiWeiCard.cs" = "d5a1d84e0388647bd2296d217c58f9f54fb18245ca6331307b0a14b88f100c99"
    "Cards\ZanBiFengMangCard.cs" = "14031597d33e006003362f3b15692bfcfc922f42175e6d52fa9bb82fec30b238"
    "Powers\FengWeiPower.cs" = "1d3df1fdfd1a7f272ca3c9e7fe44a38c0b7b6ebde470fda8b07010c542eb716b"
    "Powers\YuHuoBannerPower.cs" = "2f33f4294354c90ae8941ec5208597d4d5f54562d76637ca04ff0226d99ad50a"
    "Powers\YuHuoBannerTemporaryStrengthPower.cs" = "66e32ad32cc12d7fc4bc415e2b19a2b6c65fe1e62809fd45f1538747d3054d27"
    "Rules\FengWei\FengWeiService.cs" = "c88a0cae39486db68fad2c0beab7ca612dce20b27677b7541a6ef91559022461"
    "Rules\YuHuo\YuHuoCombatState.cs" = "3f12ec54d3d35da3c94d5427653dca446b439d389ec877c3fadc1dce66d123c1"
    "Rules\YuHuo\YuHuoResolver.cs" = "2e63f9cbb43fcd449d63d70d6f922c4fa91f951aa12c837cc69d436470c1468e"
    "Rules\YuHuo\YuHuoService.cs" = "bca86dcb0ed4a5caa7c0c9945eac654e90278da406ae4cb4c4c94be3f38edbe3"
    "Patches\YuHuoExhaustPatch.cs" = "82441b3866502fc8087dfeec059f15eb3ab36b112911a2240a2a457a648e292b"
    "Cards\PanXuanCard.cs" = "16cba702ced3bc31b63bebbd8a632bbbd1b1832c7b15e79263272996db479d6f"
    "Cards\XingYueFaMoCard.cs" = "1cdd5683ace925ad677c9a1e10ebabd09705f84f2e036ee9f9052c71355899b3"
    "Cards\TianFengJunZhenCard.cs" = "5eea54215ad465e1a09a8f728ab4ad87530e5bc7926db0610a0e0f1b626640f6"
    "Powers\PanXuanPower.cs" = "1dcce5ef0c3b7704af54b7b85cf9a2457fd75233fabf1aeef4535d33abb8f5e8"
    "Powers\TianFengJunZhenPower.cs" = "2cc7e9d0ac165b7df7c82b7afd68764ba2ab5de316a6ec6a8b6a962129a93e58"
    "Powers\TemporaryFengWeiPower.cs" = "0b1a10135751d7e6e90d7277edbee14a23be420a05427f770916ba5881003a02"
    "Rules\StarMoon\IStarMoonEvents.cs" = "b227296dfc8799a702b0a6eadf050e8a31d75c77bbe99bbaae0cb6c0b462aa93"
    "Rules\StarMoon\StarMoonCombatState.cs" = "843e44301a7fd185dc9410051cece9346855daae420d39c7eda99b7c045c87c4"
    "Rules\StarMoon\StarMoonGenerationContext.cs" = "9a4becae516e5b031d97971908c2a16b3ce7c76105818f5946a7db44f472856a"
    "Rules\StarMoon\StarMoonPlayedContext.cs" = "2a54e2c28b363e4dda18b8667c2dd323cd60eb3b98daf25367d543a9ca852497"
    "Rules\StarMoon\StarMoonListenerRegistry.cs" = "d739a1117a12fa959479450d56880f5ef11bb99e3273a0e85944b25a40d6ebcb"
    "Rules\StarMoon\StarMoonService.cs" = "c21b96c9f674c543b10de1661c69f2b6961d63b6a33860768da20198155afa5a"
    "Rules\StarMoonHelper.cs" = "9f94f956a9b856117ec01dadf94156d11d6286c27ba9903882ddfba059c76424"
}

foreach ($Entry in $VerifiedHashes.GetEnumerator()) {
    $Path = Join-Path $SourceRoot $Entry.Key
    $Actual = Get-NormalizedTextSha256 $Path

    if ($Actual -ne $Entry.Value) {
        throw (
            "Verified R5/R6 source changed unexpectedly: {0}`nExpected: {1}`nActual:   {2}" -f
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
    "CANAO_NATIVE_R7_EXHAUST_EVENTS_20260717",
    "enum CanAoExhaustCause",
    "record ExhaustRecord",
    "interface IAfterCanAoCardExhausted",
    "class ExhaustCombatState",
    "class ExhaustService",
    "RecordAndNotify",
    "GetRecordsThisTurn",
    "AfterCardExhausted",
    "class ZhengZhaoCard",
    "class YuHuoStrikeCard",
    "class FenGaoJiGuiCard",
    "class QingGongCard",
    "class FengGuZaiRanCard",
    "class YuHuoHoverTipPatch",
    "class YuHuoDisplay",
    "class CanAoHoverTips",
    "ExtraHoverTips",
    "IsCanonical",
    "ref Task __result"
)

foreach ($Marker in $RequiredMarkers) {
    if (-not $SourceText.Contains($Marker)) {
        throw ("R7 source marker missing: {0}" -f $Marker)
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
    "HashSet<CardModel>\s+TemporaryYuHuo"
)

foreach ($Pattern in $ForbiddenRegexPatterns) {
    if ($SourceText -match $Pattern) {
        throw ("Forbidden R7 source pattern found: {0}" -f $Pattern)
    }
}

$CombatRulesPath = Join-Path $SourceRoot "Rules\CanAoCombatRules.cs"
$CombatRules = Read-Utf8Strict $CombatRulesPath

foreach ($Marker in @(
    "AfterCardPlayedLate",
    "StarMoonService.RecordPlayed",
    "StarMoonService.NotifyAfterPlayed",
    "AfterSideTurnEndLate",
    "StarMoonService.ClearTurnForPlayers",
    "RemoveExpiredForPlayers",
    "AfterCardExhausted",
    "ExhaustService.RecordAndNotify",
    "ExhaustService.ClearForPlayers"
)) {
    if (-not $CombatRules.Contains($Marker)) {
        throw ("CanAoCombatRules R7 marker missing: {0}" -f $Marker)
    }
}

if ($CombatRules.Contains("public override Task BeforeSideTurnEnd(")) {
    throw "CanAoCombatRules still clears temporary state in BeforeSideTurnEnd."
}

$ExhaustServicePath = Join-Path $SourceRoot "Rules\Exhaust\ExhaustService.cs"
$ExhaustServiceText = Read-Utf8Strict $ExhaustServicePath

foreach ($Marker in @(
    "YuHuoService.HasYuHuo",
    "YuHuoService.IsResolving",
    "NotifyAfterExhausted",
    "GetState(combatState).Record"
)) {
    if (-not $ExhaustServiceText.Contains($Marker)) {
        throw ("ExhaustService R7 marker missing: {0}" -f $Marker)
    }
}

$CardKeys = @(
    "YU_HUO_KEYWORD",
    "ZHENG_ZHAO_CARD.title",
    "ZHENG_ZHAO_CARD.description",
    "YU_HUO_STRIKE_CARD.title",
    "YU_HUO_STRIKE_CARD.description",
    "FEN_GAO_JI_GUI_CARD.title",
    "FEN_GAO_JI_GUI_CARD.description",
    "FEN_GAO_JI_GUI_CARD.selectionScreenPrompt",
    "QING_GONG_CARD.title",
    "QING_GONG_CARD.description",
    "FENG_GU_ZAI_RAN_CARD.title",
    "FENG_GU_ZAI_RAN_CARD.description",
    "FENG_GU_ZAI_RAN_CARD.selectionScreenPrompt"
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

$TipsLocZh = Join-Path $Root "godot\CanAoNative\localization\zhs\static_hover_tips.json"
$TipsLocEn = Join-Path $Root "godot\CanAoNative\localization\eng\static_hover_tips.json"

foreach ($LocPath in @($TipsLocZh, $TipsLocEn)) {
    $Loc = (Read-Utf8Strict $LocPath) | ConvertFrom-Json

    foreach ($Key in @("YU_HUO.title", "YU_HUO.description")) {
        if ($null -eq $Loc.PSObject.Properties[$Key]) {
            throw ("Static hover tip key missing in {0}: {1}" -f
                $LocPath, $Key)
        }
    }
}

$Manifest = (Read-Utf8Strict $ManifestPath) | ConvertFrom-Json

if ($Manifest.version -ne "0.0.7") {
    throw ("Unexpected R7 manifest version: {0}" -f $Manifest.version)
}

if ($Manifest.min_game_version -ne "0.109.0") {
    throw ("Unexpected minimum game version: {0}" -f
        $Manifest.min_game_version)
}

Write-Host "Verified R5/R6 gameplay-core hashes passed."
Write-Host "R7 exhaust event layer, UTF-8, manifest and localization verification passed."
