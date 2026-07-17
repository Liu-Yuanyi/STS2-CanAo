using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace CanAoNative.Rules.StarMoon;

/// <summary>
/// Runs after the concrete Star-Moon Strike instance is created but before it
/// is added to the player's hand.
/// </summary>
public interface IBeforeStarMoonGenerated
{
    Task BeforeStarMoonGenerated(
        PlayerChoiceContext choiceContext,
        StarMoonGenerationContext context);
}

/// <summary>
/// Runs after one concrete Star-Moon Strike has successfully entered combat.
/// The per-player generated-this-turn counter has already been updated.
/// </summary>
public interface IAfterStarMoonGenerated
{
    Task AfterStarMoonGenerated(
        PlayerChoiceContext choiceContext,
        StarMoonGenerationContext context);
}

/// <summary>
/// Runs after one Star-Moon Strike CardPlay and the normal late card-play
/// hooks have reached CanAoCombatRules. The played-this-turn counter has
/// already been updated.
/// </summary>
public interface IAfterStarMoonPlayed
{
    Task AfterStarMoonPlayed(
        PlayerChoiceContext choiceContext,
        StarMoonPlayedContext context);
}
