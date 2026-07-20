using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Powers;

/// <summary>
/// 不灭王朝：拥有者的回合开始时，将其弃牌堆中 Amount 张随机攻击牌或
/// 技能牌加入手牌（逐张不放回选取）。随机走
/// RunState.Rng.CombatCardSelection，与游戏原生"随机选牌"同一 RNG 流。
/// </summary>
public sealed class BuMieWangChaoPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (!ReferenceEquals(player.Creature, Owner)
            || Amount <= 0)
        {
            return;
        }

        for (int i = 0; i < (int)Amount; i++)
        {
            List<CardModel> candidates =
                player.PlayerCombatState.DiscardPile.Cards
                    .Where(card =>
                        card.Type is CardType.Attack or CardType.Skill)
                    .ToList();

            if (candidates.Count == 0)
                return;

            CardModel? selected =
                player.RunState.Rng.CombatCardSelection.NextItem(
                    candidates);

            if (selected == null)
                return;

            if (i == 0)
                Flash();

            await CardPileCmd.Add(selected, PileType.Hand);
        }
    }
}
