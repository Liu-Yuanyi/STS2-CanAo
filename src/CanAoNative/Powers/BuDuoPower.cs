using CanAoNative.Cards;
using CanAoNative.Rules.StarMoon;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Powers;

/// <summary>
/// 不堕（v12 重做）：每回合前 Amount 次打出的星月合击不受小于 0 的
/// 凤威影响。凤威对星月合击的修正由 FengWeiPower 与
/// TemporaryFengWeiPower 的增量 Hook 叠加而成；命中前 n 次时本 Power
/// 追加一个非负补偿量，使合计修正等于 max(0, 永久凤威 + 临时凤威)。
/// 叠加语义：层数 = 每回合免疫次数（Counter，R11 叠加统一）。
/// </summary>
public sealed class BuDuoPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (cardSource is StarMoonStrike && WithinImmunePlays())
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
        if (cardSource is StarMoonStrike && WithinImmunePlays())
            return NegativeFengWeiOffset();

        return 0m;
    }

    /// <summary>
    /// 本回合已打出数（打出记录在 AfterCardPlayedLate 才递增，
    /// 当前这张合击尚未计入）小于层数时，本张免疫。
    /// </summary>
    private bool WithinImmunePlays()
    {
        if (Amount <= 0 || Owner.Player is not { } player)
            return false;

        return StarMoonService.GetPlayedThisTurn(player) < Amount;
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
