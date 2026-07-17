using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Rules.YuHuo;

/// <summary>
/// Immutable information shared by the entire resolution of one 浴火 event.
/// TriggerIndex lives on <see cref="YuHuoExecutionContext"/> because one
/// resolution may execute the card more than once.
/// </summary>
public sealed record YuHuoResolutionContext(
    CardModel Card,
    ICombatState CombatState,
    int TriggerCount,
    bool CausedByEthereal,
    PileType? OriginalPile);
