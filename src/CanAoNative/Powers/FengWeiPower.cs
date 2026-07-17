using CanAoNative.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Powers;

/// <summary>
/// FengWei (凤威): persistent stacking buff that allows negative amounts.
/// Each point of FengWei adds +1 damage and +1 block to StarMoonStrike cards.
/// Implements ModifyDamageAdditive / ModifyBlockAdditive hooks so previews
/// show green/red values and actual combat values match.
/// </summary>
public sealed class FengWeiPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => true;

    public override Decimal ModifyDamageAdditive(
        Creature? target,
        Decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (cardSource is StarMoonStrike)
            return Amount;

        return 0m;
    }

    public override Decimal ModifyBlockAdditive(
        Creature target,
        Decimal block,
        ValueProp props,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (cardSource is StarMoonStrike)
            return Amount;

        return 0m;
    }
}
