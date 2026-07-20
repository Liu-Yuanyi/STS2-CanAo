using CanAoNative.Pools;
using CanAoNative.Rules.Exhaust;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Cards;

/// <summary>
/// 燃羽突袭：造成 5（6）点伤害。本回合每消耗过一张牌，伤害便增加 3（4）。
/// </summary>
public sealed class RanYuTuXiCard : CardModel
{
    public override string PortraitPath => CardModel.MissingPortraitPath;
    protected override string PortraitPngPath => CardModel.MissingPortraitPath;

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new CardsVar(3)
    ];

    public RanYuTuXiCard()
        : base(
            canonicalEnergyCost: 0,
            type: CardType.Attack,
            rarity: CardRarity.Common,
            targetType: TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        Player owner = Owner
            ?? throw new InvalidOperationException(
                "RanYu TuXi requires a card owner.");

        decimal damage =
            DynamicVars.Damage.BaseValue
            + DynamicVars.Cards.BaseValue
              * ExhaustService.GetExhaustedThisTurn(owner);

        await DamageCmd.Attack(damage)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
