using CanAoNative.Pools;
using CanAoNative.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Cards;

/// <summary>
/// 孤月明：获得 7（11）月，每拥有一张其他手牌，数值 -2。
/// </summary>
public sealed class GuYueMingCard : CardModel
{
    public override string PortraitPath => CardModel.MissingPortraitPath;
    protected override string PortraitPngPath => CardModel.MissingPortraitPath;

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<MoonPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(7)
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

        int moon = Math.Max(
            0,
            DynamicVars.Cards.IntValue
            - 2 * owner.PlayerCombatState.Hand.Cards.Count);

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
        DynamicVars.Cards.UpgradeValueBy(4m);
    }
}
