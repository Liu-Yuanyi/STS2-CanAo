using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Rules.YuHuo;

/// <summary>
/// Canonical-safe 浴火 display helpers shared by the description and hover
/// tip patches.
/// </summary>
public static class YuHuoDisplay
{
    /// <summary>
    /// True for intrinsic 浴火 (type-level, safe on canonical template cards)
    /// or temporary 浴火 on a mutable combat instance.
    /// </summary>
    public static bool HasYuHuo(CardModel card)
    {
        if (card is IIntrinsicYuHuo { HasIntrinsicYuHuo: true })
            return true;

        // Canonical (library template) models throw when Owner is accessed.
        if (card.IsCanonical)
            return false;

        ICombatState? combatState =
            card.CombatState ?? card.Owner?.Creature?.CombatState;

        return combatState != null
               && YuHuoService.HasYuHuo(card, combatState);
    }

    /// <summary>
    /// Gold 浴火 keyword line in the exact native keyword format:
    /// [gold]title[/gold]period.
    /// </summary>
    public static string KeywordLine
    {
        get
        {
            string title =
                new LocString("cards", "YU_HUO_KEYWORD").GetFormattedText();

            string period =
                new LocString("card_keywords", "PERIOD").GetRawText();

            return $"[gold]{title}[/gold]{period}";
        }
    }
}
