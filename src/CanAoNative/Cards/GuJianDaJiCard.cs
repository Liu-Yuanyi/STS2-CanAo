using CanAoNative.Pools;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Cards;

/// <summary>
/// 孤剑打击：对所有敌人造成 11（14）点伤害，每拥有一张其他手牌，伤害 -3。
/// </summary>
public sealed class GuJianDaJiCard : CardModel
{
    public override string PortraitPath => CardModel.MissingPortraitPath;
    protected override string PortraitPngPath => CardModel.MissingPortraitPath;

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(11m, ValueProp.Move)
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

        decimal damage = Math.Max(
            0m,
            DynamicVars.Damage.BaseValue
            - 3m * (Owner?.PlayerCombatState.Hand.Cards.Count ?? 0));

        if (combatState == null)
        {
            await DamageCmd.Attack(damage)
                .FromCard(this, cardPlay)
                .Execute(choiceContext);
            return;
        }

        await DamageCmd.Attack(damage)
            .FromCard(this, cardPlay)
            .TargetingAllOpponents(combatState)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
