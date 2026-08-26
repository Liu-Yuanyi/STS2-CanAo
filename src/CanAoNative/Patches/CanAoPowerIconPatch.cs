using System.Reflection;
using CanAoNative.Powers;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Patches;

/// <summary>
/// Applies optional, narrowly-scoped Power icon fallbacks. Reflection is used
/// to locate the current game getters so a renamed/removed UI property does not
/// make Mod initialization fail; gameplay remains available even if this
/// compatibility patch cannot be applied.
/// </summary>
public static class CanAoPowerIconPatch
{
    private const string SmallFallback =
        "res://images/atlases/power_atlas.sprites/strength_power.tres";

    private const string BigFallback =
        "res://images/powers/strength_power.png";

    public static void TryApply(
        Harmony harmony,
        Logger log)
    {
        ArgumentNullException.ThrowIfNull(harmony);
        ArgumentNullException.ThrowIfNull(log);

        TryPatchGetter(
            harmony,
            log,
            "PackedIconPath",
            nameof(PackedIconPostfix));

        TryPatchGetter(
            harmony,
            log,
            "ResolvedBigIconPath",
            nameof(BigIconPostfix));
    }

    private static void TryPatchGetter(
        Harmony harmony,
        Logger log,
        string propertyName,
        string postfixName)
    {
        MethodInfo? getter =
            AccessTools.PropertyGetter(typeof(PowerModel), propertyName);

        MethodInfo? postfix =
            AccessTools.Method(typeof(CanAoPowerIconPatch), postfixName);

        if (getter == null || postfix == null)
        {
            log.Info(
                $"CANAO_POWER_ICON_PATCH_SKIPPED: {propertyName}");
            return;
        }

        try
        {
            harmony.Patch(
                getter,
                postfix: new HarmonyMethod(postfix));

            log.Info(
                $"CANAO_POWER_ICON_PATCH_APPLIED: {propertyName}");
        }
        catch (Exception ex)
        {
            log.Info(
                $"CANAO_POWER_ICON_PATCH_SKIPPED: {propertyName}; " +
                $"{ex.GetType().Name}: {ex.Message}");
        }
    }

    private static void PackedIconPostfix(
        PowerModel __instance,
        ref string __result)
    {
        if (!IsCanAoPower(__instance))
            return;

        // 2026-08-10 图标实装：优先使用每 Power 专属小图
        // res://images/powers/small/<id>.png；缺失时回落原生占位。
        string custom =
            "res://images/powers/small/" +
            __instance.Id.Entry.ToLowerInvariant() + ".png";

        __result = Godot.ResourceLoader.Exists(custom) ? custom : SmallFallback;
    }

    private static void BigIconPostfix(
        PowerModel __instance,
        ref string __result)
    {
        if (!IsCanAoPower(__instance))
            return;

        // 大图默认约定 res://images/powers/<id>.png，存在即用。
        string custom =
            "res://images/powers/" +
            __instance.Id.Entry.ToLowerInvariant() + ".png";

        __result = Godot.ResourceLoader.Exists(custom) ? custom : BigFallback;
    }

    /// <summary>
    /// All CanAo powers live in CanAoNative.Powers; matching by namespace
    /// keeps the fallback working for future powers without a manual list.
    /// </summary>
    private static bool IsCanAoPower(PowerModel power) =>
        power.GetType().Namespace == typeof(StarPower).Namespace;
}
