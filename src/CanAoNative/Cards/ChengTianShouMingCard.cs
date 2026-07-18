using CanAoNative.Powers;
using CanAoNative.Rules.Edict;
using CanAoNative.Rules.FengWei;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace CanAoNative.Cards;

/// <summary>
/// 承天受命：获得 3 点凤威。将 2 张诏令加入手牌。消耗。
/// </summary>
public sealed class ChengTianShouMingCard : CardModel
{
    public override string PortraitPath => CardModel.MissingPortraitPath;
    protected override string PortraitPngPath => CardModel.MissingPortraitPath;

    public override CardPoolModel Pool =>
        ModelDb.CardPool<ColorlessCardPool>();

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<EdictCard>(),
        HoverTipFactory.FromPower<FengWeiPower>()
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

        await FengWeiService.GainPermanent(
            choiceContext,
            owner,
            3m,
            this);

        await EdictService.Generate(
            choiceContext,
            owner,
            2);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
