using CanAoNative.Rules.YuHuo;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Patches;

/// <summary>
/// Renders 浴火 like a native keyword: a gold title on its own line in front
/// of the description. Intrinsic 浴火 is type-based and therefore also shows
/// on canonical (library template) cards; temporary 浴火 is read from the
/// combat-scoped state on mutable instances only.
/// </summary>
[HarmonyPatch(
    typeof(CardModel),
    nameof(CardModel.GetDescriptionForPile),
    typeof(PileType),
    typeof(Creature))]
public static class YuHuoDescriptionPatch
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
