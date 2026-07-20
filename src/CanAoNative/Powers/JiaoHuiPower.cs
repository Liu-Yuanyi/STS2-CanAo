using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Powers;

/// <summary>
/// 交辉：偶数回合开始时获得 Amount 星，奇数回合开始时获得 Amount 月。
/// 打出的当回合已过回合开始时机，自然从下回合起生效。
/// </summary>
public sealed class JiaoHuiPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (!ReferenceEquals(player.Creature, Owner)
            || Amount <= 0)
        {
            return;
        }

        Flash();

        if (player.PlayerCombatState.TurnNumber % 2 == 0)
        {
            await PowerCmd.Apply<StarPower>(
                choiceContext,
                Owner,
                Amount,
                Owner,
                cardSource: null);
        }
        else
        {
            await PowerCmd.Apply<MoonPower>(
                choiceContext,
                Owner,
                Amount,
                Owner,
                cardSource: null);
        }
    }
}
