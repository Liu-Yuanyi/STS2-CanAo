using CanAoNative.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;

namespace CanAoNative.Rules.Edict;

/// <summary>
/// Describes one completed Edict CardPlay. The per-player played-this-turn
/// counter has already been updated before listeners run.
/// </summary>
public sealed record EdictPlayedContext(
    Player Player,
    EdictCard Card,
    CardPlay CardPlay);
