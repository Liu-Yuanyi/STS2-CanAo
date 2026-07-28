using CanAoNative.Pools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Cards;

/// <summary>
/// 打击：造成 6（9）点伤害。
/// </summary>
public sealed class CanAoStrikeCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/can_ao_strike.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/can_ao_strike.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override HashSet<CardTag> CanonicalTags =>
    [
        CardTag.Strike
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move)
    ];

    public CanAoStrikeCard()
        : base(
            canonicalEnergyCost: 1,
            type: CardType.Attack,
            rarity: CardRarity.Basic,
            targetType: TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
