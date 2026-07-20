using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Powers;

/// <summary>
/// 守缺：回合结束清除星/月时，各自保留至多 Amount 点。
/// 本 Power 只是标记；保留逻辑在 StarPower 与 MoonPower 的
/// 回合末清除中读取，保证与清除在同一条 Task 链内结算。
/// </summary>
public sealed class ShouQuePower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}
