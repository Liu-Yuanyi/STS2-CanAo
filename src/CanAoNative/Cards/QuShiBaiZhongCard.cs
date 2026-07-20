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

namespace CanAoNative.Cards;

/// <summary>
/// 取势败中：失去 1 点凤威，获得 1 费，抽 1 张牌。消耗。
/// 升级后获得保留。
/// </summary>
public sealed class QuShiBaiZhongCard : CardModel
{
    public override string PortraitPath => CardModel.MissingPortraitPath;
    protected override string PortraitPngPath => CardModel.MissingPortraitPath;

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<FengWeiPower>()
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1),
        new CardsVar(1)
    ];

    public QuShiBaiZhongCard()
        : base(
            canonicalEnergyCost: 0,
            type: CardType.Skill,
            rarity: CardRarity.Common,
            targetType: TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        Player owner = Owner
            ?? throw new InvalidOperationException(
                "QuShi BaiZhong requires a card owner.");

        await FengWeiService.GainPermanent(
            choiceContext,
            owner,
            -1m,
            this);

        await PlayerCmd.GainEnergy(
            DynamicVars.Energy.IntValue,
            owner);

        await CardPileCmd.Draw(
            choiceContext,
            DynamicVars.Cards.BaseValue,
            owner);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
