using CanAoNative.Pools;
using CanAoNative.Powers;
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
/// 观星问月：选择：获得 2 星，或获得 2 月。
/// 若没有因本牌生成【星月合击】，抽 1（2）张牌。
/// </summary>
public sealed class GuanXingWenYueCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/guan_xing_wen_yue.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/guan_xing_wen_yue.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StarPower>(),
        HoverTipFactory.FromPower<MoonPower>(),
        HoverTipFactory.FromCard<StarMoonStrike>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1)
    ];

    public GuanXingWenYueCard()
        : base(
            canonicalEnergyCost: 1,
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
                "GuanXing WenYue requires a card owner.");

        if (CombatState is not { } combatState)
            return;

        CardModel guanXing =
            combatState.CreateCard<GuanXingOptionCard>(owner);
        CardModel wenYue =
            combatState.CreateCard<WenYueOptionCard>(owner);

        int generatedBefore =
            StarMoonService.GetGeneratedThisTurn(owner);

        CardModel? choice =
            await CardSelectCmd.FromChooseACardScreen(
                choiceContext,
                [guanXing, wenYue],
                owner,
                canSkip: false);

        if (choice != null)
        {
            if (choice is GuanXingOptionCard)
            {
                await PowerCmd.Apply<StarPower>(
                    choiceContext,
                    owner.Creature,
                    2m,
                    owner.Creature,
                    this);
            }
            else
            {
                await PowerCmd.Apply<MoonPower>(
                    choiceContext,
                    owner.Creature,
                    2m,
                    owner.Creature,
                    this);
            }
        }

        if (StarMoonService.GetGeneratedThisTurn(owner)
            != generatedBefore)
        {
            return;
        }

        await CardPileCmd.Draw(
            choiceContext,
            DynamicVars.Cards.BaseValue,
            owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
