using CanAoNative.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace CanAoNative.Cards;

/// <summary>
/// 星月王冠：每回合第一次获得凤威时，获得 1 张星月合击。
/// 升级后改为获得星月合击+。
/// </summary>
public sealed class XingYueWangGuanCard : CardModel
{
    public override string PortraitPath => CardModel.MissingPortraitPath;
    protected override string PortraitPngPath => CardModel.MissingPortraitPath;

    public override CardPoolModel Pool =>
        ModelDb.CardPool<ColorlessCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<FengWeiPower>(),
        HoverTipFactory.FromCard<StarMoonStrike>(IsUpgraded)
    ];

    public XingYueWangGuanCard()
        : base(
            canonicalEnergyCost: 1,
            type: CardType.Power,
            rarity: CardRarity.Rare,
            targetType: TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        XingYueWangGuanPower? power =
            await PowerCmd.Apply<XingYueWangGuanPower>(
                choiceContext,
                Owner.Creature,
                1m,
                Owner.Creature,
                this);

        if (power != null)
            power.UpgradedGeneration = IsUpgraded;
    }
}
