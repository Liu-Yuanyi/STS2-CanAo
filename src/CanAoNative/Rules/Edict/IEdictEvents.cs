using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace CanAoNative.Rules.Edict;

/// <summary>
/// Runs after one Edict CardPlay and the normal late card-play hooks have
/// reached CanAoCombatRules. The played-this-turn counter has already been
/// updated.
/// </summary>
public interface IAfterEdictPlayed
{
    Task AfterEdictPlayed(
        PlayerChoiceContext choiceContext,
        EdictPlayedContext context);
}
