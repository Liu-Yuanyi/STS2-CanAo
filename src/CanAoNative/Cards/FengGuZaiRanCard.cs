using CanAoNative.Rules;
using CanAoNative.Rules.YuHuo;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using CanAoNative.Pools;

namespace CanAoNative.Cards;

/// <summary>
/// 凤骨再燃：从消耗牌堆选择 1 张浴火牌加入手牌，它本回合费用 -1（-2）。消耗。
/// </summary>
public sealed class FengGuZaiRanCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/feng_gu_zai_ran.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/feng_gu_zai_ran.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        CanAoHoverTips.YuHuo
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1)
    ];

    public FengGuZaiRanCard()
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
                "FengGu ZaiRan requires a card owner.");

        ICombatState? combatState =
            CombatState ?? owner.Creature.CombatState;

        if (combatState == null)
            return;

        CardPile exhaustPile = owner.PlayerCombatState.ExhaustPile;

        if (!exhaustPile.Cards.Any(
                card => YuHuoService.HasYuHuo(card, combatState)))
        {
            return;
        }

        CardSelectorPrefs prefs = new(
            SelectionScreenPrompt,
            1);

        CardModel? selected =
            (await CardSelectCmd.FromCombatPile(
                choiceContext,
                exhaustPile,
                owner,
                prefs,
                card => YuHuoService.HasYuHuo(card, combatState)))
            .FirstOrDefault();

        if (selected == null)
            return;

        await CardPileCmd.Add(selected, PileType.Hand);

        selected.EnergyCost.AddThisTurnOrUntilPlayed(
            -DynamicVars.Cards.IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
