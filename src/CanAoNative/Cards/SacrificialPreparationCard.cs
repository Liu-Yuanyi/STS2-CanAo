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
using MegaCrit.Sts2.Core.Models.Powers;
using CanAoNative.Pools;

namespace CanAoNative.Cards;

/// <summary>
/// 牺牲准备：获得 1 点力量。选择手牌中的 2（3）张没有浴火的
/// 非能力牌，使其本回合获得浴火。消耗。合法候选不足时自动选择全部候选。
/// </summary>
public sealed class SacrificialPreparationCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/sacrificial_preparation.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/sacrificial_preparation.png";

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
        new CardsVar(2)
    ];

    public SacrificialPreparationCard()
        : base(
            canonicalEnergyCost: 0,
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
                "Sacrificial Preparation requires a card owner.");

        ICombatState? combatState =
            CombatState ?? owner.Creature.CombatState;

        if (combatState == null)
            return;

        // Gain 1 Strength unconditionally as a baseline.
        await PowerCmd.Apply<StrengthPower>(
            choiceContext,
            owner.Creature,
            1m,
            owner.Creature,
            this);

        // 已有浴火的牌不能再选（参考原生响指/雕琢打击的同类过滤）。
        bool IsEligible(CardModel candidate) =>
            !ReferenceEquals(candidate, this)
            && candidate.Type != CardType.Power
            && !YuHuoService.HasYuHuo(candidate, combatState);

        int eligibleCount =
            owner.PlayerCombatState.Hand.Cards.Count(IsEligible);

        int selectCount =
            Math.Min(DynamicVars.Cards.IntValue, eligibleCount);

        if (selectCount <= 0)
            return;

        CardSelectorPrefs prefs = new(
            SelectionScreenPrompt,
            selectCount)
        {
            PretendCardsCanBePlayed = true
        };

        List<CardModel> selected =
            (await CardSelectCmd.FromHand(
                choiceContext,
                owner,
                prefs,
                IsEligible,
                this))
            .ToList();

        foreach (CardModel card in selected)
        {
            YuHuoService.GrantUntilTurnEnd(
                card,
                owner,
                combatState);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
