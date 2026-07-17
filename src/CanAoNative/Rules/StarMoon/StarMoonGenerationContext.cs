using CanAoNative.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Rules.StarMoon;

/// <summary>
/// Describes one concrete Star-Moon Strike generated from one resolved
/// Star/Moon pair. GenerationIndex is one-based within GenerationCount.
/// </summary>
public sealed record StarMoonGenerationContext(
    Player Player,
    StarMoonStrike Card,
    int GenerationIndex,
    int GenerationCount,
    Creature? Applier,
    CardModel? CardSource);
