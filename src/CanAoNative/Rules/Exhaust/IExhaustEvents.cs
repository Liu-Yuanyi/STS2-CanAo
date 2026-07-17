using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace CanAoNative.Rules.Exhaust;

/// <summary>
/// Runs after one card exhaust has been recorded by ExhaustService. The
/// per-player turn history already contains the ExhaustRecord, including the
/// first-exhaust-this-turn fact (SequenceNumberThisTurn == 1).
/// </summary>
public interface IAfterCanAoCardExhausted
{
    Task AfterCanAoCardExhausted(
        PlayerChoiceContext choiceContext,
        ExhaustRecord record);
}
