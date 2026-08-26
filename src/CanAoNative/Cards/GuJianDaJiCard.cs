using CanAoNative.Pools;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Cards;

/// <summary>
/// 孤剑打击：对所有敌人造成 12（15）点伤害，每拥有一张其他手牌，伤害 -3。
/// Uses CalculatedDamageVar to show live damage on the card, mirroring PreciseCut.
/// </summary>
public sealed class GuJianDaJiCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/gu_jian_da_ji.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/gu_jian_da_ji.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(12m),
        new ExtraDamageVar(3m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card, Creature? _) =>
        {
            int handCount = PileType.Hand.GetPile(card.Owner).Cards.Count;
            if (card.Pile is { } pile && pile.Type == PileType.Hand)
                handCount--;
            return -handCount;
        })
    ];

    public GuJianDaJiCard()
        : base(
            canonicalEnergyCost: 1,
            type: CardType.Attack,
            rarity: CardRarity.Common,
            targetType: TargetType.AllEnemies)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        ICombatState? combatState = CombatState;

        if (combatState == null)
        {
            await DamageCmd.Attack(DynamicVars.CalculatedDamage)
                .WithHitFx("vfx/vfx_attack_blunt")
                .FromCard(this, cardPlay)
                .Execute(choiceContext);
            return;
        }

        await DamageCmd.Attack(DynamicVars.CalculatedDamage)
            .FromCard(this, cardPlay)
            .TargetingAllOpponents(combatState)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(3m);
    }
}
