using CanAoNative.Pools;
using CanAoNative.Powers;
using CanAoNative.Rules.YuHuo;
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
/// 淬火：浴火。在本回合获得 2（3）点力量。将一张此牌的复制品加入手牌。
/// </summary>
public sealed class CuiHuoCard : CardModel, IIntrinsicYuHuo
{
    public override string PortraitPath => CardModel.MissingPortraitPath;
    protected override string PortraitPngPath => CardModel.MissingPortraitPath;

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    public bool HasIntrinsicYuHuo => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2)
    ];

    public CuiHuoCard()
        : base(
            canonicalEnergyCost: 1,
            type: CardType.Skill,
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
                "CuiHuo requires a card owner.");

        if (CombatState is not { } combatState)
            return;

        await PowerCmd.Apply<CuiHuoTemporaryStrengthPower>(
            choiceContext,
            owner.Creature,
            DynamicVars.Cards.IntValue,
            owner.Creature,
            this);

        CuiHuoCard copy = combatState.CreateCard<CuiHuoCard>(owner);

        if (IsUpgraded)
            CardCmd.Upgrade(copy);

        await CardPileCmd.AddGeneratedCardToCombat(
            copy,
            PileType.Hand,
            owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
