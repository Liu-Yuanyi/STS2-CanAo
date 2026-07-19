using CanAoNative.Pools;
using CanAoNative.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace CanAoNative.Cards;

/// <summary>
/// 满目星辰：获得 2 星，这张牌在本局游戏中获得的星永久增加 1。消耗。
/// Mirrors GeneticAlgorithm: the deck instance carries its own growing value.
/// </summary>
public sealed class ManMuXingChenCard : CardModel
{
    private int _currentStars = 2;

    public override string PortraitPath => CardModel.MissingPortraitPath;
    protected override string PortraitPngPath => CardModel.MissingPortraitPath;

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    [SavedProperty]
    public int CurrentStars
    {
        get => _currentStars;
        set
        {
            AssertMutable();
            _currentStars = value;
            DynamicVars["Stars"].BaseValue = value;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("Stars", CurrentStars)
    ];

    public ManMuXingChenCard()
        : base(
            canonicalEnergyCost: 2,
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
                "ManMu XingChen requires a card owner.");

        await PowerCmd.Apply<StarPower>(
            choiceContext,
            owner.Creature,
            CurrentStars,
            owner.Creature,
            this);

        CurrentStars++;
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
