using CanAoNative.Rules;
using CanAoNative.Rules.YuHuo;
using HarmonyLib;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Patches;

/// <summary>
/// Adds the 浴火 hover tip to every card that currently has 浴火, mirroring
/// the game's automatic keyword hover tips.
/// </summary>
[HarmonyPatch(typeof(CardModel), "get_HoverTips")]
public static class YuHuoHoverTipPatch
{
    private static void Postfix(
        CardModel __instance,
        ref IEnumerable<IHoverTip> __result)
    {
        if (YuHuoDisplay.HasYuHuo(__instance))
            __result = __result.Append(CanAoHoverTips.YuHuo);
    }
}
