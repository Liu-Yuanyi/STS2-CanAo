using CanAoNative.Pools;
using CanAoNative.Powers;
using CanAoNative.Rules.FengWei;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Cards;

/// <summary>
/// 凤瓦决战：对所有敌人造成 20（30）点伤害。
/// 每击杀一名敌人，获得 1 点凤威。
/// </summary>
public sealed class FengWaJueZhanCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/feng_wa_jue_zhan.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/feng_wa_jue_zhan.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<FengWeiPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(20m, ValueProp.Move)
    ];

    public FengWaJueZhanCard()
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
                "FengWa JueZhan requires a card owner.");

        if (CombatState is not { } combatState)
            return;

        List<Creature> aliveBefore = combatState.HittableEnemies
            .Where(enemy => !enemy.IsDead)
            .ToList();

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .TargetingAllOpponents(combatState)
            .Execute(choiceContext);

        int kills = aliveBefore.Count(enemy => enemy.IsDead);

        if (kills <= 0)
            return;

        await FengWeiService.GainPermanent(
            choiceContext,
            owner,
            kills,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(10m);
    }
}
