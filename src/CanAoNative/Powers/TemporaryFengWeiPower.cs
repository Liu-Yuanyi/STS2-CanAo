using CanAoNative.Cards;
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
/// Turn-only 凤威 adjustment. Positive and negative amounts are both valid.
/// It contributes to Star-Moon Strike in the same additive hook pipeline as
/// permanent FengWeiPower, then removes itself after the owner's complete
/// turn-end pipeline. Removal (rather than restoring to zero with a positive
/// delta) keeps "gained FengWei" listeners from mistaking turn-end cleanup
/// of a negative value for a genuine gain.
/// </summary>
public sealed class TemporaryFengWeiPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => true;

    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        // 复辟生效期间，临时凤威不计入星月合击。
        if (cardSource is StarMoonStrike
            && Owner.GetPower<FuBiPower>() == null)
            return Amount;

        return 0m;
    }

    public override decimal ModifyBlockAdditive(
        Creature target,
        decimal block,
        ValueProp props,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        // 复辟生效期间，临时凤威不计入星月合击。
        if (cardSource is StarMoonStrike
            && Owner.GetPower<FuBiPower>() == null)
            return Amount;

        return 0m;
    }

    public override async Task AfterSideTurnEndLate(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (participants.All(creature => creature != Owner)
            || Amount == 0)
        {
            return;
        }

        await PowerCmd.Remove(this);
    }
}
