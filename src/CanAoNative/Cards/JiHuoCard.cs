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
/// 祭火：消耗不超过 2（3）张手牌。消耗。
/// </summary>
public sealed class JiHuoCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/ji_huo.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/ji_huo.png";

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

    public JiHuoCard()
        : base(
            canonicalEnergyCost: 0,
            type: CardType.Skill,
            rarity: CardRarity.Basic,
            targetType: TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        Player owner = Owner
            ?? throw new InvalidOperationException(
                "JiHuo requires a card owner.");

        int maxSelect = Math.Min(
            DynamicVars.Cards.IntValue,
            owner.PlayerCombatState.Hand.Cards.Count);

        if (maxSelect <= 0)
            return;

        CardSelectorPrefs prefs = new(
            SelectionScreenPrompt,
            0,
            maxSelect);

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
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
