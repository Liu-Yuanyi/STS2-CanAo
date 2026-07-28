using CanAoNative.Powers;
using CanAoNative.Rules.Edict;
using CanAoNative.Rules.FengWei;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using CanAoNative.Pools;

namespace CanAoNative.Cards;

/// <summary>
/// 承天受命：将 4 张诏令加入手牌。消耗。
/// </summary>
public sealed class ChengTianShouMingCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/cheng_tian_shou_ming.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/cheng_tian_shou_ming.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<EdictCard>()
    ];

    public ChengTianShouMingCard()
        : base(
            canonicalEnergyCost: 3,
            type: CardType.Skill,
            rarity: CardRarity.Rare,
            targetType: TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        Player owner = Owner
            ?? throw new InvalidOperationException(
                "ChengTian ShouMing requires a card owner.");

        await EdictService.Generate(
            choiceContext,
            owner,
            4);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
