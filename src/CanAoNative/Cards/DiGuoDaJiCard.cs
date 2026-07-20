using CanAoNative.Pools;
using CanAoNative.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Cards;

/// <summary>
/// 帝国大祭：消耗最多 X（X+1）张手牌。每消耗 1 张，
/// 失去 1 点生命，获得 1 星和 1 月。
/// </summary>
public sealed class DiGuoDaJiCard : CardModel
{
    public override string PortraitPath => CardModel.MissingPortraitPath;
    protected override string PortraitPngPath => CardModel.MissingPortraitPath;

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StarPower>(),
        HoverTipFactory.FromPower<MoonPower>()
    ];

    protected override bool HasEnergyCostX => true;

    public DiGuoDaJiCard()
        : base(
            canonicalEnergyCost: 0,
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
                "DiGuo DaJi requires a card owner.");

        int maxSelect = ResolveEnergyXValue();

        if (IsUpgraded)
            maxSelect++;

        maxSelect = Math.Min(
            maxSelect,
            owner.PlayerCombatState.Hand.Cards.Count);

        if (maxSelect <= 0)
            return;

        CardSelectorPrefs prefs = new(
            SelectionScreenPrompt,
            0,
            maxSelect);

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

            owner.Creature.LoseHpInternal(
                1m,
                ValueProp.Unblockable | ValueProp.Unpowered);

            await PowerCmd.Apply<StarPower>(
                choiceContext,
                owner.Creature,
                1m,
                owner.Creature,
                this);

            await PowerCmd.Apply<MoonPower>(
                choiceContext,
                owner.Creature,
                1m,
                owner.Creature,
                this);
        }
    }
}
