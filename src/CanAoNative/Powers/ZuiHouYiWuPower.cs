using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Powers;

/// <summary>
/// 最后一舞：回合结束时（手牌弃掉前），每拥有一张手牌失去 Amount 点生命，然后移除自身。
///
/// BUG FIX (20260721):
/// 1. Timing: AfterSideTurnEndLate fires AFTER FlushPlayerHand discards all
///    non-retained cards, so handCount was always 0 and no HP was ever lost.
///    Switched to BeforeSideTurnEnd which fires before any discard.
/// 2. HP loss: LoseHpInternal bypasses the Command system entirely (no hooks,
///    no network sync, no death processing). Switched to CreatureCmd.Damage
///    with Unblockable|Unpowered flags so the HP loss goes through the proper
///    game pipeline.
/// </summary>
public sealed class ZuiHouYiWuPower : PowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner) || Amount <= 0)
            return;

        // Count hand cards BEFORE discard — this fires in Phase One step 2,
        // well before FlushPlayerHand in Phase Two.
        int handCount =
            Owner.Player.PlayerCombatState.Hand.Cards.Count;

        decimal loss = Amount * handCount;

        // Remove the power before dealing damage so it doesn't retrigger.
        await PowerCmd.Remove(this);

        if (loss > 0m)
        {
            // Use the proper Command pipeline instead of LoseHpInternal.
            // Unblockable: bypasses block (this is HP loss, not damage).
            // Unpowered: not affected by strength/weak.
            await CreatureCmd.Damage(
                choiceContext,
                new[] { Owner },
                loss,
                ValueProp.Unblockable | ValueProp.Unpowered,
                Owner);
        }
    }
}
