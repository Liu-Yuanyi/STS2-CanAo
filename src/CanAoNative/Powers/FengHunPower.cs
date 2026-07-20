using CanAoNative.Rules.Exhaust;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Powers;

/// <summary>
/// 凤魂：每回合前 Amount 次有牌被消耗时，各抽 1 张牌。
/// 消耗次序由 ExhaustService 的回合序号判定。
/// </summary>
public sealed class FengHunPower : PowerModel, IAfterCanAoCardExhausted
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task AfterCanAoCardExhausted(
        PlayerChoiceContext choiceContext,
        ExhaustRecord record)
    {
        if (record.SequenceNumberThisTurn > Amount
            || !ReferenceEquals(record.Owner.Creature, Owner)
            || Amount <= 0)
        {
            return;
        }

        Flash();

        await CardPileCmd.Draw(
            choiceContext,
            1m,
            record.Owner);
    }
}
