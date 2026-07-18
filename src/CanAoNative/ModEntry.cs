using CanAoNative.Cards;
using CanAoNative.Patches;
using CanAoNative.Potions;
using CanAoNative.Powers;
using CanAoNative.Relics;
using CanAoNative.Rules;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace CanAoNative;

[ModInitializer(nameof(Initialize))]
public static class ModEntry
{
    public const string ModId = "CanAoNative";
    public const string BuildMarker =
        "CANAO_NATIVE_R9_RELICS_POTIONS_20260717";

    private static readonly Logger Log =
        new(ModId, LogType.Generic);

    public static void Initialize()
    {
        string mvid =
            typeof(ModEntry)
                .Assembly
                .ManifestModule
                .ModuleVersionId
                .ToString();

        Log.Info(
            $"{BuildMarker}; " +
            $"MVID={mvid}; " +
            $"Location={typeof(ModEntry).Assembly.Location}");

        ModHelper.SubscribeForCombatStateHooks(
            $"{ModId}.CombatRules",
            _ =>
            [
                ModelDb.GetById<CanAoCombatRules>(
                    ModelDb.GetId<CanAoCombatRules>())
            ]);

        ModHelper.AddModelToPool<ColorlessCardPool, CanAoProbeCard>();
        ModHelper.AddModelToPool<ColorlessCardPool, CanAoProbePowerCard>();
        ModHelper.AddModelToPool<ColorlessCardPool, GainStarCard>();
        ModHelper.AddModelToPool<ColorlessCardPool, GainMoonCard>();
        ModHelper.AddModelToPool<ColorlessCardPool, GainFengWeiCard>();
        ModHelper.AddModelToPool<ColorlessCardPool, YuHuoSlashCard>();
        ModHelper.AddModelToPool<ColorlessCardPool, SacrificialPreparationCard>();
        ModHelper.AddModelToPool<ColorlessCardPool, FengYanBuXiCard>();
        ModHelper.AddModelToPool<ColorlessCardPool, FeatherRanksCard>();
        ModHelper.AddModelToPool<ColorlessCardPool, YuHuoBannerCard>();
        ModHelper.AddModelToPool<ColorlessCardPool, ShiWeiCard>();
        ModHelper.AddModelToPool<ColorlessCardPool, ZanBiFengMangCard>();
        ModHelper.AddModelToPool<ColorlessCardPool, PanXuanCard>();
        ModHelper.AddModelToPool<ColorlessCardPool, XingYueFaMoCard>();
        ModHelper.AddModelToPool<ColorlessCardPool, TianFengJunZhenCard>();
        ModHelper.AddModelToPool<ColorlessCardPool, ZhengZhaoCard>();
        ModHelper.AddModelToPool<ColorlessCardPool, YuHuoStrikeCard>();
        ModHelper.AddModelToPool<ColorlessCardPool, FenGaoJiGuiCard>();
        ModHelper.AddModelToPool<ColorlessCardPool, QingGongCard>();
        ModHelper.AddModelToPool<ColorlessCardPool, FengGuZaiRanCard>();
        ModHelper.AddModelToPool<ColorlessCardPool, ChuanLingCard>();
        ModHelper.AddModelToPool<ColorlessCardPool, MiZhaoCard>();
        ModHelper.AddModelToPool<ColorlessCardPool, WangQuanCard>();
        ModHelper.AddModelToPool<ColorlessCardPool, DiGuoYuWeiCard>();
        ModHelper.AddModelToPool<ColorlessCardPool, ChengTianShouMingCard>();
        ModHelper.AddModelToPool<ColorlessCardPool, TianFengXingTaiCard>();
        ModHelper.AddModelToPool<ColorlessCardPool, XingYueWangGuanCard>();

        ModHelper.AddModelToPool<SharedRelicPool, NiePanHuoZhongRelic>();
        ModHelper.AddModelToPool<SharedRelicPool, TianFengJunYinRelic>();
        ModHelper.AddModelToPool<SharedRelicPool, QingLuanYuYiRelic>();
        ModHelper.AddModelToPool<SharedRelicPool, HeJiWuDianRelic>();
        ModHelper.AddModelToPool<SharedRelicPool, ZhanBeiRelic>();
        ModHelper.AddModelToPool<SharedRelicPool, GuWangYuZuoRelic>();
        ModHelper.AddModelToPool<SharedRelicPool, DiGuoShuiQiRelic>();

        ModHelper.AddModelToPool<SharedPotionPool, FengWeiJiuPotion>();
        ModHelper.AddModelToPool<SharedPotionPool, YuLingPingPotion>();
        ModHelper.AddModelToPool<SharedPotionPool, QiongJiangPotion>();

        Harmony harmony =
            new($"{ModId}.RuntimePatches");
        harmony.PatchAll(typeof(YuHuoExhaustPatch).Assembly);
        CanAoPowerIconPatch.TryApply(harmony, Log);

        Log.Info(
            $"CANAO_MODELS: " +
            $"Star={ModelDb.GetId(typeof(StarPower))}, " +
            $"Moon={ModelDb.GetId(typeof(MoonPower))}, " +
            $"FengWei={ModelDb.GetId(typeof(FengWeiPower))}, " +
            $"TemporaryFengWei={ModelDb.GetId(typeof(TemporaryFengWeiPower))}, " +
            $"FengYan={ModelDb.GetId(typeof(FengYanBuXiPower))}, " +
            $"StarMoonStrike={ModelDb.GetId(typeof(StarMoonStrike))}, " +
            $"YuHuoSlash={ModelDb.GetId(typeof(YuHuoSlashCard))}, " +
            $"SacrificialPreparation={ModelDb.GetId(typeof(SacrificialPreparationCard))}, " +
            $"FengYanBuXiCard={ModelDb.GetId(typeof(FengYanBuXiCard))}, " +
            $"FeatherRanks={ModelDb.GetId(typeof(FeatherRanksCard))}, " +
            $"YuHuoBannerCard={ModelDb.GetId(typeof(YuHuoBannerCard))}, " +
            $"YuHuoBannerPower={ModelDb.GetId(typeof(YuHuoBannerPower))}, " +
            $"ShiWei={ModelDb.GetId(typeof(ShiWeiCard))}, " +
            $"ZanBiFengMang={ModelDb.GetId(typeof(ZanBiFengMangCard))}, " +
            $"PanXuan={ModelDb.GetId(typeof(PanXuanCard))}, " +
            $"PanXuanPower={ModelDb.GetId(typeof(PanXuanPower))}, " +
            $"XingYueFaMo={ModelDb.GetId(typeof(XingYueFaMoCard))}, " +
            $"TianFengJunZhen={ModelDb.GetId(typeof(TianFengJunZhenCard))}, " +
            $"TianFengJunZhenPower={ModelDb.GetId(typeof(TianFengJunZhenPower))}, " +
            $"ZhengZhao={ModelDb.GetId(typeof(ZhengZhaoCard))}, " +
            $"YuHuoStrike={ModelDb.GetId(typeof(YuHuoStrikeCard))}, " +
            $"FenGaoJiGui={ModelDb.GetId(typeof(FenGaoJiGuiCard))}, " +
            $"QingGong={ModelDb.GetId(typeof(QingGongCard))}, " +
            $"FengGuZaiRan={ModelDb.GetId(typeof(FengGuZaiRanCard))}, " +
            $"Edict={ModelDb.GetId(typeof(EdictCard))}, " +
            $"ChuanLing={ModelDb.GetId(typeof(ChuanLingCard))}, " +
            $"MiZhao={ModelDb.GetId(typeof(MiZhaoCard))}, " +
            $"WangQuan={ModelDb.GetId(typeof(WangQuanCard))}, " +
            $"DiGuoYuWei={ModelDb.GetId(typeof(DiGuoYuWeiCard))}, " +
            $"DiGuoYuWeiPower={ModelDb.GetId(typeof(DiGuoYuWeiPower))}, " +
            $"ChengTianShouMing={ModelDb.GetId(typeof(ChengTianShouMingCard))}, " +
            $"TianFengXingTai={ModelDb.GetId(typeof(TianFengXingTaiCard))}, " +
            $"TianFengXingTaiPower={ModelDb.GetId(typeof(TianFengXingTaiPower))}, " +
            $"NiePanHuoZhong={ModelDb.GetId(typeof(NiePanHuoZhongRelic))}, " +
            $"XingYueWangGuan={ModelDb.GetId(typeof(XingYueWangGuanCard))}, " +
            $"XingYueWangGuanPower={ModelDb.GetId(typeof(XingYueWangGuanPower))}, " +
            $"FengWeiJiu={ModelDb.GetId(typeof(FengWeiJiuPotion))}, " +
            $"YuLingPing={ModelDb.GetId(typeof(YuLingPingPotion))}, " +
            $"TianFengJunYin={ModelDb.GetId(typeof(TianFengJunYinRelic))}, " +
            $"QingLuanYuYi={ModelDb.GetId(typeof(QingLuanYuYiRelic))}, " +
            $"HeJiWuDian={ModelDb.GetId(typeof(HeJiWuDianRelic))}, " +
            $"ZhanBei={ModelDb.GetId(typeof(ZhanBeiRelic))}, " +
            $"GuWangYuZuo={ModelDb.GetId(typeof(GuWangYuZuoRelic))}, " +
            $"DiGuoShuiQi={ModelDb.GetId(typeof(DiGuoShuiQiRelic))}, " +
            $"QiongJiang={ModelDb.GetId(typeof(QiongJiangPotion))}");
    }
}
