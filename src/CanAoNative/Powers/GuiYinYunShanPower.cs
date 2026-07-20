using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Powers;

/// <summary>
/// 归隐陨山：技能牌的费用 -Amount（多张归隐陨山叠层相加）。
/// </summary>
public sealed class GuiYinYunShanPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool TryModifyEnergyCostInCombat(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost)
    {
        if (card.Type == CardType.Skill
            && card.Owner?.Creature == Owner
            && originalCost > 0m
            && Amount > 0)
        {
            modifiedCost = originalCost > Amount
                ? originalCost - Amount
                : 0m;

            return modifiedCost != originalCost;
        }

        return base.TryModifyEnergyCostInCombat(
            card,
            originalCost,
            out modifiedCost);
    }
}
