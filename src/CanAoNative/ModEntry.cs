using CanAoNative.Cards;
using CanAoNative.Characters;
using CanAoNative.Patches;
using CanAoNative.Pools;
using CanAoNative.Potions;
using CanAoNative.Powers;
using CanAoNative.Relics;
using CanAoNative.Rules;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative;

[ModInitializer(nameof(Initialize))]
public static class ModEntry
{
    public const string ModId = "CanAoNative";
    public const string BuildMarker =
        "CANAO_NATIVE_R11_POWER_CARDS_20260719";

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

        Harmony harmony =
            new($"{ModId}.RuntimePatches");
        harmony.PatchAll(typeof(YuHuoExhaustPatch).Assembly);
        CanAoPowerIconPatch.TryApply(harmony, Log);
        CanAoRelicIconPatch.TryApply(harmony, Log);
        CanAoPotionIconPatch.TryApply(harmony, Log);
        CanAoCardPoolIconPatch.TryApply(harmony, Log);

        Log.Info(
            $"CANAO_MODELS: " +
            $"CanAo={ModelDb.GetId(typeof(CanAo))}, " +
            $"CardPool={ModelDb.GetId(typeof(CanAoCardPool))}, " +
            $"RelicPool={ModelDb.GetId(typeof(CanAoRelicPool))}, " +
            $"PotionPool={ModelDb.GetId(typeof(CanAoPotionPool))}, " +
            $"Star={ModelDb.GetId(typeof(StarPower))}, " +
            $"Moon={ModelDb.GetId(typeof(MoonPower))}, " +
            $"FengWei={ModelDb.GetId(typeof(FengWeiPower))}, " +
            $"TemporaryFengWei={ModelDb.GetId(typeof(TemporaryFengWeiPower))}, " +
            $"FengYan={ModelDb.GetId(typeof(FengYanBuXiPower))}, " +
            $"StarMoonStrike={ModelDb.GetId(typeof(StarMoonStrike))}, " +
            $"Edict={ModelDb.GetId(typeof(EdictCard))}, " +
            $"CanAoStrike={ModelDb.GetId(typeof(CanAoStrikeCard))}, " +
            $"CanAoDefend={ModelDb.GetId(typeof(CanAoDefendCard))}, " +
            $"FengYuCanHuo={ModelDb.GetId(typeof(FengYuCanHuoCard))}, " +
            $"JiHuo={ModelDb.GetId(typeof(JiHuoCard))}, " +
            $"DiGuoNianBiao={ModelDb.GetId(typeof(DiGuoNianBiaoRelic))}, " +
            $"DiGuoShiCe={ModelDb.GetId(typeof(DiGuoShiCeRelic))}");
    }
}
