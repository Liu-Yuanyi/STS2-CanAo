using CanAoNative.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Powers;

/// <summary>
/// 不堕：星月合击不受小于 0 的凤威影响。
/// 凤威对星月合击的修正由 FengWeiPower 与 TemporaryFengWeiPower 的
/// 增量 Hook 叠加而成；本 Power 追加一个非负补偿量，
/// 使合计修正等于 max(0, 永久凤威 + 临时凤威)。
/// 效果与层数无关，重复打出不叠加（StackType.Single，壁垒式）。
/// </summary>
public sealed class BuDuoPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (cardSource is StarMoonStrike)
            return NegativeFengWeiOffset();

        return 0m;
    }

    public override decimal ModifyBlockAdditive(
        Creature target,
        decimal block,
        ValueProp props,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (cardSource is StarMoonStrike)
            return NegativeFengWeiOffset();

        return 0m;
    }

    /// <summary>
    /// Compensation that lifts the summed FengWei modifier up to zero.
    /// Mirrors FengWeiService semantics: permanent always counts, temporary
    /// counts only while 复辟 is not suppressing it. Matches exactly what the
    /// two FengWei powers contribute through their own additive hooks.
    /// </summary>
    private decimal NegativeFengWeiOffset()
    {
        decimal temporary =
            Owner.GetPower<FuBiPower>() != null
                ? 0m
                : Owner.GetPower<TemporaryFengWeiPower>()?.Amount ?? 0m;

        decimal effective =
            (Owner.GetPower<FengWeiPower>()?.Amount ?? 0m)
            + temporary;

        return effective < 0m ? -effective : 0m;
    }
}
