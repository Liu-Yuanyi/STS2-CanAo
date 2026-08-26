using System.Reflection;
using CanAoNative.Potions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Patches;

/// <summary>
/// 2026-08-16 药水图标实装。药水小图/轮廓的原生约定路径是 atlas .tres，
/// 本补丁把残傲药水的这两条 getter 重定向到 pck 内的独立 PNG；
/// 大图沿用原生约定 res://images/potions/large/&lt;id&gt;.png 自动命中，
/// 无需补丁。与 CanAoPowerIconPatch 同款反射容错。
/// </summary>
public static class CanAoPotionIconPatch
{
    public static void TryApply(
        Harmony harmony,
        Logger log)
    {
        ArgumentNullException.ThrowIfNull(harmony);
        ArgumentNullException.ThrowIfNull(log);

        TryPatchGetter(
            harmony,
            log,
            "PackedImagePath",
            nameof(PackedImagePostfix));

        TryPatchGetter(
            harmony,
            log,
            "PackedOutlinePath",
            nameof(OutlinePostfix));
    }

    private static void TryPatchGetter(
        Harmony harmony,
        Logger log,
        string propertyName,
        string postfixName)
    {
        MethodInfo? getter =
            AccessTools.PropertyGetter(typeof(PotionModel), propertyName);

        MethodInfo? postfix =
            AccessTools.Method(typeof(CanAoPotionIconPatch), postfixName);

        if (getter == null || postfix == null)
        {
            log.Info(
                $"CANAO_POTION_ICON_PATCH_SKIPPED: {propertyName}");
            return;
        }

        try
        {
            harmony.Patch(
                getter,
                postfix: new HarmonyMethod(postfix));

            log.Info(
                $"CANAO_POTION_ICON_PATCH_APPLIED: {propertyName}");
        }
        catch (Exception ex)
        {
            log.Info(
                $"CANAO_POTION_ICON_PATCH_SKIPPED: {propertyName}; " +
                $"{ex.GetType().Name}: {ex.Message}");
        }
    }

    private static void PackedImagePostfix(
        PotionModel __instance,
        ref string __result)
    {
        if (!IsCanAoPotion(__instance))
            return;

        string custom =
            "res://images/potions/small/" +
            __instance.Id.Entry.ToLowerInvariant() + ".png";

        if (Godot.ResourceLoader.Exists(custom))
            __result = custom;
    }

    private static void OutlinePostfix(
        PotionModel __instance,
        ref string __result)
    {
        if (!IsCanAoPotion(__instance))
            return;

        string custom =
            "res://images/potions/outline/" +
            __instance.Id.Entry.ToLowerInvariant() + ".png";

        if (Godot.ResourceLoader.Exists(custom))
            __result = custom;
    }

    /// <summary>
    /// All CanAo potions live in CanAoNative.Potions; matching by namespace
    /// keeps the patch working for future potions without a manual list.
    /// </summary>
    private static bool IsCanAoPotion(PotionModel potion) =>
        potion.GetType().Namespace == typeof(QiongJiangPotion).Namespace;
}
