using MegaCrit.Sts2.Core.Entities.Players;

namespace CanAoNative.Rules.StarMoon;

/// <summary>
/// Combat-scoped, per-player Star-Moon turn and combat history.
/// It is deliberately kept outside cards and powers so all future effects
/// read one authoritative state.
/// </summary>
public sealed class StarMoonCombatState
{
    private readonly Dictionary<Player, PlayerTurnState> _players =
        new(ReferenceEqualityComparer.Instance);

    public void RecordGenerated(Player player)
    {
        ArgumentNullException.ThrowIfNull(player);
        GetOrCreate(player).GeneratedThisTurn++;
    }

    public void RecordPlayed(Player player)
    {
        ArgumentNullException.ThrowIfNull(player);
        PlayerTurnState state = GetOrCreate(player);
        state.PlayedThisTurn++;
        state.PlayedThisCombat++;
    }

    public int GetGeneratedThisTurn(Player player)
    {
        ArgumentNullException.ThrowIfNull(player);
        return _players.TryGetValue(player, out PlayerTurnState? state)
            ? state.GeneratedThisTurn
            : 0;
    }

    public int GetPlayedThisTurn(Player player)
    {
        ArgumentNullException.ThrowIfNull(player);
        return _players.TryGetValue(player, out PlayerTurnState? state)
            ? state.PlayedThisTurn
            : 0;
    }

    /// <summary>
    /// Returns the total number of Star-Moon Strikes played this combat,
    /// across all turns. Used by 星月终式 for per-combat hit count scaling.
    /// </summary>
    public int GetPlayedThisCombat(Player player)
    {
        ArgumentNullException.ThrowIfNull(player);
        return _players.TryGetValue(player, out PlayerTurnState? state)
            ? state.PlayedThisCombat
            : 0;
    }

    public void ClearForPlayers(IEnumerable<Player> players)
    {
        // 只清回合计数；PlayedThisCombat 是整场战斗的累计
        // （星月终式的段数依据），不得随回合结束移除条目重置。
        foreach (Player player in players)
        {
            if (_players.TryGetValue(player, out PlayerTurnState? state))
            {
                state.GeneratedThisTurn = 0;
                state.PlayedThisTurn = 0;
            }
        }
    }

    private PlayerTurnState GetOrCreate(Player player)
    {
        if (_players.TryGetValue(player, out PlayerTurnState? state))
            return state;

        state = new PlayerTurnState();
        _players.Add(player, state);
        return state;
    }

    private sealed class PlayerTurnState
    {
        public int GeneratedThisTurn { get; set; }
        public int PlayedThisTurn { get; set; }
        /// <summary>
        /// Running total of Star-Moon Strikes played this combat.
        /// Never reset within a combat — only cleared when the combat ends.
        /// </summary>
        public int PlayedThisCombat { get; set; }
    }
}
