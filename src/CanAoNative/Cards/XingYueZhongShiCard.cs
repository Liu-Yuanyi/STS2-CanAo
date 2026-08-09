using CanAoNative.Pools;
using CanAoNative.Rules.StarMoon;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Cards;

/// <summary>
/// 星月终式：造成 8（11）点伤害。本局游戏每打出过 1 张【星月合击】，重复一次。
/// Uses CalculatedVar to show hit count on the card, mirroring PullFromBelow.
/// </summary>
public sealed class XingYueZhongShiCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/xing_yue_zhong_shi.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/xing_yue_zhong_shi.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<StarMoonStrike>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        new CalculatedVar("ExtraHits").WithMultiplier((CardModel card, Creature? _) =>
        {
            if (card.Owner is Player player)
                return StarMoonService.GetPlayedThisCombat(player);
            return 0;
        })
    ];

    public XingYueZhongShiCard()
        : base(
            canonicalEnergyCost: 2,
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
                "XingYue ZhongShi requires a card owner.");

        int hitCount = 1 + (int)((CalculatedVar)DynamicVars["ExtraHits"])
            .Calculate(cardPlay.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(hitCount)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
