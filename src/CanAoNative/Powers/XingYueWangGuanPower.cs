using CanAoNative.Rules.StarMoon;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Powers;

/// <summary>
/// 星月王冠：每回合前 Amount 次获得凤威（永久或临时）时，
/// 各获得 1 张星月合击。同一次行动造成的多段凤威（如凤威酒）
/// 只计一次获得。
/// </summary>
public sealed class XingYueWangGuanPower : PowerModel
{
    private readonly HashSet<object> _triggerSourcesThisTurn =
        new(ReferenceEqualityComparer.Instance);

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

        // One action can apply several FengWei powers (e.g. FengWei Wine
        // applies both). Treat each distinct source as one gain.
        object sourceKey =
            (object?)cardSource ?? (object?)applier ?? power;
        if (!_triggerSourcesThisTurn.Add(sourceKey))
            return;

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
        {
            _triggersThisTurn = 0;
            _triggerSourcesThisTurn.Clear();
        }

        return Task.CompletedTask;
    }
}
