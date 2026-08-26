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
using MegaCrit.Sts2.Core.Models.Powers;

namespace CanAoNative.Cards;

/// <summary>
/// 归隐陨山（先古牌）：3 费。失去 4（2）点凤威，失去 4（2）点力量，
/// 技能牌的费用 -1。
/// 尘封魔典将它授予残傲。
/// </summary>
public sealed class GuiYinYunShanCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/gui_yin_yun_shan.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/gui_yin_yun_shan.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<FengWeiPower>(),
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(4)
    ];

    public GuiYinYunShanCard()
        : base(
            canonicalEnergyCost: 3,
            type: CardType.Power,
            rarity: CardRarity.Ancient,
            targetType: TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        Player owner = Owner
            ?? throw new InvalidOperationException(
                "GuiYin YunShan requires a card owner.");

        await FengWeiService.GainPermanent(
            choiceContext,
            owner,
            -DynamicVars.Cards.IntValue,
            this);

        await PowerCmd.Apply<StrengthPower>(
            choiceContext,
            owner.Creature,
            -DynamicVars.Cards.IntValue,
            owner.Creature,
            this);

        await PowerCmd.Apply<GuiYinYunShanPower>(
            choiceContext,
            owner.Creature,
            1m,
            owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(-2m);
    }
}
