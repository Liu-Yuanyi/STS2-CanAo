using CanAoNative.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Rules.FengWei;

/// <summary>
/// Single authority for permanent, temporary and effective 凤威 values.
/// Permanent 凤威 lives in FengWeiPower. Turn-only adjustments live in
/// TemporaryFengWeiPower. Gameplay code should use this service rather than
/// applying or querying those powers ad hoc.
/// </summary>
public static class FengWeiService
{
    public static decimal GetPermanentAmount(Player player)
    {
        ArgumentNullException.ThrowIfNull(player);

        FengWeiPower? power =
            player.Creature.GetPower<FengWeiPower>();

        return power == null ? 0m : power.Amount;
    }

    public static decimal GetTemporaryAmount(Player player)
    {
        ArgumentNullException.ThrowIfNull(player);

        if (player.Creature.GetPower<FuBiPower>() != null)
            return 0m;

        TemporaryFengWeiPower? power =
            player.Creature.GetPower<TemporaryFengWeiPower>();

        return power == null ? 0m : power.Amount;
    }

    public static decimal GetEffectiveAmount(Player player)
    {
        ArgumentNullException.ThrowIfNull(player);

        return GetPermanentAmount(player)
               + GetTemporaryAmount(player);
    }

    public static async Task GainPermanent(
        PlayerChoiceContext choiceContext,
        Player player,
        decimal amount,
        CardModel? cardSource)
    {
        ArgumentNullException.ThrowIfNull(player);

        if (amount == 0m)
            return;

        await PowerCmd.Apply<FengWeiPower>(
            choiceContext,
            player.Creature,
            amount,
            player.Creature,
            cardSource);
    }

    public static async Task ModifyTemporary(
        PlayerChoiceContext choiceContext,
        Player player,
        decimal amount,
        CardModel? cardSource)
    {
        ArgumentNullException.ThrowIfNull(player);

        if (amount == 0m)
            return;

        await PowerCmd.Apply<TemporaryFengWeiPower>(
            choiceContext,
            player.Creature,
            amount,
            player.Creature,
            cardSource);
    }
}
