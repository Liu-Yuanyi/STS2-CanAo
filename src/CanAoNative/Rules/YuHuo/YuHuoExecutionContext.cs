using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Rules.YuHuo;

/// <summary>
/// Runtime information for one individual 浴火 trigger.
/// TriggerIndex is one-based. EffectExecuted is false for auto-play attempts
/// rejected by Unplayable, target resolution or a ShouldPlay hook.
/// </summary>
public sealed class YuHuoExecutionContext
{
    public CardModel Card { get; }
    public int TriggerIndex { get; }
    public int TriggerCount { get; }
    public bool CausedByEthereal { get; }
    public PileType? OriginalPile { get; }
    public bool EffectExecuted { get; private set; }

    public YuHuoExecutionContext(
        CardModel card,
        int triggerIndex,
        int triggerCount,
        bool causedByEthereal,
        PileType? originalPile)
    {
        Card = card;
        TriggerIndex = triggerIndex;
        TriggerCount = triggerCount;
        CausedByEthereal = causedByEthereal;
        OriginalPile = originalPile;
    }

    internal void MarkEffectExecuted()
    {
        EffectExecuted = true;
    }
}
