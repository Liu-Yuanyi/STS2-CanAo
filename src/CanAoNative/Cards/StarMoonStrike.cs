using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using CanAoNative.Pools;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Cards;

/// <summary>
/// Star-Moon Strike (星月合击): 0-cost derived attack.
/// Deals 5(7) damage and gains 4(6) block. Ethereal. Exhaust.
/// Each point of FengWei adds +1 damage and +1 block (min 0).
/// Not in the loot pool, but uses CanAoCardPool for visuals.
/// </summary>
public sealed class StarMoonStrike : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/star_moon_strike.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/star_moon_strike.png";
    public override bool GainsBlock => true;

    /// <summary>Use CanAoCardPool for visual rendering.</summary>
    public override CardPoolModel Pool => ModelDb.CardPool<CanAoCardPool>();

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Ethereal,
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new BlockVar(4m, ValueProp.Move)
    ];

    public StarMoonStrike()
        : base(
            canonicalEnergyCost: 0,
            type: CardType.Attack,
            rarity: CardRarity.Token,
            targetType: TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // FengWei modifier is applied automatically via hooks on FengWeiPower.
        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block,
            cardPlay);

        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars.Block.UpgradeValueBy(2m);
    }
}
