using CanAoNative.Pools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Cards;

/// <summary>
/// 防御：获得 5（8）点格挡。
/// </summary>
public sealed class CanAoDefendCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/can_ao_defend.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/can_ao_defend.png";
    public override bool GainsBlock => true;

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override HashSet<CardTag> CanonicalTags =>
    [
        CardTag.Defend
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5m, ValueProp.Move)
    ];

    public CanAoDefendCard()
        : base(
            canonicalEnergyCost: 1,
            type: CardType.Skill,
            rarity: CardRarity.Basic,
            targetType: TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block,
            cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}
