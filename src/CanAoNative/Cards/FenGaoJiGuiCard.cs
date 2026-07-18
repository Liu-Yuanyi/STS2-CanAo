using CanAoNative.Powers;
using CanAoNative.Rules;
using CanAoNative.Rules.Exhaust;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace CanAoNative.Cards;

/// <summary>
/// 焚膏继晷：消耗不超过 1（2）张手牌。若至少消耗 1 张浴火牌，
/// 获得 1 星与 1 月。消耗。
/// </summary>
public sealed class FenGaoJiGuiCard : CardModel
{
    public override string PortraitPath => CardModel.MissingPortraitPath;
    protected override string PortraitPngPath => CardModel.MissingPortraitPath;

    public override CardPoolModel Pool =>
        ModelDb.CardPool<ColorlessCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StarPower>(),
        HoverTipFactory.FromPower<MoonPower>(),
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

    public FenGaoJiGuiCard()
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
                "FenGao JiGui requires a card owner.");

        int maxSelect = Math.Min(
            DynamicVars.Cards.IntValue,
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

        if (selected.Count == 0)
            return;

        int exhaustedBefore = ExhaustService.GetExhaustedThisTurn(owner);

        foreach (CardModel card in selected)
        {
            await CardCmd.Exhaust(choiceContext, card);
        }

        bool anyYuHuoExhausted = ExhaustService
            .GetRecordsThisTurn(owner)
            .Skip(exhaustedBefore)
            .Any(record => record.HadYuHuo);

        if (!anyYuHuoExhausted)
            return;

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

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
