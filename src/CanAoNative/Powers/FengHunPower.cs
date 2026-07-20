using CanAoNative.Rules.Exhaust;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Powers;

/// <summary>
/// 凤魂：每回合前 Amount 次消耗牌时，各抽 2 张牌。
/// 多次释放叠加（Counter），Amount = 释放次数。
/// 每次触发固定抽 2 张（与层数无关，层数只决定触发次数）。
/// </summary>
public sealed class FengHunPower : PowerModel, IAfterCanAoCardExhausted
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task AfterCanAoCardExhausted(
        PlayerChoiceContext choiceContext,
        ExhaustRecord record)
    {
        // Amount = number of exhausts per turn that trigger this effect.
        // Each application adds 1 to Amount (Counter stacking).
        // Draw count is fixed at 2 regardless of stack count.
        if (record.SequenceNumberThisTurn > Amount
            || !ReferenceEquals(record.Owner.Creature, Owner)
            || Amount <= 0)
        {
            return;
        }

        Flash();

        await CardPileCmd.Draw(
            choiceContext,
            2m,
            record.Owner);
    }
}
