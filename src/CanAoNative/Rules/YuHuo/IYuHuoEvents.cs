using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace CanAoNative.Rules.YuHuo;

/// <summary>
/// Invoked once after the trigger count is finalized and before the first
/// 浴火 auto-play begins.
/// </summary>
public interface IBeforeYuHuoResolved
{
    Task BeforeYuHuoResolved(
        PlayerChoiceContext choiceContext,
        YuHuoResolutionContext context);
}

/// <summary>
/// Invoked immediately before each individual 浴火 auto-play.
/// </summary>
public interface IBeforeYuHuoTrigger
{
    Task BeforeYuHuoTrigger(
        PlayerChoiceContext choiceContext,
        YuHuoExecutionContext context);
}

/// <summary>
/// Invoked immediately after each individual 浴火 auto-play has completed.
/// The execution context remains active while this callback runs.
/// </summary>
public interface IAfterYuHuoTrigger
{
    Task AfterYuHuoTrigger(
        PlayerChoiceContext choiceContext,
        YuHuoExecutionContext context);
}

/// <summary>
/// Invoked once after all 浴火 auto-plays and the final exhaust movement have
/// completed successfully.
/// </summary>
public interface IAfterYuHuoResolved
{
    Task AfterYuHuoResolved(
        PlayerChoiceContext choiceContext,
        YuHuoResolutionContext context);
}
