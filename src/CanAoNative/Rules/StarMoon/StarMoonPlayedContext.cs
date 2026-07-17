using CanAoNative.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;

namespace CanAoNative.Rules.StarMoon;

/// <summary>
/// Describes one completed Star-Moon Strike CardPlay. Replay and auto-play
/// instances are counted independently because each one resolves an effect.
/// </summary>
public sealed record StarMoonPlayedContext(
    Player Player,
    StarMoonStrike Card,
    CardPlay CardPlay);
