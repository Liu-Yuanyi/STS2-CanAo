using System.Reflection;
using CanAoNative.Relics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Patches;

/// <summary>
/// 2026-08-10 遗物图标实装。遗物小图/轮廓的原生约定路径是 atlas .tres，
/// 本补丁把残傲遗物的这两条 getter 重定向到 pck 内的独立 PNG
/// （与卡图 PortraitPath 同一加载机制）；大图沿用原生约定
/// res://images/relics/&lt;id&gt;.png，由 ResolvedBigIconPath 自动命中，
/// 无需补丁。与 CanAoPowerIconPatch 同款反射容错：补丁失败不阻塞 Mod 初始化。
/// </summary>
public static class CanAoRelicIconPatch
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
            "PackedIconPath",
            nameof(PackedIconPostfix));

        TryPatchGetter(
            harmony,
            log,
            "PackedIconOutlinePath",
            nameof(OutlinePostfix));
    }

    private static void TryPatchGetter(
        Harmony harmony,
        Logger log,
        string propertyName,
        string postfixName)
    {
        MethodInfo? getter =
            AccessTools.PropertyGetter(typeof(RelicModel), propertyName);

        MethodInfo? postfix =
            AccessTools.Method(typeof(CanAoRelicIconPatch), postfixName);

        if (getter == null || postfix == null)
        {
            log.Info(
                $"CANAO_RELIC_ICON_PATCH_SKIPPED: {propertyName}");
            return;
        }

        try
        {
            harmony.Patch(
                getter,
                postfix: new HarmonyMethod(postfix));

            log.Info(
                $"CANAO_RELIC_ICON_PATCH_APPLIED: {propertyName}");
        }
        catch (Exception ex)
        {
            log.Info(
                $"CANAO_RELIC_ICON_PATCH_SKIPPED: {propertyName}; " +
                $"{ex.GetType().Name}: {ex.Message}");
        }
    }

    private static void PackedIconPostfix(
        RelicModel __instance,
        ref string __result)
    {
        if (!IsCanAoRelic(__instance))
            return;

        string custom =
            "res://images/relics/small/" +
            __instance.Id.Entry.ToLowerInvariant() + ".png";

        if (Godot.ResourceLoader.Exists(custom))
            __result = custom;
    }

    private static void OutlinePostfix(
        RelicModel __instance,
        ref string __result)
    {
        if (!IsCanAoRelic(__instance))
            return;

        string custom =
            "res://images/relics/outline/" +
            __instance.Id.Entry.ToLowerInvariant() + ".png";

        if (Godot.ResourceLoader.Exists(custom))
            __result = custom;
    }

    /// <summary>
    /// All CanAo relics live in CanAoNative.Relics; matching by namespace
    /// keeps the patch working for future relics without a manual list.
    /// </summary>
    private static bool IsCanAoRelic(RelicModel relic) =>
        relic.GetType().Namespace == typeof(DiGuoNianBiaoRelic).Namespace;
}
