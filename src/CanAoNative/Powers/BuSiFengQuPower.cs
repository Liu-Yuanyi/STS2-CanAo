using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Powers;

/// <summary>
/// 不死凤躯：Amount 回合内，下一次将受到致命伤害时改为保留 1 点生命。
/// </summary>
public sealed class BuSiFengQuPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldDieLate(Creature creature)
    {
        return !ReferenceEquals(creature, Owner);
    }

    public override async Task AfterPreventingDeath(Creature creature)
    {
        Flash();

        if (creature.CurrentHp < 1)
            await CreatureCmd.Heal(creature, 1 - creature.CurrentHp);

        await PowerCmd.Remove(this);
    }

    public override async Task AfterSideTurnEndLate(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner) || Amount <= 0)
            return;

        await PowerCmd.ModifyAmount(
            choiceContext,
            this,
            -1m,
            Owner,
            cardSource: null,
            silent: true);

        if (Amount <= 0)
            await PowerCmd.Remove(this);
    }
}
