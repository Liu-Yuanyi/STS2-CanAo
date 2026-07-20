using CanAoNative.Rules.Exhaust;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Powers;

/// <summary>
/// 凤魂：每回合第一次消耗牌时，抽 2 张牌。不可叠加。
/// </summary>
public sealed class FengHunPower : PowerModel, IAfterCanAoCardExhausted
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public async Task AfterCanAoCardExhausted(
        PlayerChoiceContext choiceContext,
        ExhaustRecord record)
    {
        if (record.SequenceNumberThisTurn != 1
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
