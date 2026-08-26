using CanAoNative.Pools;
using CanAoNative.Powers;
using CanAoNative.Rules.FengWei;
using CanAoNative.Rules.StarMoon;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Cards;

/// <summary>
/// 复辟：将凤威调整至 0（1），本回合不再受临时凤威的影响。消耗。
/// 每调整 1 点，获得 1 张【星月合击】。
/// </summary>
public sealed class FuBiCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/fu_bi.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/fu_bi.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<FengWeiPower>(),
        HoverTipFactory.FromCard<StarMoonStrike>()
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(0)
    ];

    public FuBiCard()
        : base(
            canonicalEnergyCost: 2,
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
                "FuBi requires a card owner.");

        decimal currentPermanent = FengWeiService.GetPermanentAmount(owner);

        // 调整量按有效凤威计算：（原永久 + 原临时）- 目标值。
        // 必须在施加 FuBiPower 之前读取临时凤威，否则读数会被归零。
        decimal oldEffective =
            currentPermanent + FengWeiService.GetTemporaryAmount(owner);

        decimal target = DynamicVars.Cards.IntValue;

        await FengWeiService.GainPermanent(
            choiceContext,
            owner,
            target - currentPermanent,
            this);

        await PowerCmd.Apply<FuBiPower>(
            choiceContext,
            owner.Creature,
            1m,
            owner.Creature,
            this);

        // 施加 FuBiPower 后有效凤威即目标值（临时凤威本回合不计）。
        int strikes = (int)Math.Abs(oldEffective - target);

        if (strikes > 0)
        {
            await StarMoonService.Generate(
                choiceContext,
                owner,
                strikes,
                owner.Creature,
                this);
        }
    }


    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
