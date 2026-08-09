using CanAoNative.Pools;
using CanAoNative.Rules.Edict;
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
/// 王师亲征：造成 19（25）点伤害。斩杀时，将 1 张【诏令】加入手牌。
/// </summary>
public sealed class WangShiQinZhengCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/wang_shi_qin_zheng.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/wang_shi_qin_zheng.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<EdictCard>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(19m, ValueProp.Move)
    ];

    public WangShiQinZhengCard()
        : base(
            canonicalEnergyCost: 2,
            type: CardType.Attack,
            rarity: CardRarity.Uncommon,
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
                "WangShi QinZheng requires a card owner.");

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        if (!cardPlay.Target.IsDead)
            return;

        await EdictService.Generate(
            choiceContext,
            owner,
            1);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6m);
    }
}
