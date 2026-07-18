using CanAoNative.Rules.YuHuo;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Patches;

/// <summary>
/// The card library's upgrade preview uses GetDescriptionForUpgradePreview,
/// which bypasses GetDescriptionForPile. Add the same gold 浴火 keyword
/// line there so intrinsic 浴火 stays visible in upgrade view.
/// </summary>
[HarmonyPatch(typeof(CardModel), nameof(CardModel.GetDescriptionForUpgradePreview))]
public static class YuHuoUpgradeDescriptionPatch
{
    private static void Postfix(
        CardModel __instance,
        ref string __result)
    {
        if (!YuHuoDisplay.HasYuHuo(__instance))
            return;

        string prefix = YuHuoDisplay.KeywordLine;

        if (!__result.StartsWith(prefix, StringComparison.Ordinal))
            __result = prefix + "\n" + __result;
    }
}
