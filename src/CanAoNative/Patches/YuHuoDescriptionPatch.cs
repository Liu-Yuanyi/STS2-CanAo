using CanAoNative.Rules.YuHuo;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Patches;

/// <summary>
/// Adds localized 浴火 text only to cards that received temporary 浴火.
/// </summary>
[HarmonyPatch(
    typeof(CardModel),
    nameof(CardModel.GetDescriptionForPile),
    typeof(PileType),
    typeof(Creature))]
public static class YuHuoDescriptionPatch
{
    private const string SuffixLocKey = "YU_HUO_TEMP_SUFFIX";

    private static void Postfix(
        CardModel __instance,
        ref string __result)
    {
        if (__instance is IIntrinsicYuHuo
            {
                HasIntrinsicYuHuo: true
            })
        {
            return;
        }

        ICombatState? combatState =
            __instance.CombatState
            ?? __instance.Owner?.Creature?.CombatState;

        if (combatState == null
            || !YuHuoService.HasYuHuo(__instance, combatState))
        {
            return;
        }

        string suffix =
            new LocString("cards", SuffixLocKey)
                .GetFormattedText();

        if (!string.IsNullOrWhiteSpace(suffix)
            && !__result.EndsWith(suffix, StringComparison.Ordinal))
        {
            __result += suffix;
        }
    }
}
