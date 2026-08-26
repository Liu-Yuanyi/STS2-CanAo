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
/// 援军：将 3 张带有浴火的火刃加入手牌。消耗。
/// 升级后费用降为 0。（2026-08-01 起由打击令牌改为火刃，打击（援军版）卡牌已删除；
/// 2026-08-15 悬浮预览修正为未升级版火刃，与实际产出一致）
/// </summary>
public sealed class YuanJunCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/yuan_jun.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/yuan_jun.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        CanAoHoverTips.YuHuo,
        HoverTipFactory.FromCard<HuoRenCard>()
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
            HuoRenCard fireBlade =
                combatState.CreateCard<HuoRenCard>(owner);

            await CardPileCmd.AddGeneratedCardToCombat(
                fireBlade,
                PileType.Hand,
                owner);
        }
    }


    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
