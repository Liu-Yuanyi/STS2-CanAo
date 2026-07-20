using CanAoNative.Rules.Exhaust;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Powers;

/// <summary>
/// 瓦魔远征：每当拥有者消耗攻击牌时，对随机敌人造成 Amount 点非攻击伤害。
/// 目标随机走 RunState.Rng.CombatTargets，与游戏原生随机目标同一 RNG 流。
/// </summary>
public sealed class WaMoYuanZhengPower :
    PowerModel,
    IAfterCanAoCardExhausted
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task AfterCanAoCardExhausted(
        PlayerChoiceContext choiceContext,
        ExhaustRecord record)
    {
        ICombatState? combatState = Owner.CombatState;

        if (record.CardType != CardType.Attack
            || !ReferenceEquals(record.Owner.Creature, Owner)
            || Amount <= 0
            || combatState == null)
        {
            return;
        }

        Creature? target =
            record.Owner.RunState.Rng.CombatTargets.NextItem(
                combatState.HittableEnemies);

        if (target == null)
            return;

        Flash();

        await CreatureCmd.Damage(
            choiceContext,
            new[] { target },
            Amount,
            ValueProp.Unpowered,
            Owner);
    }
}
