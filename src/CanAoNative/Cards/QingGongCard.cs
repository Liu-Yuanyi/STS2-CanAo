using CanAoNative.Rules;
using CanAoNative.Rules.Exhaust;
using CanAoNative.Rules.YuHuo;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using CanAoNative.Pools;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Cards;

/// <summary>
/// 清宫：消耗手牌中所有非浴火技能牌。每消耗 1 张，获得 5（8）点格挡。消耗。
/// </summary>
public sealed class QingGongCard : CardModel
{
    public override string PortraitPath => CardModel.MissingPortraitPath;
    protected override string PortraitPngPath => CardModel.MissingPortraitPath;
    public override bool GainsBlock => true;

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
        new BlockVar(5m, ValueProp.Move)
    ];

    public QingGongCard()
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
                "QingGong requires a card owner.");

        ICombatState? combatState =
            CombatState ?? owner.Creature.CombatState;

        if (combatState == null)
            return;

        // Snapshot first: exhaust hooks may draw or move cards while the
        // batch is resolving.
        List<CardModel> targets = owner.PlayerCombatState.Hand.Cards
            .Where(card =>
                card.Type == CardType.Skill
                && !YuHuoService.HasYuHuo(card, combatState))
            .ToList();

        if (targets.Count == 0)
            return;

        int exhaustedBefore = ExhaustService.GetExhaustedThisTurn(owner);

        foreach (CardModel card in targets)
        {
            await CardCmd.Exhaust(choiceContext, card);
        }

        int exhausted =
            ExhaustService.GetExhaustedThisTurn(owner) - exhaustedBefore;

        if (exhausted <= 0)
            return;

        await CreatureCmd.GainBlock(
            owner.Creature,
            DynamicVars.Block.BaseValue * exhausted,
            ValueProp.Move,
            cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}
