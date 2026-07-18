using CanAoNative.Rules.StarMoon;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Powers;

/// <summary>
/// 星月王冠：每回合前 Amount 次获得凤威（永久或临时）时，
/// 各获得 1 张星月合击。
/// </summary>
public sealed class XingYueWangGuanPower : PowerModel
{
    private int _triggersThisTurn;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (_triggersThisTurn >= Amount
            || amount <= 0
            || power is not (FengWeiPower or TemporaryFengWeiPower)
            || !ReferenceEquals(power.Owner, Owner)
            || Owner.Player is not Player player)
        {
            return;
        }

        _triggersThisTurn++;
        Flash();

        await StarMoonService.Generate(
            choiceContext,
            player,
            1,
            applier,
            cardSource);
    }

    public override Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (ReferenceEquals(player.Creature, Owner))
            _triggersThisTurn = 0;

        return Task.CompletedTask;
    }
}
