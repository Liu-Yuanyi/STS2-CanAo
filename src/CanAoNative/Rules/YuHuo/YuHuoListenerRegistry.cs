using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Rules.YuHuo;

/// <summary>
/// Produces a deterministic snapshot of models that may participate in 浴火.
/// The card itself is evaluated first, followed by the owner's powers and
/// relics in their existing game order.
/// </summary>
internal static class YuHuoListenerRegistry
{
    public static IReadOnlyList<TListener> GetListeners<TListener>(
        CardModel card)
        where TListener : class
    {
        List<TListener> listeners = [];
        HashSet<object> seen =
            new(ReferenceEqualityComparer.Instance);

        AddIfListener(card, listeners, seen);

        Player? owner = card.Owner;
        if (owner == null)
            return listeners;

        foreach (PowerModel power in owner.Creature.Powers.ToArray())
            AddIfListener(power, listeners, seen);

        foreach (RelicModel relic in owner.Relics.ToArray())
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
