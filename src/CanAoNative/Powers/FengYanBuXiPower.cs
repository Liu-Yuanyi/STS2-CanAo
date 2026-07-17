using CanAoNative.Rules.YuHuo;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Powers;

/// <summary>
/// 凤焰不息：每层令浴火额外结算一次。
/// </summary>
public sealed class FengYanBuXiPower :
    PowerModel,
    IYuHuoTriggerCountModifier
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public int ModifyYuHuoTriggerCount(
        CardModel card,
        int currentCount)
    {
        return currentCount + Math.Max(0, Amount);
    }
}
