using MegaCrit.Sts2.Core.Entities.Players;

namespace CanAoNative.Rules.Edict;

/// <summary>
/// Combat-scoped, per-player Edict turn history. It is deliberately kept
/// outside cards and powers so all future effects read one authoritative
/// state.
/// </summary>
public sealed class EdictCombatState
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
        GetOrCreate(player).PlayedThisTurn++;
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

    public void ClearForPlayers(IEnumerable<Player> players)
    {
        foreach (Player player in players)
            _players.Remove(player);
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
    }
}
