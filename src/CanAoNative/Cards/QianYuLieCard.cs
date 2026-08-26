using CanAoNative.Pools;
using CanAoNative.Powers;
using CanAoNative.Rules.FengWei;
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
/// 千羽裂：造成 3 点伤害 3（5）次，若你的凤威大于 0，
/// 每拥有 1 点凤威，额外攻击一次。
/// </summary>
public sealed class QianYuLieCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/qian_yu_lie.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/qian_yu_lie.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<FengWeiPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3m, ValueProp.Move),
        new DynamicVar("Hits", 3m)
    ];

    public QianYuLieCard()
        : base(
            canonicalEnergyCost: 1,
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
                "QianYu Lie requires a card owner.");

        decimal fengWei = FengWeiService.GetEffectiveAmount(owner);

        int hitCount =
            DynamicVars["Hits"].IntValue
            + (int)Math.Max(0m, fengWei);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)

            .WithHitFx("vfx/vfx_heavy_blunt")
            .WithHitCount(hitCount)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Hits"].UpgradeValueBy(2m);
    }
}
