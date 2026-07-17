using CanAoNative.Rules.YuHuo;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace CanAoNative.Cards;

/// <summary>
/// 牺牲准备：选择手牌中的 2（升级后 3）张非能力牌，使这些具体
/// 卡牌实例在本回合获得浴火。合法候选不足时自动选择全部候选。
/// </summary>
public sealed class SacrificialPreparationCard : CardModel
{
    public override string PortraitPath => CardModel.MissingPortraitPath;
    protected override string PortraitPngPath => CardModel.MissingPortraitPath;

    public override CardPoolModel Pool =>
        ModelDb.CardPool<ColorlessCardPool>();

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
        bool IsEligible(CardModel candidate) =>
            !ReferenceEquals(candidate, this)
            && candidate.Type != CardType.Power;

        Player owner = Owner
            ?? throw new InvalidOperationException(
                "Sacrificial Preparation requires a card owner.");

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

        ICombatState? combatState =
            CombatState ?? owner.Creature.CombatState;

        if (combatState == null)
            return;

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
