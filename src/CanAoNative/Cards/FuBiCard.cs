using CanAoNative.Pools;
using CanAoNative.Powers;
using CanAoNative.Rules.FengWei;
using CanAoNative.Rules.StarMoon;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Cards;

/// <summary>
/// 复辟：将凤威调整至 0（1），本回合不再受临时凤威的影响。
/// 每调整 1 点，获得 1 张【星月合击】。
/// </summary>
public sealed class FuBiCard : CardModel
{
    public override string PortraitPath => CardModel.MissingPortraitPath;
    protected override string PortraitPngPath => CardModel.MissingPortraitPath;

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(0)
    ];

    public FuBiCard()
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
                "FuBi requires a card owner.");

        decimal current = FengWeiService.GetPermanentAmount(owner);
        decimal target = DynamicVars.Cards.IntValue;
        decimal diff = target - current;

        await FengWeiService.GainPermanent(
            choiceContext,
            owner,
            diff,
            this);

        await PowerCmd.Apply<FuBiPower>(
            choiceContext,
            owner.Creature,
            1m,
            owner.Creature,
            this);

        int strikes = (int)Math.Abs(diff);

        if (strikes > 0)
        {
            await StarMoonService.Generate(
                choiceContext,
                owner,
                strikes,
                owner.Creature,
                this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
