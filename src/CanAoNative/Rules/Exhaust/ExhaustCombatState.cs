using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Rules.Exhaust;

/// <summary>
/// Combat-scoped, per-player exhaust turn history. It is deliberately kept
/// outside cards and powers so all present and future effects read one
/// authoritative state instead of private counters.
/// </summary>
public sealed class ExhaustCombatState
{
    private readonly Dictionary<Player, PlayerTurnState> _players =
        new(ReferenceEqualityComparer.Instance);

    public ExhaustRecord Record(
        Player player,
        CardModel card,
        bool hadYuHuo,
        bool causedByEthereal,
        bool resolvedByYuHuo,
        AbstractModel? sourceModel)
    {
        ArgumentNullException.ThrowIfNull(player);
        ArgumentNullException.ThrowIfNull(card);

        PlayerTurnState state = GetOrCreate(player);

        ExhaustRecord record = new(
            card,
            player,
            card.Type,
            hadYuHuo,
            causedByEthereal,
            resolvedByYuHuo,
            sourceModel,
            state.Records.Count + 1);

        state.Records.Add(record);
        return record;
    }

    public int GetExhaustedThisTurn(Player player)
    {
        ArgumentNullException.ThrowIfNull(player);
        return _players.TryGetValue(player, out PlayerTurnState? state)
            ? state.Records.Count
            : 0;
    }

    public IReadOnlyList<ExhaustRecord> GetRecordsThisTurn(Player player)
    {
        ArgumentNullException.ThrowIfNull(player);
        return _players.TryGetValue(player, out PlayerTurnState? state)
            ? state.Records.ToArray()
            : [];
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
        public List<ExhaustRecord> Records { get; } = [];
    }
}
