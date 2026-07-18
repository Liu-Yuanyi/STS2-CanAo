$ErrorActionPreference = "Stop"

$Root = Split-Path $PSScriptRoot -Parent
$SourceRoot = Join-Path $Root "src\CanAoNative"
$ManifestPath = Join-Path $Root "packaging\CanAoNative.json"
$CardsLocZh = Join-Path $Root "godot\CanAoNative\localization\zhs\cards.json"
$CardsLocEn = Join-Path $Root "godot\CanAoNative\localization\eng\cards.json"
$PowersLocZh = Join-Path $Root "godot\CanAoNative\localization\zhs\powers.json"
$PowersLocEn = Join-Path $Root "godot\CanAoNative\localization\eng\powers.json"
$RelicsLocZh = Join-Path $Root "godot\CanAoNative\localization\zhs\relics.json"
$RelicsLocEn = Join-Path $Root "godot\CanAoNative\localization\eng\relics.json"
$PotionsLocZh = Join-Path $Root "godot\CanAoNative\localization\zhs\potions.json"
$PotionsLocEn = Join-Path $Root "godot\CanAoNative\localization\eng\potions.json"

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
    "Relics\NiePanHuoZhongRelic.cs",
    "Relics\TianFengJunYinRelic.cs",
    "Relics\QingLuanYuYiRelic.cs",
    "Relics\HeJiWuDianRelic.cs",
    "Relics\ZhanBeiRelic.cs",
    "Relics\GuWangYuZuoRelic.cs",
    "Relics\DiGuoShuiQiRelic.cs",
    "Cards\XingYueWangGuanCard.cs",
    "Powers\XingYueWangGuanPower.cs",
    "Potions\FengWeiJiuPotion.cs",
    "Potions\YuLingPingPotion.cs",
    "Potions\QiongJiangPotion.cs"
)

foreach ($RelativePath in $RequiredFiles) {
    $Path = Join-Path $SourceRoot $RelativePath
    if (-not (Test-Path $Path)) {
        throw ("R8 source file missing: {0}" -f $Path)
    }
}

# Preserve the already user-validated R5/R6/R7 gameplay core. R8 intentionally
# changes CanAoCombatRules and ModEntry, so those files are validated
# structurally below rather than frozen here.
$VerifiedHashes = [ordered]@{
    "Cards\FeatherRanksCard.cs" = "b85c4b790e6b4eb0dddb38a88e94b15c7bdb3568749cef003a65bbdd6d039141"
    "Cards\SacrificialPreparationCard.cs" = "c885a537e62f8f119fed33b32e8c383f2ea7ac31a8538d9991813152570336fb"
    "Cards\StarMoonStrike.cs" = "b43d3e2ca0e7aa0eef27510e35c555ee664e23ecdedbf1cdaa45a9c3f33d489e"
    "Cards\YuHuoBannerCard.cs" = "9bd485b38361230c22181ac969f7843dcb30caa8366e8c2e57ed3698ef1de916"
    "Cards\ShiWeiCard.cs" = "d5a1d84e0388647bd2296d217c58f9f54fb18245ca6331307b0a14b88f100c99"
    "Cards\ZanBiFengMangCard.cs" = "14031597d33e006003362f3b15692bfcfc922f42175e6d52fa9bb82fec30b238"
    "Powers\FengWeiPower.cs" = "1d3df1fdfd1a7f272ca3c9e7fe44a38c0b7b6ebde470fda8b07010c542eb716b"
    "Powers\YuHuoBannerPower.cs" = "3f497726be9b58122f2fd5495d6f6b435b8b371bdd3ae13379521f654e659de9"
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
    "Rules\StarMoon\StarMoonService.cs" = "158187bb559ecad8804cfa29622e804721172719b0f9006bb2fd481fab567ede"
    "Rules\StarMoonHelper.cs" = "9f94f956a9b856117ec01dadf94156d11d6286c27ba9903882ddfba059c76424"
    "Rules\Exhaust\ExhaustRecord.cs" = "d7559b5bee7b41b58e0b4c98454a886fd21ef6cec8c28b80559b92f3b7562f97"
    "Rules\Exhaust\IExhaustEvents.cs" = "da4c1234f28df6f831724cd78ad67505110e3a02f709be44a94ed6690582db58"
    "Rules\Exhaust\ExhaustCombatState.cs" = "66fb6f4e1a20f3a11cef951cbf54084a810acb057dcc67d9a0739f23c941f750"
    "Rules\Exhaust\ExhaustListenerRegistry.cs" = "4fabba3a2c2043a82a8c58e990dc2b963c421d9266d9e5f555bd71fdf4df7ab3"
    "Rules\Exhaust\ExhaustService.cs" = "360ed8f8e2e5601fb94ad575e7110e9eba0c3b35d3f07049977e73cb7ae000c1"
    "Cards\ZhengZhaoCard.cs" = "3223c41021eb6bfdc084dfa5de6ab803bf31c500cb696e504cfd951733b9a3b6"
    "Cards\YuHuoStrikeCard.cs" = "2f23752ce4d26c654e42bb753c8f8df204a1c1148a5ab61d514cb7659e2e931b"
    "Cards\FenGaoJiGuiCard.cs" = "7770f930490b692f1df66fa686d01ef4e8fe8ea5f20c345a9d3d088cfa0bc2f8"
    "Cards\QingGongCard.cs" = "ff6e58a96c5cd070b191674d1b5bcd9b0aa3f9d3362b3cba02dabbd3e0c5f6e3"
    "Cards\FengGuZaiRanCard.cs" = "0be92cc5722e078a2b40bec24613606ab0b25f0ce5605f2cf0511f1b70b43b23"
    "Rules\YuHuo\YuHuoDisplay.cs" = "37ddcc6bcc69421de4c65cb2d9178b6ce4319f10740a488714c797d2e24da2d7"
    "Rules\CanAoHoverTips.cs" = "e4dcbef7fb3f0dc77aae53f9b532771014ed99d91714b00dd2c009dc5e260c44"
    "Patches\YuHuoHoverTipPatch.cs" = "c71e120e2183868a91d7d0982ef259632d8b059baf7bc0e8e9e9b2e0a7a501d3"
    "Patches\YuHuoDescriptionPatch.cs" = "6e75a204f9b41c87028ca0d706f5c4efc003b50469444dc2ae8fd1fd106357fd"
    "Rules\Edict\EdictCombatState.cs" = "b4abddb8013f39c5b93f1bbfcd62ea4914dd07eb1bc56dd2d01dbc7ff6cd685d"
    "Rules\Edict\EdictPlayedContext.cs" = "f733708fdad89e406c8bc34e1cf9a55f2b900587e114d26ed47769547b3c18a1"
    "Rules\Edict\IEdictEvents.cs" = "ffec96a14cbe24b529ca11b40ae00afb941ef631fbbce443a12b73f43ad2cca1"
    "Rules\Edict\EdictListenerRegistry.cs" = "b6635a61eb8f857cac9790b1c8522abb933f8a6636101df4761d3edb87d102f3"
    "Rules\Edict\EdictService.cs" = "18f1732523520bafb85abcc4486aafee3d27065aac52e9498bbfbc2f85649461"
    "Cards\EdictCard.cs" = "bb4cf99271ccde789868b695211cd276bef308de668c03819ed31cde3f5e5a70"
    "Cards\ChuanLingCard.cs" = "5ca3e77e46a698b836d309bbd231afb2859667aab54d2be629716d8a3526f2e7"
    "Cards\MiZhaoCard.cs" = "aebb3f09ee6e8828c7253bdeebc798c803d76737d54cd87b7008437656895188"
    "Cards\WangQuanCard.cs" = "4b7083d920cf4d483c568d2055a6cff2a0cde2a6e95ad3b96820552607600ebe"
    "Cards\DiGuoYuWeiCard.cs" = "8d019864c88b0a799ccea48d493b36bd7d77b91ad7eaf7862c9444f05e49d767"
    "Powers\DiGuoYuWeiPower.cs" = "8ce5226b0b95ca5b22aab54f88f2ca22ec988f237aee0c084ca7d38afdd6709b"
    "Cards\ChengTianShouMingCard.cs" = "1f6067351b79c7a488a0f91a379acf9932bcf277c1f8c530c471023ea25d762d"
    "Cards\TianFengXingTaiCard.cs" = "86a5eceefd2703485457b66c367c544a23fd7a3aed040b20b7fba7f9bd543271"
    "Powers\TianFengXingTaiPower.cs" = "69d3fbda5dbeaa05cdb42d67a6061379c26d2054d0a3473c0747ee4db17fc7ad"
}

foreach ($Entry in $VerifiedHashes.GetEnumerator()) {
    $Path = Join-Path $SourceRoot $Entry.Key
    $Actual = Get-NormalizedTextSha256 $Path

    if ($Actual -ne $Entry.Value) {
        throw (
            "Verified R5/R6/R7 source changed unexpectedly: {0}`nExpected: {1}`nActual:   {2}" -f
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
    "CANAO_NATIVE_R9_RELICS_POTIONS_20260717",
    "class NiePanHuoZhongRelic",
    "class XingYueWangGuanCard",
    "class XingYueWangGuanPower",
    "class TianFengJunYinRelic",
    "class QingLuanYuYiRelic",
    "class HeJiWuDianRelic",
    "class ZhanBeiRelic",
    "class GuWangYuZuoRelic",
    "class DiGuoShuiQiRelic",
    "class FengWeiJiuPotion",
    "class YuLingPingPotion",
    "class QiongJiangPotion",
    "IYuHuoTriggerCountModifier",
    "RelicRarity.Rare",
    "RelicRarity.Shop",
    "PotionRarity.Uncommon",
    "PileType.Discard",
    "ModifyBlockAdditive",
    "ModifyDamageMultiplicative",
    "CardCmd.Upgrade",
    "ref Task __result"
)

foreach ($Marker in $RequiredMarkers) {
    if (-not $SourceText.Contains($Marker)) {
        throw ("R9 source marker missing: {0}" -f $Marker)
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
        throw ("Forbidden R9 source pattern found: {0}" -f $Pattern)
    }
}

$CombatRulesPath = Join-Path $SourceRoot "Rules\CanAoCombatRules.cs"
$CombatRules = Read-Utf8Strict $CombatRulesPath

foreach ($Marker in @(
    "AfterCardPlayedLate",
    "StarMoonService.RecordPlayed",
    "StarMoonService.NotifyAfterPlayed",
    "EdictService.RecordPlayed",
    "EdictService.NotifyAfterPlayed",
    "AfterSideTurnEndLate",
    "StarMoonService.ClearTurnForPlayers",
    "ExhaustService.ClearForPlayers",
    "EdictService.ClearForPlayers",
    "RemoveExpiredForPlayers",
    "AfterCardExhausted",
    "ExhaustService.RecordAndNotify"
)) {
    if (-not $CombatRules.Contains($Marker)) {
        throw ("CanAoCombatRules R9 marker missing: {0}" -f $Marker)
    }
}

if ($CombatRules.Contains("public override Task BeforeSideTurnEnd(")) {
    throw "CanAoCombatRules still clears temporary state in BeforeSideTurnEnd."
}

$CardKeys = @(
    "YU_HUO_KEYWORD",
    "MI_ZHAO_CARD.title",
    "MI_ZHAO_CARD.description",
    "EDICT_CARD.title",
    "EDICT_CARD.description",
    "EDICT_CARD.selectionScreenPrompt",
    "XING_YUE_WANG_GUAN_CARD.title",
    "XING_YUE_WANG_GUAN_CARD.description"
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

$RelicKeys = @(
    "NIE_PAN_HUO_ZHONG_RELIC.title",
    "NIE_PAN_HUO_ZHONG_RELIC.description",
    "NIE_PAN_HUO_ZHONG_RELIC.flavor",
    "TIAN_FENG_JUN_YIN_RELIC.title",
    "TIAN_FENG_JUN_YIN_RELIC.description",
    "QING_LUAN_YU_YI_RELIC.title",
    "QING_LUAN_YU_YI_RELIC.description",
    "HE_JI_WU_DIAN_RELIC.title",
    "HE_JI_WU_DIAN_RELIC.description",
    "ZHAN_BEI_RELIC.title",
    "ZHAN_BEI_RELIC.description",
    "GU_WANG_YU_ZUO_RELIC.title",
    "GU_WANG_YU_ZUO_RELIC.description",
    "DI_GUO_SHUI_QI_RELIC.title",
    "DI_GUO_SHUI_QI_RELIC.description"
)

foreach ($LocPath in @($RelicsLocZh, $RelicsLocEn)) {
    $Loc = (Read-Utf8Strict $LocPath) | ConvertFrom-Json

    foreach ($Key in $RelicKeys) {
        if ($null -eq $Loc.PSObject.Properties[$Key]) {
            throw ("Relic localization key missing in {0}: {1}" -f
                $LocPath, $Key)
        }
    }
}

$PotionKeys = @(
    "FENG_WEI_JIU_POTION.title",
    "FENG_WEI_JIU_POTION.description",
    "YU_LING_PING_POTION.title",
    "YU_LING_PING_POTION.description",
    "QIONG_JIANG_POTION.title",
    "QIONG_JIANG_POTION.description"
)

foreach ($LocPath in @($PotionsLocZh, $PotionsLocEn)) {
    $Loc = (Read-Utf8Strict $LocPath) | ConvertFrom-Json

    foreach ($Key in $PotionKeys) {
        if ($null -eq $Loc.PSObject.Properties[$Key]) {
            throw ("Potion localization key missing in {0}: {1}" -f
                $LocPath, $Key)
        }
    }
}

$PowerKeys = @(
    "XING_YUE_WANG_GUAN_POWER.title",
    "XING_YUE_WANG_GUAN_POWER.description"
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

if ($Manifest.version -ne "0.0.9") {
    throw ("Unexpected R9 manifest version: {0}" -f $Manifest.version)
}

if ($Manifest.min_game_version -ne "0.109.0") {
    throw ("Unexpected minimum game version: {0}" -f
        $Manifest.min_game_version)
}

Write-Host "Verified R5/R6/R7/R8 gameplay-core hashes passed."
Write-Host "R9 relics/potions, UTF-8, manifest and localization verification passed."
