using CanAoNative.Powers;
using CanAoNative.Rules.StarMoon;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Rules;

/// <summary>
/// Resolves all currently available Star/Moon pairs for one player and emits
/// one deterministic generation event for each concrete Star-Moon Strike.
/// </summary>
public static class StarMoonHelper
{
    private static readonly Logger Log =
        new("CanAoNative", LogType.Generic);

    private static readonly HashSet<Player> ResolvingPlayers =
        new(ReferenceEqualityComparer.Instance);

    public static async Task CheckAndResolve(
        PlayerChoiceContext choiceContext,
        Player player,
        Creature? applier,
        CardModel? cardSource)
    {
        if (!ResolvingPlayers.Add(player))
            return;

        try
        {
            StarPower? star =
                player.Creature.GetPower<StarPower>();
            MoonPower? moon =
                player.Creature.GetPower<MoonPower>();

            if (star == null || moon == null)
                return;

            int pairCount =
                Math.Min(star.Amount, moon.Amount);

            if (pairCount <= 0)
                return;

            // Use native commands so hooks, combat history, multiplayer state
            // and automatic removal all observe the resource change.
            await PowerCmd.ModifyAmount(
                choiceContext,
                star,
                -pairCount,
                applier,
                cardSource,
                silent: true);

            await PowerCmd.ModifyAmount(
                choiceContext,
                moon,
                -pairCount,
                applier,
                cardSource,
                silent: true);

            if (player.Creature.CombatState == null)
                return;

            await StarMoonService.Generate(
                choiceContext,
                player,
                pairCount,
                applier,
                cardSource);

#if DEBUG
            Log.Info(
                $"STARMOON_RESOLVED: player={player.NetId}, " +
                $"pairs={pairCount}, " +
                $"generatedThisTurn=" +
                $"{StarMoonService.GetGeneratedThisTurn(player)}");
#endif
        }
        catch (Exception ex)
        {
            Log.Error(
                $"STARMOON_FAILED: player={player.NetId}\n{ex}");
            throw;
        }
        finally
        {
            ResolvingPlayers.Remove(player);
        }
    }
}
