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
/// 合击武典：你的每第 4 张星月合击效果翻倍。
/// Mirrors the native Pen Nib pattern: the doubling condition is computed
/// from the play counter, so both the card-face preview and the resolution
/// see the same doubled values.
/// </summary>
public sealed class HeJiWuDianRelic : RelicModel
{
    private int _strikesPlayed;

    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override bool ShowCounter => true;

    public override int DisplayAmount =>
        _strikesPlayed % DynamicVars.Cards.IntValue;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(4)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<StarMoonStrike>()
    ];

    /// <summary>
    /// True while the next Star-Moon Strike played will be the Nth one.
    /// The counter advances in AfterCardPlayed, after the card's effects
    /// have resolved, so the Nth strike itself resolves doubled.
    /// </summary>
    private bool IsNextStrikeDoubled =>
        _strikesPlayed % DynamicVars.Cards.IntValue
        == DynamicVars.Cards.IntValue - 1;

    public override Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (cardPlay.Card is StarMoonStrike
            && cardPlay.Card.Owner == Owner)
        {
            _strikesPlayed++;
            InvokeDisplayAmountChanged();
        }

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
        if (!IsNextStrikeDoubled
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
        bool isStarMoonStrike =
            cardSource is StarMoonStrike
            || cardPlay?.Card is StarMoonStrike;

        if (!IsNextStrikeDoubled
            || !isStarMoonStrike
            || !ReferenceEquals(target, Owner.Creature))
        {
            return block;
        }

        return block * 2m;
    }

    public override Task BeforeCombatStart()
    {
        _strikesPlayed = 0;
        return Task.CompletedTask;
    }
}
