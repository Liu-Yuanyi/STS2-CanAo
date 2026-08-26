using System.Reflection;
using CanAoNative.Pools;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Patches;

/// <summary>
/// 2026-08-16 卡面费用图标修复。原生约定路径
/// res://images/atlases/ui_atlas.sprites/card/energy_&lt;prefix&gt;.tres 会被
/// AtlasResourceLoader 拦截（只认游戏自带图集，我们塞进去的 .tres 永远
/// 拿不到 → 显示 NOPE 占位）。本补丁把残傲卡池的 EnergyIconPath 直接
/// 重定向到独立 PNG，绕开图集管线。与 CanAoPowerIconPatch 同款反射容错。
/// </summary>
public static class CanAoCardPoolIconPatch
{
    private const string CanAoEnergyIcon =
        "res://images/ui/card/energy_can_ao.png";

    public static void TryApply(
        Harmony harmony,
        Logger log)
    {
        ArgumentNullException.ThrowIfNull(harmony);
        ArgumentNullException.ThrowIfNull(log);

        MethodInfo? getter =
            AccessTools.PropertyGetter(typeof(CardPoolModel), "EnergyIconPath");

        MethodInfo? postfix =
            AccessTools.Method(typeof(CanAoCardPoolIconPatch), nameof(Postfix));

        if (getter == null || postfix == null)
        {
            log.Info("CANAO_CARDPOOL_ICON_PATCH_SKIPPED: EnergyIconPath");
            return;
        }

        try
        {
            harmony.Patch(getter, postfix: new HarmonyMethod(postfix));
            log.Info("CANAO_CARDPOOL_ICON_PATCH_APPLIED: EnergyIconPath");
        }
        catch (Exception ex)
        {
            log.Info(
                $"CANAO_CARDPOOL_ICON_PATCH_SKIPPED: EnergyIconPath; " +
                $"{ex.GetType().Name}: {ex.Message}");
        }
    }

    private static void Postfix(
        CardPoolModel __instance,
        ref string __result)
    {
        if (__instance is CanAoCardPool)
            __result = CanAoEnergyIcon;
    }
}
