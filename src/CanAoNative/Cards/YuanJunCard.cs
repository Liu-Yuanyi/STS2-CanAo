using CanAoNative.Pools;
using CanAoNative.Rules;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Cards;

/// <summary>
/// 援军：将 3（4）张带有浴火的打击加入手牌。消耗。
/// 打击为 YuanJunStrikeCard，固有浴火，无需逐张授予。
/// </summary>
public sealed class YuanJunCard : CardModel
{
    public override string PortraitPath => CardModel.MissingPortraitPath;
    protected override string PortraitPngPath => CardModel.MissingPortraitPath;

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        CanAoHoverTips.YuHuo,
        HoverTipFactory.FromCard<YuanJunStrikeCard>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3)
    ];

    public YuanJunCard()
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
                "YuanJun requires a card owner.");

        if (CombatState is not { } combatState)
            return;

        for (int i = 0; i < DynamicVars.Cards.IntValue; i++)
        {
            YuanJunStrikeCard strike =
                combatState.CreateCard<YuanJunStrikeCard>(owner);

            CardCmd.Upgrade(strike);

            await CardPileCmd.AddGeneratedCardToCombat(
                strike,
                PileType.Hand,
                owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
