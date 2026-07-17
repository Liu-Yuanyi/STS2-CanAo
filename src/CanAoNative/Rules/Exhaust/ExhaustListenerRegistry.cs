using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Rules.Exhaust;

/// <summary>
/// Returns a deterministic listener snapshot: exhausted card first, then the
/// owner's powers, then enabled relics in the game's existing order.
/// </summary>
internal static class ExhaustListenerRegistry
{
    public static IReadOnlyList<TListener> GetListeners<TListener>(
        Player player,
        CardModel eventCard)
        where TListener : class
    {
        List<TListener> listeners = [];
        HashSet<object> seen =
            new(ReferenceEqualityComparer.Instance);

        AddIfListener(eventCard, listeners, seen);

        foreach (PowerModel power in player.Creature.Powers.ToArray())
            AddIfListener(power, listeners, seen);

        foreach (RelicModel relic in player.Relics.ToArray())
        {
            if (relic.Status != RelicStatus.Disabled)
                AddIfListener(relic, listeners, seen);
        }

        return listeners;
    }

    private static void AddIfListener<TListener>(
        object candidate,
        ICollection<TListener> listeners,
        ISet<object> seen)
        where TListener : class
    {
        if (candidate is TListener listener && seen.Add(candidate))
            listeners.Add(listener);
    }
}
