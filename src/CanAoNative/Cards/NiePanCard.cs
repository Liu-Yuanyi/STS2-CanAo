using CanAoNative.Pools;
using CanAoNative.Rules;
using CanAoNative.Rules.YuHuo;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Cards;

/// <summary>
/// 涅槃：消耗手牌中的所有浴火牌。每消耗 1 张，获得 7 格挡。消耗。
/// Only the cards in hand at play time are consumed; anything the 浴火
/// triggers draw or generate afterwards is left alone.
/// </summary>
public sealed class NiePanCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/nie_pan.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/nie_pan.png";
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
        new BlockVar(7m, ValueProp.Move)
    ];

    public NiePanCard()
        : base(
            canonicalEnergyCost: 3,
            type: CardType.Skill,
            rarity: CardRarity.Rare,
            targetType: TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        Player owner = Owner
            ?? throw new InvalidOperationException(
                "NiePan requires a card owner.");

        if (CombatState is not { } combatState)
            return;

        // Snapshot first: only the 浴火 cards present in hand right now.
        List<CardModel> targets = owner.PlayerCombatState.Hand.Cards
            .Where(card => YuHuoService.HasYuHuo(card, combatState))
            .ToList();

        foreach (CardModel card in targets)
        {
            await CardCmd.Exhaust(choiceContext, card);

            await CreatureCmd.GainBlock(
                owner.Creature,
                DynamicVars.Block,
                cardPlay);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
