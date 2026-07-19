using CanAoNative.Pools;
using CanAoNative.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Cards;

/// <summary>
/// 尘土之战：失去所有星和月，对所有敌人造成 15（20）点伤害。
/// 每失去 1 点星或月，额外造成 5（7）点伤害。
/// </summary>
public sealed class ChenTuZhiZhanCard : CardModel
{
    public override string PortraitPath => CardModel.MissingPortraitPath;
    protected override string PortraitPngPath => CardModel.MissingPortraitPath;

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(15m, ValueProp.Move),
        new CardsVar(5)
    ];

    public ChenTuZhiZhanCard()
        : base(
            canonicalEnergyCost: 2,
            type: CardType.Attack,
            rarity: CardRarity.Rare,
            targetType: TargetType.AllEnemies)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        Player owner = Owner
            ?? throw new InvalidOperationException(
                "ChenTu ZhiZhan requires a card owner.");

        if (CombatState is not { } combatState)
            return;

        decimal stars = 0m;
        decimal moons = 0m;

        if (owner.Creature.GetPower<StarPower>() is
            { Amount: > 0 } starPower)
        {
            stars = starPower.Amount;

            await PowerCmd.ModifyAmount(
                choiceContext,
                starPower,
                -stars,
                owner.Creature,
                this);
        }

        if (owner.Creature.GetPower<MoonPower>() is
            { Amount: > 0 } moonPower)
        {
            moons = moonPower.Amount;

            await PowerCmd.ModifyAmount(
                choiceContext,
                moonPower,
                -moons,
                owner.Creature,
                this);
        }

        decimal damage =
            DynamicVars.Damage.BaseValue
            + DynamicVars.Cards.BaseValue * (stars + moons);

        await DamageCmd.Attack(damage)
            .FromCard(this, cardPlay)
            .TargetingAllOpponents(combatState)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
        DynamicVars.Cards.UpgradeValueBy(2m);
    }
}
