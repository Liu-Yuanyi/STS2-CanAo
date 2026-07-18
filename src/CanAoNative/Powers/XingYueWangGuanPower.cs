using CanAoNative.Rules.StarMoon;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Powers;

/// <summary>
/// 星月王冠：每回合第一次获得凤威（永久或临时）时，
/// 一次性获得 Amount 张星月合击。首个触发即锁存，
/// 同回合后续获得（如凤威酒的第二段）自然被忽略。
/// </summary>
public sealed class XingYueWangGuanPower : PowerModel
{
    private bool _triggeredThisTurn;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (_triggeredThisTurn
            || amount <= 0
            || power is not (FengWeiPower or TemporaryFengWeiPower)
            || !ReferenceEquals(power.Owner, Owner)
            || Owner.Player is not Player player)
        {
            return;
        }

        _triggeredThisTurn = true;
        Flash();

        await StarMoonService.Generate(
            choiceContext,
            player,
            (int)Amount,
            applier,
            cardSource);
    }

    public override Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (ReferenceEquals(player.Creature, Owner))
            _triggeredThisTurn = false;

        return Task.CompletedTask;
    }
}
