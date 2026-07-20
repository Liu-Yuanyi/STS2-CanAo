using CanAoNative.Rules.YuHuo;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Powers;

/// <summary>
/// 登基：每回合开始时，查看抽牌堆顶的 2 张牌并消耗其中 1 张。
/// 若消耗了浴火牌，抽 Amount 张牌。
/// 堆顶为 CardPile.Cards 的 index 0；消耗走统一的 CardCmd.Exhaust，
/// 浴火牌会正常触发浴火自动打出。
/// </summary>
public sealed class DengJiPower : PowerModel
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

        ICombatState? combatState = Owner.CombatState;

        if (combatState == null)
            return;

        CardPile drawPile = player.PlayerCombatState.DrawPile;
        List<CardModel> topCards = drawPile.Cards.Take(2).ToList();

        if (topCards.Count == 0)
            return;

        Flash();

        CardSelectorPrefs prefs = new(SelectionScreenPrompt, 1);

        CardModel? selected =
            (await CardSelectCmd.FromCombatPile(
                choiceContext,
                drawPile,
                player,
                prefs,
                card => topCards.Contains(card)))
            .FirstOrDefault();

        if (selected == null)
            return;

        bool hadYuHuo = YuHuoService.HasYuHuo(selected, combatState);

        await CardCmd.Exhaust(choiceContext, selected);

        if (!hadYuHuo)
            return;

        await CardPileCmd.Draw(
            choiceContext,
            Amount,
            player);
    }
}
