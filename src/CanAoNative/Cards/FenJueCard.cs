using CanAoNative.Pools;
using CanAoNative.Rules.YuHuo;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Cards;

/// <summary>
/// 焚诀（先古牌）：抽 2（4）张牌，然后消耗任意张手牌。消耗。
/// 古老牙齿把祭火替换为这张牌。
/// </summary>
public sealed class FenJueCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/fen_jue.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/fen_jue.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2)
    ];

    public FenJueCard()
        : base(
            canonicalEnergyCost: 0,
            type: CardType.Skill,
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
                "FenJue requires a card owner.");

        await CardPileCmd.Draw(
            choiceContext,
            DynamicVars.Cards.BaseValue,
            owner);

        int handCount = owner.PlayerCombatState.Hand.Cards.Count;

        if (handCount <= 0)
            return;

        CardSelectorPrefs prefs = new(
            SelectionScreenPrompt,
            0,
            handCount);

        // 选择消耗目标时，浴火牌金色高亮（同原生奇巧弃牌高亮）。
        if ((CombatState ?? owner.Creature.CombatState)
            is { } combatState)
        {
            prefs.ShouldGlowGold =
                card => YuHuoService.HasYuHuo(card, combatState);
        }

        List<CardModel> selected =
            (await CardSelectCmd.FromHand(
                choiceContext,
                owner,
                prefs,
                null,
                this))
            .ToList();

        foreach (CardModel card in selected)
        {
            await CardCmd.Exhaust(choiceContext, card);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(2m);
    }
}
