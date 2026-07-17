using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Powers;

/// <summary>
/// Probe power used to validate apply/stack/turn-end behavior.
/// </summary>
public sealed class CanAoProbePower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (participants.All(creature => creature != Owner)
            || Amount <= 0)
        {
            return;
        }

        await PowerCmd.ModifyAmount(
            choiceContext,
            this,
            -1m,
            Owner,
            cardSource: null,
            silent: true);
    }
}
