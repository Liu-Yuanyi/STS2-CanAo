using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Powers;

/// <summary>
/// 照月成星：下回合开始时获得 2 星，然后移除此 Power。
/// </summary>
public sealed class ZhaoYueChengXingPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (!ReferenceEquals(player.Creature, Owner))
            return;

        await PowerCmd.Remove(this);

        await PowerCmd.Apply<StarPower>(
            choiceContext,
            Owner,
            2m,
            Owner,
            cardSource: null);
    }
}
