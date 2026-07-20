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
using CanAoNative.Pools;

namespace CanAoNative.Cards;

/// <summary>
/// 万邦来朝：获得 1（2）点凤威。
/// 每当你生成星月合击时，对所有敌人施加 1 层虚弱。
/// </summary>
public sealed class WanBangLaiChaoCard : CardModel
{
    public override string PortraitPath => CardModel.MissingPortraitPath;
    protected override string PortraitPngPath => CardModel.MissingPortraitPath;

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<FengWeiPower>(),
        HoverTipFactory.FromCard<StarMoonStrike>(),
        HoverTipFactory.FromPower<WeakPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1)
    ];

    public WanBangLaiChaoCard()
        : base(
            canonicalEnergyCost: 2,
            type: CardType.Power,
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
                "WanBang LaiChao requires a card owner.");

        await FengWeiService.GainPermanent(
            choiceContext,
            owner,
            DynamicVars.Cards.BaseValue,
            this);

        await PowerCmd.Apply<WanBangLaiChaoPower>(
            choiceContext,
            owner.Creature,
            1m,
            owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
