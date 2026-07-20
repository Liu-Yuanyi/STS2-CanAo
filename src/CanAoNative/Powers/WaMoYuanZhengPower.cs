using CanAoNative.Rules.Exhaust;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Powers;

/// <summary>
/// 远征：每当拥有者消耗攻击牌时，获得 Amount 点格挡。
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
        if (record.CardType != CardType.Attack
            || !ReferenceEquals(record.Owner.Creature, Owner)
            || Amount <= 0)
        {
            return;
        }

        Flash();

        await CreatureCmd.GainBlock(
            Owner,
            Amount,
            ValueProp.Unpowered,
            (CardPlay?)null);
    }
}
