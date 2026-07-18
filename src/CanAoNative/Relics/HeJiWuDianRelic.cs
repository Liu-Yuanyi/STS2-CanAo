using CanAoNative.Cards;
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
/// 合击武典：每打出 4 张星月合击，下一张星月合击的效果翻倍。
/// Mirrors the native Pen Nib pattern: count plays, then double the next
/// strike's damage and block through the modifier hooks at resolution time.
/// </summary>
public sealed class HeJiWuDianRelic : RelicModel
{
    private int _strikesPlayed;
    private bool _armed;

    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override bool ShowCounter => true;
    public override int DisplayAmount => _strikesPlayed;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(4)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<StarMoonStrike>()
    ];

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card is not StarMoonStrike
            || cardPlay.Card.Owner != Owner
            || _armed)
        {
            return Task.CompletedTask;
        }

        _strikesPlayed++;

        if (_strikesPlayed >= DynamicVars.Cards.IntValue)
        {
            _strikesPlayed = 0;
            _armed = true;
        }

        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (!_armed
            || !props.IsPoweredAttack()
            || cardSource is not StarMoonStrike
            || dealer != Owner.Creature)
        {
            return 1m;
        }

        return 2m;
    }

    public override decimal ModifyBlockAdditive(
        Creature target,
        decimal block,
        ValueProp props,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (!_armed
            || cardSource is not StarMoonStrike
            || !ReferenceEquals(target, Owner.Creature))
        {
            return block;
        }

        return block * 2m;
    }

    public override Task AfterCardPlayedLate(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (_armed
            && cardPlay.Card is StarMoonStrike
            && cardPlay.Card.Owner == Owner)
        {
            _armed = false;
            InvokeDisplayAmountChanged();
        }

        return Task.CompletedTask;
    }

    public override Task BeforeCombatStart()
    {
        _strikesPlayed = 0;
        _armed = false;
        return Task.CompletedTask;
    }
}
