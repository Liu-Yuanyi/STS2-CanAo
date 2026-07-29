using CanAoNative.Powers;
using CanAoNative.Rules;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using CanAoNative.Pools;

namespace CanAoNative.Cards;

/// <summary>
/// 浴火军旗：每次一张牌确实因浴火执行效果后，本回合获得 2（3）点力量。
/// Power amount equals the temporary Strength granted per trigger.
/// </summary>
public sealed class YuHuoBannerCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/yu_huo_banner.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/yu_huo_banner.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        CanAoHoverTips.YuHuo,
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2)
    ];

    public YuHuoBannerCard()
        : base(
            canonicalEnergyCost: 1,
            type: CardType.Power,
            rarity: CardRarity.Uncommon,
            targetType: TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        Player owner = Owner
            ?? throw new InvalidOperationException(
                "YuHuo Banner requires a card owner.");

        await PowerCmd.Apply<YuHuoBannerPower>(
            choiceContext,
            owner.Creature,
            DynamicVars.Cards.IntValue,
            owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
