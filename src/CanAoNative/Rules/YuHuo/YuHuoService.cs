using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Rules.YuHuo;

/// <summary>
/// Unified entry point for intrinsic and temporary 浴火 state, execution
/// context queries, trigger-count modifiers and 浴火 lifecycle events.
/// </summary>
public static class YuHuoService
{
    private static readonly ConditionalWeakTable<ICombatState, YuHuoCombatState>
        States = new();

    public static YuHuoCombatState GetState(ICombatState combatState)
    {
        ArgumentNullException.ThrowIfNull(combatState);
        return States.GetValue(
            combatState,
            static _ => new YuHuoCombatState());
    }

    public static void GrantUntilTurnEnd(
        CardModel card,
        Player owner,
        ICombatState combatState)
    {
        ArgumentNullException.ThrowIfNull(card);
        ArgumentNullException.ThrowIfNull(owner);
        ArgumentNullException.ThrowIfNull(combatState);

        Player? cardOwner = card.Owner;
        if (cardOwner == null || !ReferenceEquals(cardOwner, owner))
        {
            throw new InvalidOperationException(
                "Temporary YuHuo can only be granted by the card's owner.");
        }

        GetState(combatState).GrantUntilTurnEnd(
            card,
            owner,
            owner.PlayerCombatState.TurnNumber);
    }

    /// <summary>
    /// Grants 浴火 for the rest of the combat (no turn expiry). The grant is
    /// still scoped to the combat and the owning player like all YuHuo state.
    /// </summary>
    public static void GrantPermanent(
        CardModel card,
        Player owner,
        ICombatState combatState)
    {
        ArgumentNullException.ThrowIfNull(card);
        ArgumentNullException.ThrowIfNull(owner);
        ArgumentNullException.ThrowIfNull(combatState);

        Player? cardOwner = card.Owner;
        if (cardOwner == null || !ReferenceEquals(cardOwner, owner))
        {
            throw new InvalidOperationException(
                "Permanent YuHuo can only be granted by the card's owner.");
        }

        GetState(combatState).GrantPermanent(card, owner);
    }

    public static bool HasYuHuo(
        CardModel card,
        ICombatState combatState)
    {
        if (card is IIntrinsicYuHuo { HasIntrinsicYuHuo: true })
            return true;

        Player? owner = card.Owner;
        if (owner == null)
            return false;

        if (GetState(combatState).HasPermanentYuHuo(card, owner))
            return true;

        int currentTurn = owner.PlayerCombatState.TurnNumber;

        return GetState(combatState).HasTemporaryYuHuo(
            card,
            owner,
            currentTurn);
    }

    public static int GetYuHuoTriggerCount(CardModel card)
    {
        int count = 1;

        foreach (IYuHuoTriggerCountModifier modifier in
                 YuHuoListenerRegistry
                     .GetListeners<IYuHuoTriggerCountModifier>(card))
        {
            count = modifier.ModifyYuHuoTriggerCount(card, count);
        }

        return Math.Max(1, count);
    }

    public static bool IsResolving(CardModel card)
    {
        ICombatState? combatState = GetCombatState(card);
        return combatState != null
               && GetState(combatState).IsResolving(card);
    }

    public static bool TryGetCurrentContext(
        CardModel card,
        out YuHuoExecutionContext? context)
    {
        ICombatState? combatState = GetCombatState(card);

        if (combatState == null)
        {
            context = null;
            return false;
        }

        return GetState(combatState)
            .TryGetExecutionContext(card, out context);
    }

    public static YuHuoExecutionContext? GetCurrentContext(
        CardModel card)
    {
        return TryGetCurrentContext(card, out YuHuoExecutionContext? context)
            ? context
            : null;
    }

    public static bool IsTriggeredByYuHuo(CardModel card) =>
        TryGetCurrentContext(card, out _);

    public static async Task NotifyBeforeResolved(
        PlayerChoiceContext choiceContext,
        YuHuoResolutionContext context)
    {
        foreach (IBeforeYuHuoResolved listener in
                 YuHuoListenerRegistry
                     .GetListeners<IBeforeYuHuoResolved>(context.Card))
        {
            await listener.BeforeYuHuoResolved(choiceContext, context);
        }
    }

    public static async Task NotifyBeforeTrigger(
        PlayerChoiceContext choiceContext,
        YuHuoExecutionContext context)
    {
        foreach (IBeforeYuHuoTrigger listener in
                 YuHuoListenerRegistry
                     .GetListeners<IBeforeYuHuoTrigger>(context.Card))
        {
            await listener.BeforeYuHuoTrigger(choiceContext, context);
        }
    }

    public static async Task NotifyAfterTrigger(
        PlayerChoiceContext choiceContext,
        YuHuoExecutionContext context)
    {
        foreach (IAfterYuHuoTrigger listener in
                 YuHuoListenerRegistry
                     .GetListeners<IAfterYuHuoTrigger>(context.Card))
        {
            await listener.AfterYuHuoTrigger(choiceContext, context);
        }
    }

    public static async Task NotifyAfterResolved(
        PlayerChoiceContext choiceContext,
        YuHuoResolutionContext context)
    {
        foreach (IAfterYuHuoResolved listener in
                 YuHuoListenerRegistry
                     .GetListeners<IAfterYuHuoResolved>(context.Card))
        {
            await listener.AfterYuHuoResolved(choiceContext, context);
        }
    }

    private static ICombatState? GetCombatState(CardModel card) =>
        card.CombatState ?? card.Owner?.Creature?.CombatState;
}
