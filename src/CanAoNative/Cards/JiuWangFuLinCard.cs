using CanAoNative.Pools;
using CanAoNative.Powers;
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
/// 旧王复临：造成 16（22）点伤害。若你本回合获得过凤威，本牌费用 -1。
/// </summary>
public sealed class JiuWangFuLinCard : CardModel
{
    private bool _gainedFengWeiThisTurn;

    public override string PortraitPath => "res://images/card_portraits/canao/jiu_wang_fu_lin.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/jiu_wang_fu_lin.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<FengWeiPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(16m, ValueProp.Move)
    ];

    public JiuWangFuLinCard()
        : base(
            canonicalEnergyCost: 2,
            type: CardType.Attack,
            rarity: CardRarity.Common,
            targetType: TargetType.AnyEnemy)
    {
    }

    public override bool TryModifyEnergyCostInCombat(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost)
    {
        if (ReferenceEquals(card, this)
            && _gainedFengWeiThisTurn
            && originalCost > 1m)
        {
            modifiedCost = originalCost - 1m;
            return true;
        }

        return base.TryModifyEnergyCostInCombat(
            card,
            originalCost,
            out modifiedCost);
    }

    public override Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (amount > 0
            && power is (FengWeiPower or TemporaryFengWeiPower)
            && ReferenceEquals(power.Owner, Owner?.Creature))
        {
            _gainedFengWeiThisTurn = true;
        }

        return Task.CompletedTask;
    }

    public override Task AfterSideTurnEndLate(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        // 只在拥有者己方 side-turn 结束时清除；
        // 敌方（或多人局其他玩家）回合结束不能清掉本回合快照。
        if (Owner != null
            && side == Owner.Creature.Side
            && participants.Contains(Owner.Creature))
        {
            _gainedFengWeiThisTurn = false;
        }

        return Task.CompletedTask;
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6m);
    }
}
