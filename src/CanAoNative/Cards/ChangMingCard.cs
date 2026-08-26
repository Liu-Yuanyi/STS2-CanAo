using CanAoNative.Pools;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Cards;

/// <summary>
/// 长鸣（v12）：获得 6（7）点格挡。将 1 张牌从消耗牌堆移至抽牌堆。
    /// 消耗。升级后格挡 +1 且不消耗。
/// </summary>
public sealed class ChangMingCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/chang_ming.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/chang_ming.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(6m, ValueProp.Move),
        new CardsVar(1)
    ];

    public ChangMingCard()
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
                "ChangMing requires a card owner.");

        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block,
            cardPlay);

        CardPile exhaustPile = owner.PlayerCombatState.ExhaustPile;

        if (exhaustPile.IsEmpty)
            return;

        CardSelectorPrefs prefs = new(
            SelectionScreenPrompt,
            0,
            DynamicVars.Cards.IntValue);

        List<CardModel> selected =
            (await CardSelectCmd.FromCombatPile(
                choiceContext,
                exhaustPile,
                owner,
                prefs))
            .ToList();

        foreach (CardModel card in selected)
        {
            await CardPileCmd.Add(card, PileType.Draw);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(1m);
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
