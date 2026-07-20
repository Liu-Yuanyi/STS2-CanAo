using CanAoNative.Rules.FengWei;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Powers;

/// <summary>
/// 王座孤明：拥有者回合结束时，若其手牌为空，获得 Amount 点永久凤威。
/// 手牌为空的判定必须在 BeforeSideTurnEnd 进行
/// （弃手牌发生在其后的 FlushPlayerHand），与孤王玉座同一时机。
/// </summary>
public sealed class WangZuoGuMingPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != Owner.Side
            || !participants.Contains(Owner)
            || Amount <= 0)
        {
            return;
        }

        if (Owner.Player is not Player player
            || !player.PlayerCombatState.Hand.IsEmpty)
        {
            return;
        }

        Flash();

        await FengWeiService.GainPermanent(
            choiceContext,
            player,
            Amount,
            cardSource: null);
    }
}
