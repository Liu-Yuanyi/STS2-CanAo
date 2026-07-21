using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Powers;

/// <summary>
/// 照月成星（延迟）：下回合开始时获得 2 星。
/// 触发后静默归零（不播放移除动画以免UI卡死），由回合清理自然回收。
/// </summary>
public sealed class ZhaoYueChengXingPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (!ReferenceEquals(player.Creature, Owner) || Amount <= 0)
            return;

        // Grant the stars first.
        await PowerCmd.Apply<StarPower>(
            choiceContext,
            Owner,
            2m,
            Owner,
            cardSource: null);

        // Silently zero out so the UI doesn't play a stuck removal animation.
        // The zero-amount power is harmless and will be cleaned up naturally.
        await PowerCmd.ModifyAmount(
            choiceContext,
            this,
            -Amount,
            Owner,
            cardSource: null,
            silent: true);
    }
}
