using CanAoNative.Cards;
using CanAoNative.Rules.Edict;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Relics;

/// <summary>
/// 帝国税契：每场战斗开始时，将 1 张诏令加入手牌。
/// 你每次打出诏令后，失去 1 金币。
/// </summary>
public sealed class DiGuoShuiQiRelic : RelicModel
{
    private bool _combatStartPending;

    public override RelicRarity Rarity => RelicRarity.Shop;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<EdictCard>()
    ];

    public override Task BeforeCombatStart()
    {
        _combatStartPending = true;
        return Task.CompletedTask;
    }

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!_combatStartPending
            || side != Owner.Creature.Side)
        {
            return;
        }

        _combatStartPending = false;
        Flash();

        await EdictService.Generate(
            choiceContext,
            Owner,
            1);
    }

    public override Task AfterCardPlayedLate(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (cardPlay.Card is not EdictCard
            || cardPlay.Card.Owner != Owner)
        {
            return Task.CompletedTask;
        }

        Flash();
        Owner.Gold = Math.Max(
            0,
            Owner.Gold - DynamicVars.Cards.IntValue);

        return Task.CompletedTask;
    }
}
