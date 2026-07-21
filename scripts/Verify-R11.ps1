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
$CharsLocZh = Join-Path $Root "godot\CanAoNative\localization\zhs\characters.json"
$CharsLocEn = Join-Path $Root "godot\CanAoNative\localization\eng\characters.json"
$AncientsLocZh = Join-Path $Root "godot\CanAoNative\localization\zhs\ancients.json"
$AncientsLocEn = Join-Path $Root "godot\CanAoNative\localization\eng\ancients.json"

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
    "Characters\CanAo.cs",
    "Pools\CanAoCardPool.cs",
    "Pools\CanAoRelicPool.cs",
    "Pools\CanAoPotionPool.cs",
    "Patches\CanAoAllCharactersPatch.cs",
    "Patches\TouchOfOrobasUpgradePatch.cs",
    "Cards\CanAoStrikeCard.cs",
    "Cards\CanAoDefendCard.cs",
    "Cards\FengYuCanHuoCard.cs",
    "Cards\JiHuoCard.cs",
    "Relics\DiGuoNianBiaoRelic.cs",
    "Relics\DiGuoShiCeRelic.cs",
    "Cards\HuoRenCard.cs",
    "Cards\YuanJunStrikeCard.cs",
    "Patches\BossEpochCharacterPatch.cs",
    "Patches\ArchitectWinRunPatch.cs",
    "Patches\ArchitectCanAoDialoguePatch.cs",
    "Cards\BuDuoCard.cs",
    "Cards\FengHunCard.cs",
    "Cards\JiaoHuiCard.cs",
    "Cards\ShouQueCard.cs",
    "Cards\DengJiCard.cs",
    "Cards\FengHuoJunXieCard.cs",
    "Cards\ZhongZhangCard.cs",
    "Cards\WaMoYuanZhengCard.cs",
    "Cards\WangZuoGuMingCard.cs",
    "Cards\BuMieWangChaoCard.cs",
    "Cards\WanBangLaiChaoCard.cs",
    "Powers\BuDuoPower.cs",
    "Powers\FengHunPower.cs",
    "Powers\JiaoHuiPower.cs",
    "Powers\ShouQuePower.cs",
    "Powers\DengJiPower.cs",
    "Powers\FengHuoJunXiePower.cs",
    "Powers\FengHuoJunXieUpgradedPower.cs",
    "Powers\ZhongZhangPower.cs",
    "Powers\WaMoYuanZhengPower.cs",
    "Powers\WangZuoGuMingPower.cs",
    "Powers\BuMieWangChaoPower.cs",
    "Powers\WanBangLaiChaoPower.cs",
    "Powers\DeferredResourcePowerBase.cs",
    "Powers\NextTurnStarPower.cs"
)

foreach ($RelativePath in $RequiredFiles) {
    $Path = Join-Path $SourceRoot $RelativePath
    if (-not (Test-Path $Path)) {
        throw ("R11 source file missing: {0}" -f $Path)
    }
}

# Preserve the user-validated R5-R9 gameplay core. R11 intentionally
# changes ModEntry, StarPower and MoonPower (ShouQue retention); those
# are validated structurally below rather than frozen here.
$VerifiedHashes = [ordered]@{
    "Cards\FeatherRanksCard.cs" = "4273e2087c9faf972c906922bc8c6f058b87057d8affea32569000a1ab630e50"
    "Cards\SacrificialPreparationCard.cs" = "4cfb0aaea1f4157bcf5ca599a046b4e0925d786b702580810d37614190545797"
    "Cards\StarMoonStrike.cs" = "3b4e429cbee75f2d84517547fe762f3bc3332563072236e591744460bac2220a"
    "Cards\YuHuoBannerCard.cs" = "69e5e1d9a68a61a0326557afa8a4abda9014ce8f2f8e47a706aa42e273dfb58b"
    "Cards\ShiWeiCard.cs" = "677e708596f2a28f6039b60855916b810cd88dadc2920fc80eff836a84dc445e"
    "Cards\ZanBiFengMangCard.cs" = "d3730579491594a67607aebeb3e6375b577b72cc94611bc595925aabdacd6042"
    "Powers\FengWeiPower.cs" = "1d3df1fdfd1a7f272ca3c9e7fe44a38c0b7b6ebde470fda8b07010c542eb716b"
    "Powers\YuHuoBannerPower.cs" = "3f497726be9b58122f2fd5495d6f6b435b8b371bdd3ae13379521f654e659de9"
    "Powers\YuHuoBannerTemporaryStrengthPower.cs" = "66e32ad32cc12d7fc4bc415e2b19a2b6c65fe1e62809fd45f1538747d3054d27"
    "Rules\FengWei\FengWeiService.cs" = "53c1bb467bcba03b9899cfec794beff32032949ddedd3562bbcf2a7e1eeddd49"
    "Rules\YuHuo\YuHuoCombatState.cs" = "598443d5ba9a8eb82adb141cdacbd5d73e35d6433280e0a03ff85d6e8ae9932e"
    "Rules\YuHuo\YuHuoResolver.cs" = "2e63f9cbb43fcd449d63d70d6f922c4fa91f951aa12c837cc69d436470c1468e"
    "Rules\YuHuo\YuHuoService.cs" = "0c5fbffa9f7f88734c160a404f2b98c88921ff8ebe3b0e68b4ef9c13d64a8376"
    "Patches\YuHuoExhaustPatch.cs" = "82441b3866502fc8087dfeec059f15eb3ab36b112911a2240a2a457a648e292b"
    "Cards\PanXuanCard.cs" = "fcc790581ca550cd522150139340a3e2308ed4d3b83ade410cf2afc2ffcf47f3"
    "Cards\XingYueFaMoCard.cs" = "ebe2eb674d8c5bc659d13ded41ef069e3a566270be8aefb8c5f2499a1a763d1b"
    "Cards\TianFengJunZhenCard.cs" = "875f16bdf428a9b4410888ef685ed522cfdaa49b9335caa52212d8c222e59285"
    "Powers\PanXuanPower.cs" = "1dcce5ef0c3b7704af54b7b85cf9a2457fd75233fabf1aeef4535d33abb8f5e8"
    "Powers\TianFengJunZhenPower.cs" = "2cc7e9d0ac165b7df7c82b7afd68764ba2ab5de316a6ec6a8b6a962129a93e58"
    "Powers\TemporaryFengWeiPower.cs" = "137185acb2e72e12da4528750bcb70376524b0707cc13e05981edf19b1e48135"
    "Rules\StarMoon\IStarMoonEvents.cs" = "b227296dfc8799a702b0a6eadf050e8a31d75c77bbe99bbaae0cb6c0b462aa93"
    "Rules\StarMoon\StarMoonCombatState.cs" = "1b6c5cfca7f3cc9d4a8a96f326b3a84ebcf41b6013322adb803bf4168e16c292"
    "Rules\StarMoon\StarMoonGenerationContext.cs" = "9a4becae516e5b031d97971908c2a16b3ce7c76105818f5946a7db44f472856a"
    "Rules\StarMoon\StarMoonPlayedContext.cs" = "2a54e2c28b363e4dda18b8667c2dd323cd60eb3b98daf25367d543a9ca852497"
    "Rules\StarMoon\StarMoonListenerRegistry.cs" = "d739a1117a12fa959479450d56880f5ef11bb99e3273a0e85944b25a40d6ebcb"
    "Rules\StarMoon\StarMoonService.cs" = "a45e8f190567cc5b06ba26549b09e3e27da563bc25bd964cd2301793ecfe4023"
    "Rules\StarMoonHelper.cs" = "9f94f956a9b856117ec01dadf94156d11d6286c27ba9903882ddfba059c76424"
    "Rules\Exhaust\ExhaustRecord.cs" = "d7559b5bee7b41b58e0b4c98454a886fd21ef6cec8c28b80559b92f3b7562f97"
    "Rules\Exhaust\IExhaustEvents.cs" = "da4c1234f28df6f831724cd78ad67505110e3a02f709be44a94ed6690582db58"
    "Rules\Exhaust\ExhaustCombatState.cs" = "66fb6f4e1a20f3a11cef951cbf54084a810acb057dcc67d9a0739f23c941f750"
    "Rules\Exhaust\ExhaustListenerRegistry.cs" = "4fabba3a2c2043a82a8c58e990dc2b963c421d9266d9e5f555bd71fdf4df7ab3"
    "Rules\Exhaust\ExhaustService.cs" = "360ed8f8e2e5601fb94ad575e7110e9eba0c3b35d3f07049977e73cb7ae000c1"
    "Cards\ZhengZhaoCard.cs" = "ed0aff57b9af3c21a11e3815f15553d5f42b78bbda56ca27b5f6b5b29ef4561a"
    "Cards\YuHuoStrikeCard.cs" = "c022fd5a4791b96f4b5c3e85149c1f8f918108cc1faac41c82bee1a16caf7689"
    "Cards\FenGaoJiGuiCard.cs" = "3dbc62dbb0dd3c111e6ea7946ff3a236c945018a69c0d57740b959903d56cce4"
    # QingGongCard removed — moved to 弃稿
    "Cards\FengGuZaiRanCard.cs" = "9e40ce13b87222b0835e6ff34a67f06ab4e9c4dd4b3ba0d75a1d7ff031d374a2"
    "Rules\YuHuo\YuHuoDisplay.cs" = "37ddcc6bcc69421de4c65cb2d9178b6ce4319f10740a488714c797d2e24da2d7"
    "Rules\CanAoHoverTips.cs" = "e4dcbef7fb3f0dc77aae53f9b532771014ed99d91714b00dd2c009dc5e260c44"
    "Patches\YuHuoHoverTipPatch.cs" = "c71e120e2183868a91d7d0982ef259632d8b059baf7bc0e8e9e9b2e0a7a501d3"
    "Patches\YuHuoDescriptionPatch.cs" = "6e75a204f9b41c87028ca0d706f5c4efc003b50469444dc2ae8fd1fd106357fd"
    "Rules\Edict\EdictCombatState.cs" = "b4abddb8013f39c5b93f1bbfcd62ea4914dd07eb1bc56dd2d01dbc7ff6cd685d"
    "Rules\Edict\EdictPlayedContext.cs" = "f733708fdad89e406c8bc34e1cf9a55f2b900587e114d26ed47769547b3c18a1"
    "Rules\Edict\IEdictEvents.cs" = "ffec96a14cbe24b529ca11b40ae00afb941ef631fbbce443a12b73f43ad2cca1"
    "Rules\Edict\EdictListenerRegistry.cs" = "b6635a61eb8f857cac9790b1c8522abb933f8a6636101df4761d3edb87d102f3"
    "Rules\Edict\EdictService.cs" = "95973b773ce77263faf995ac5f6909c590daf7733cf2836305080d408beabef6"
    "Cards\EdictCard.cs" = "0e0cf0d62b0da3df9ae3a21bc6d5b924737091e2e3054f3f4fb9d24b79b9ccec"
    "Cards\ChuanLingCard.cs" = "018e84c640d1911e48845eeade02cb2fd410c6009c7d334bbaeffee984934d71"
    "Cards\MiZhaoCard.cs" = "4d3f67641090d33a7aca6cb79be11fa055c2db94470d9ebf8c7d19b69374c3c5"
    "Cards\WangQuanCard.cs" = "57e451902800af5a8636998880e369d47110281c8713c2bf0aea74f1e27f695e"
    "Cards\DiGuoYuWeiCard.cs" = "acdae500b79fe6a7c7ed76d01621025dc0e66e772d674e846e67b08cdd8bc846"
    "Powers\DiGuoYuWeiPower.cs" = "8ce5226b0b95ca5b22aab54f88f2ca22ec988f237aee0c084ca7d38afdd6709b"
    "Cards\ChengTianShouMingCard.cs" = "8607262eea26da0ca30b3a6940c318cf847a75eba4cc0c4f45586796c81050c0"
    "Cards\TianFengXingTaiCard.cs" = "ed57eb9666fbbc1b06a99868efcf9a2f7986827a39aec563974d98f5d185189b"
    "Powers\TianFengXingTaiPower.cs" = "69d3fbda5dbeaa05cdb42d67a6061379c26d2054d0a3473c0747ee4db17fc7ad"
    "Relics\NiePanHuoZhongRelic.cs" = "10252fa3aa167c2abb239e9312a3cec9b942e24c20064532e0955c2191afbeac"
    "Relics\TianFengJunYinRelic.cs" = "7285187fb95144e7ff8140f780c54c868dc125f2edcc4ffb92752f5346303d08"
    "Relics\QingLuanYuYiRelic.cs" = "ad62ba2cb3e773d580257b8967f81057eedfd19e1cbe89d24f4037a74a3df773"
    "Relics\HeJiWuDianRelic.cs" = "b8bca024db348e206e26a2bac6aaa879df2588c7c8265dbc4ee189b06d7070b7"
    "Relics\ZhanBeiRelic.cs" = "4a36cc39bea3ba30ab49f8a591c51e51ff8f36aeec31b1dd5bbe33354aead037"
    "Relics\GuWangYuZuoRelic.cs" = "6819be49a5f8f41edd714411f84e7e99d07498d5af6ddf98c3e68528e3b43d29"
    "Relics\DiGuoShuiQiRelic.cs" = "ee3b02b2be5c2ed6bf243ce31234124ad9dbdd3dba372032398712c703736a31"
    "Cards\XingYueWangGuanCard.cs" = "3b91a7dfdc4193ddba02a1a1e71dd6b6428f17e0b944125606f39ebeb663e2e4"
    "Powers\XingYueWangGuanPower.cs" = "a6fd4ee53433cecb5b17f603a27d5ccfed24fadd300410d36b433664bcbfe8d8"
    "Potions\FengWeiJiuPotion.cs" = "36347c09f467016b19e4320d4a3fe05fcb265d9e4764c84d59770352340ca933"
    "Potions\YuLingPingPotion.cs" = "332e628a13f8f2858972360f9b11d0de8b7673f60d55de3963a63ffd87ef0ec1"
    "Potions\QiongJiangPotion.cs" = "e6990887b1e66fa6d57296c8ad41ca404cac302c38492793dbefeed0c1d98bdb"
}

foreach ($Entry in $VerifiedHashes.GetEnumerator()) {
    $Path = Join-Path $SourceRoot $Entry.Key
    $Actual = Get-NormalizedTextSha256 $Path

    if ($Actual -ne $Entry.Value) {
        throw (
            "Verified R5-R9 source changed unexpectedly: {0}`nExpected: {1}`nActual:   {2}" -f
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
    "CANAO_NATIVE_R11_POWER_CARDS_20260719",
    "class CanAo : CharacterModel",
    "class CanAoCardPool",
    "class CanAoRelicPool",
    "class CanAoPotionPool",
    "class CanAoAllCharactersPatch",
    "class TouchOfOrobasUpgradePatch",
    "class CanAoCardLibraryFilterPatch",
    "class ElitesEpochCharacterPatch",
    "class YuHuoUpgradeDescriptionPatch",
    "class CanAoStrikeCard",
    "class CanAoDefendCard",
    "class FengYuCanHuoCard",
    "class JiHuoCard",
    "class DiGuoNianBiaoRelic",
    "class DiGuoShiCeRelic",
    "RelicRarity.Starter",
    "ref Task __result",
    "class HuoRenCard : CardModel, IIntrinsicYuHuo",
    "class YuanJunStrikeCard : CardModel, IIntrinsicYuHuo",
    "class ObtainCharUnlockEpochPatch",
    "class BossEpochCharacterPatch",
    "class ArchitectWinRunPatch",
    "class ArchitectCanAoDialoguePatch",
    "class BuDuoCard",
    "class BuDuoPower",
    "class FengHunCard",
    "class FengHunPower",
    "class JiaoHuiCard",
    "class JiaoHuiPower",
    "class ShouQueCard",
    "class ShouQuePower",
    "class DengJiCard",
    "class DengJiPower",
    "class FengHuoJunXieCard",
    "class FengHuoJunXiePower",
    "class FengHuoJunXieUpgradedPower",
    "class ZhongZhangCard",
    "class ZhongZhangPower",
    "class WaMoYuanZhengCard",
    "class WaMoYuanZhengPower",
    "class WangZuoGuMingCard",
    "class WangZuoGuMingPower",
    "class BuMieWangChaoCard",
    "class BuMieWangChaoPower",
    "class WanBangLaiChaoCard",
    "class WanBangLaiChaoPower",
    "class DeferredResourcePowerBase",
    "class NextTurnStarPower"
)

foreach ($Marker in $RequiredMarkers) {
    if (-not $SourceText.Contains($Marker)) {
        throw ("R11 source marker missing: {0}" -f $Marker)
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
        throw ("Forbidden R11 source pattern found: {0}" -f $Pattern)
    }
}

$CombatRulesPath = Join-Path $SourceRoot "Rules\CanAoCombatRules.cs"
$CombatRules = Read-Utf8Strict $CombatRulesPath

foreach ($Marker in @(
    "AfterCardPlayedLate",
    "StarMoonService.RecordPlayed",
    "EdictService.RecordPlayed",
    "AfterSideTurnEndLate",
    "ExhaustService.ClearForPlayers",
    "EdictService.ClearForPlayers",
    "ExhaustService.RecordAndNotify"
)) {
    if (-not $CombatRules.Contains($Marker)) {
        throw ("CanAoCombatRules R11 marker missing: {0}" -f $Marker)
    }
}

$CardKeys = @(
    "YU_HUO_KEYWORD",
    "CAN_AO_STRIKE_CARD.title",
    "CAN_AO_STRIKE_CARD.description",
    "CAN_AO_DEFEND_CARD.title",
    "CAN_AO_DEFEND_CARD.description",
    "FENG_YU_CAN_HUO_CARD.title",
    "FENG_YU_CAN_HUO_CARD.description",
    "JI_HUO_CARD.title",
    "JI_HUO_CARD.description",
    "JI_HUO_CARD.selectionScreenPrompt",
    "HUO_REN_CARD.title",
    "HUO_REN_CARD.description",
    "YUAN_JUN_STRIKE_CARD.title",
    "YUAN_JUN_STRIKE_CARD.description",
    "BU_DUO_CARD.title",
    "BU_DUO_CARD.description",
    "FENG_HUN_CARD.title",
    "FENG_HUN_CARD.description",
    "JIAO_HUI_CARD.title",
    "JIAO_HUI_CARD.description",
    "SHOU_QUE_CARD.title",
    "SHOU_QUE_CARD.description",
    "DENG_JI_CARD.title",
    "DENG_JI_CARD.description",
    "FENG_HUO_JUN_XIE_CARD.title",
    "FENG_HUO_JUN_XIE_CARD.description",
    "ZHONG_ZHANG_CARD.title",
    "ZHONG_ZHANG_CARD.description",
    "WA_MO_YUAN_ZHENG_CARD.title",
    "WA_MO_YUAN_ZHENG_CARD.description",
    "WANG_ZUO_GU_MING_CARD.title",
    "WANG_ZUO_GU_MING_CARD.description",
    "BU_MIE_WANG_CHAO_CARD.title",
    "BU_MIE_WANG_CHAO_CARD.description",
    "WAN_BANG_LAI_CHAO_CARD.title",
    "WAN_BANG_LAI_CHAO_CARD.description",
    "JU_JING_HUI_SHEN_CARD.title",
    "JU_JING_HUI_SHEN_CARD.description"
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
    "BU_DUO_POWER.title",
    "BU_DUO_POWER.description",
    "FENG_HUN_POWER.title",
    "FENG_HUN_POWER.description",
    "JIAO_HUI_POWER.title",
    "JIAO_HUI_POWER.description",
    "SHOU_QUE_POWER.title",
    "SHOU_QUE_POWER.description",
    "DENG_JI_POWER.title",
    "DENG_JI_POWER.description",
    "DENG_JI_POWER.selectionScreenPrompt",
    "FENG_HUO_JUN_XIE_POWER.title",
    "FENG_HUO_JUN_XIE_POWER.description",
    "FENG_HUO_JUN_XIE_UPGRADED_POWER.title",
    "FENG_HUO_JUN_XIE_UPGRADED_POWER.description",
    "ZHONG_ZHANG_POWER.title",
    "ZHONG_ZHANG_POWER.description",
    "WA_MO_YUAN_ZHENG_POWER.title",
    "WA_MO_YUAN_ZHENG_POWER.description",
    "WANG_ZUO_GU_MING_POWER.title",
    "WANG_ZUO_GU_MING_POWER.description",
    "BU_MIE_WANG_CHAO_POWER.title",
    "BU_MIE_WANG_CHAO_POWER.description",
    "WAN_BANG_LAI_CHAO_POWER.title",
    "WAN_BANG_LAI_CHAO_POWER.description",
    "NEXT_TURN_STAR_POWER.title",
    "NEXT_TURN_STAR_POWER.description"
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

$AncientKeys = @(
    "THE_ARCHITECT.talk.CAN_AO.0-0.ancient",
    "THE_ARCHITECT.talk.CAN_AO.0-0.next",
    "THE_ARCHITECT.talk.CAN_AO.0-1.char",
    "THE_ARCHITECT.talk.CAN_AO.0-1.next",
    "THE_ARCHITECT.talk.CAN_AO.0-2.ancient",
    "THE_ARCHITECT.talk.CAN_AO.1-0.ancient",
    "THE_ARCHITECT.talk.CAN_AO.1-0.next",
    "THE_ARCHITECT.talk.CAN_AO.1-1.char",
    "THE_ARCHITECT.talk.CAN_AO.2-0.ancient",
    "THE_ARCHITECT.talk.CAN_AO.2-0.next",
    "THE_ARCHITECT.talk.CAN_AO.2-1.char"
)

foreach ($LocPath in @($AncientsLocZh, $AncientsLocEn)) {
    $Loc = (Read-Utf8Strict $LocPath) | ConvertFrom-Json

    foreach ($Key in $AncientKeys) {
        if ($null -eq $Loc.PSObject.Properties[$Key]) {
            throw ("Ancient localization key missing in {0}: {1}" -f
                $LocPath, $Key)
        }
    }
}

$RelicKeys = @(
    "DI_GUO_NIAN_BIAO_RELIC.title",
    "DI_GUO_NIAN_BIAO_RELIC.description",
    "DI_GUO_NIAN_BIAO_RELIC.flavor",
    "DI_GUO_SHI_CE_RELIC.title",
    "DI_GUO_SHI_CE_RELIC.description",
    "DI_GUO_SHI_CE_RELIC.flavor"
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

$CharacterKeys = @(
    "CAN_AO.title",
    "CAN_AO.titleObject",
    "CAN_AO.description",
    "CAN_AO.pronounObject",
    "CAN_AO.possessiveAdjective",
    "CAN_AO.pronounPossessive",
    "CAN_AO.pronounSubject",
    "CAN_AO.unlockText"
)

foreach ($LocPath in @($CharsLocZh, $CharsLocEn)) {
    $Loc = (Read-Utf8Strict $LocPath) | ConvertFrom-Json

    foreach ($Key in $CharacterKeys) {
        if ($null -eq $Loc.PSObject.Properties[$Key]) {
            throw ("Character localization key missing in {0}: {1}" -f
                $LocPath, $Key)
        }
    }
}

$Manifest = (Read-Utf8Strict $ManifestPath) | ConvertFrom-Json

if ($Manifest.version -ne "0.0.11") {
    throw ("Unexpected R11 manifest version: {0}" -f $Manifest.version)
}

if ($Manifest.min_game_version -ne "0.109.0") {
    throw ("Unexpected minimum game version: {0}" -f
        $Manifest.min_game_version)
}

Write-Host "Verified R5-R9 gameplay-core hashes passed."
Write-Host "R11 power cards, UTF-8, manifest and localization verification passed."
