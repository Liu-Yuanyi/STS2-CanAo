using CanAoNative.Pools;
using CanAoNative.Powers;
using CanAoNative.Rules.StarMoon;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Cards;

/// <summary>
/// 碎月一击：失去所有月，每失去 1 月，造成 11（15）点伤害。
/// </summary>
public sealed class SuiYueYiJiCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/sui_yue_yi_ji.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/sui_yue_yi_ji.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<MoonPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(11m, ValueProp.Move)
    ];

    public SuiYueYiJiCard()
        : base(
            canonicalEnergyCost: 0,
            type: CardType.Attack,
            rarity: CardRarity.Rare,
            targetType: TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        Player owner = Owner
            ?? throw new InvalidOperationException(
                "SuiYue YiJi requires a card owner.");

        decimal moonLost = await StarMoonService.LoseMoon(
            choiceContext,
            owner,
            decimal.MaxValue,
            this);

        if (moonLost <= 0m)
            return;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount((int)moonLost)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}
