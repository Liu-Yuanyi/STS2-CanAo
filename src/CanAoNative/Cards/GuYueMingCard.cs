using CanAoNative.Pools;
using CanAoNative.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Cards;

/// <summary>
/// 孤月明：获得 7（11）月，每拥有一张其他手牌，数值 -2。
/// Uses CalculatedVar to show live moon count on the card, mirroring PreciseCut.
/// </summary>
public sealed class GuYueMingCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/gu_yue_ming.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/gu_yue_ming.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<MoonPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(7m),
        new CalculationExtraVar(2m),
        new CalculatedVar("Moon").WithMultiplier((CardModel card, Creature? _) =>
        {
            int handCount = PileType.Hand.GetPile(card.Owner).Cards.Count;
            if (card.Pile is { } pile && pile.Type == PileType.Hand)
                handCount--;
            return -handCount;
        })
    ];

    public GuYueMingCard()
        : base(
            canonicalEnergyCost: 2,
            type: CardType.Skill,
            rarity: CardRarity.Common,
            targetType: TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        Player owner = Owner
            ?? throw new InvalidOperationException(
                "GuYueMing requires a card owner.");

        int moon = (int)((CalculatedVar)DynamicVars["Moon"])
            .Calculate(null);

        if (moon <= 0)
            return;

        await PowerCmd.Apply<MoonPower>(
            choiceContext,
            owner.Creature,
            moon,
            owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(4m);
    }
}
