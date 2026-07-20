using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Powers;

/// <summary>
/// 终章：当拥有者的手牌只有 1 张时，那张牌的费用 -Amount。
/// 走原生 AbstractModel.TryModifyEnergyCostInCombat 全局费用 Hook，
/// 牌面数字与费用颜色由游戏自动刷新。
/// </summary>
public sealed class ZhongZhangPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool TryModifyEnergyCostInCombat(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost)
    {
        modifiedCost = originalCost;

        if (originalCost < 0m || Amount <= 0)
            return false;

        Player? player = Owner.Player;

        if (player == null)
            return false;

        IReadOnlyList<CardModel> hand =
            player.PlayerCombatState.Hand.Cards;

        if (hand.Count != 1 || !ReferenceEquals(hand[0], card))
            return false;

        modifiedCost = originalCost > Amount
            ? originalCost - Amount
            : 0m;

        return modifiedCost != originalCost;
    }
}
