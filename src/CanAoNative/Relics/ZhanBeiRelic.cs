using CanAoNative.Cards;
using CanAoNative.Powers;
using CanAoNative.Rules.FengWei;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Relics;

/// <summary>
/// 战碑：每场战斗开始时，获得 3 点凤威。第一回合，
/// 你的星月合击只造成伤害，不获得格挡。
/// </summary>
public sealed class ZhanBeiRelic : RelicModel
{
    private bool _combatStartPending;

    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<FengWeiPower>(),
        HoverTipFactory.FromCard<StarMoonStrike>()
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

        await FengWeiService.GainPermanent(
            choiceContext,
            Owner,
            DynamicVars.Cards.IntValue,
            null);
    }

    /// <summary>
    /// ModifyBlockAdditive is a DELTA channel (0 = unchanged); the
    /// multiplicative channel is where suppression belongs. Returning 0 here
    /// zeroes the strike's block gain on the owner's first turn.
    /// </summary>
    public override decimal ModifyBlockMultiplicative(
        Creature target,
        decimal block,
        ValueProp props,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        bool isStarMoonStrike =
            cardSource is StarMoonStrike
            || cardPlay?.Card is StarMoonStrike;

        if (isStarMoonStrike
            && ReferenceEquals(target, Owner.Creature)
            && Owner.PlayerCombatState.TurnNumber <= 1)
        {
            return 0m;
        }

        return 1m;
    }
}
