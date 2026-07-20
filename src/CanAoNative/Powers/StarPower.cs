using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Powers;

/// <summary>
/// 星：在拥有者回合结束时清除。守缺存在时保留至多守缺层数点。
/// </summary>
public sealed class StarPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (participants.All(creature => creature != Owner))
            return;

        // 守缺：回合末清除时保留至多 ShouQuePower.Amount 点星。
        decimal removal = Amount;
        ShouQuePower? shouQue = Owner.GetPower<ShouQuePower>();

        if (shouQue != null)
            removal = Math.Max(0m, Amount - shouQue.Amount);

        if (removal != 0)
        {
            await PowerCmd.ModifyAmount(
                choiceContext,
                this,
                -removal,
                Owner,
                cardSource: null,
                silent: true);
        }
    }
}
