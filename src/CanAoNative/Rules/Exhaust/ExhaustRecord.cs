using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Rules.Exhaust;

/// <summary>
/// Best-effort classification of how one recorded exhaust came about. The raw
/// facts on <see cref="ExhaustRecord"/> stay authoritative; this is only a
/// convenience view for future cards, powers and relics.
/// </summary>
public enum CanAoExhaustCause
{
    /// <summary>No involved model was visible on the choice stack.</summary>
    Unknown,

    /// <summary>The card exhausted itself after being played (Exhaust keyword).</summary>
    SelfPlay,

    /// <summary>Another model (usually a card effect) caused this exhaust.</summary>
    OtherEffect,

    /// <summary>Turn-end Ethereal exhaust (causedByEthereal flag).</summary>
    Ethereal,

    /// <summary>Final exhaust inside a 浴火 resolution.</summary>
    YuHuoResolution
}

/// <summary>
/// Immutable snapshot of one card exhaust. Created by ExhaustService after the
/// game's own CardCmd.Exhaust pipeline finished, so 浴火 and Ethereal facts
/// are captured before any later listener mutates state.
/// </summary>
public sealed record ExhaustRecord(
    CardModel Card,
    Player Owner,
    CardType CardType,
    bool HadYuHuo,
    bool CausedByEthereal,
    bool ResolvedByYuHuo,
    AbstractModel? SourceModel,
    int SequenceNumberThisTurn)
{
    public CanAoExhaustCause Cause
    {
        get
        {
            if (CausedByEthereal)
                return CanAoExhaustCause.Ethereal;

            if (ResolvedByYuHuo)
                return CanAoExhaustCause.YuHuoResolution;

            if (SourceModel is null)
                return CanAoExhaustCause.Unknown;

            return ReferenceEquals(SourceModel, Card)
                ? CanAoExhaustCause.SelfPlay
                : CanAoExhaustCause.OtherEffect;
        }
    }
}
