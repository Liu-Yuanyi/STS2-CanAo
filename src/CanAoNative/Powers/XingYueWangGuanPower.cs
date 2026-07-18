using CanAoNative.Rules.StarMoon;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Powers;

/// <summary>
/// 星月王冠：每回合第一次获得凤威（永久或临时）时，获得 1 张星月合击。
/// UpgradedGeneration is set by the source card when it was upgraded,
/// producing 星月合击+ instead.
/// </summary>
public sealed class XingYueWangGuanPower : PowerModel
{
    private bool _gainedThisTurn;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>Generate 星月合击+ instead of the base token.</summary>
    public bool UpgradedGeneration { get; set; }

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (_gainedThisTurn
            || amount <= 0
            || power is not (FengWeiPower or TemporaryFengWeiPower)
            || !ReferenceEquals(power.Owner, Owner)
            || Owner.Player is not Player player)
        {
            return;
        }

        _gainedThisTurn = true;
        Flash();

        await StarMoonService.Generate(
            choiceContext,
            player,
            1,
            applier,
            cardSource,
            UpgradedGeneration);
    }

    public override Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (ReferenceEquals(player.Creature, Owner))
            _gainedThisTurn = false;

        return Task.CompletedTask;
    }
}
