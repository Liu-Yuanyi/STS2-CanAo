using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Powers;

/// <summary>
/// 最后一舞：回合结束时，每拥有一张手牌，失去 Amount 点生命，然后移除。
/// </summary>
public sealed class ZuiHouYiWuPower : PowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnEndLate(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner) || Amount <= 0)
            return;

        int handCount =
            Owner.Player.PlayerCombatState.Hand.Cards.Count;

        decimal loss = Amount * handCount;

        await PowerCmd.Remove(this);

        if (loss > 0m)
        {
            Owner.LoseHpInternal(
                loss,
                ValueProp.Unblockable | ValueProp.Unpowered);
        }
    }
}
