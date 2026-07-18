using CanAoNative.Powers;
using MegaCrit.Sts2.Core.CardSelection;
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
/// 诏令：0 费衍生技能。保留，消耗。
/// 消耗 1 张手牌：攻击牌 → 获得 1（2）星；技能牌 → 获得 1（2）月；
/// 能力牌 → 获得 1（2）星与 1（2）月。
/// Not in the loot pool, but uses CanAoCardPool for visuals.
/// </summary>
public sealed class EdictCard : CardModel
{
    public override string PortraitPath => CardModel.MissingPortraitPath;
    protected override string PortraitPngPath => CardModel.MissingPortraitPath;

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Retain,
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StarPower>(),
        HoverTipFactory.FromPower<MoonPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1)
    ];

    public EdictCard()
        : base(
            canonicalEnergyCost: 0,
            type: CardType.Skill,
            rarity: CardRarity.Token,
            targetType: TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        Player owner = Owner
            ?? throw new InvalidOperationException(
                "Edict requires a card owner.");

        if (owner.PlayerCombatState.Hand.Cards.Count == 0)
            return;

        CardSelectorPrefs prefs = new(
            SelectionScreenPrompt,
            1);

        CardModel? selected =
            (await CardSelectCmd.FromHand(
                choiceContext,
                owner,
                prefs,
                null,
                this))
            .FirstOrDefault();

        if (selected == null)
            return;

        CardType selectedType = selected.Type;
        int amount = DynamicVars.Cards.IntValue;

        await CardCmd.Exhaust(choiceContext, selected);

        if (selectedType is CardType.Attack or CardType.Power)
        {
            await PowerCmd.Apply<StarPower>(
                choiceContext,
                owner.Creature,
                amount,
                owner.Creature,
                this);
        }

        if (selectedType is CardType.Skill or CardType.Power)
        {
            await PowerCmd.Apply<MoonPower>(
                choiceContext,
                owner.Creature,
                amount,
                owner.Creature,
                this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
