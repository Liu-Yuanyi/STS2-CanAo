using System.Runtime.CompilerServices;
using CanAoNative.Rules.YuHuo;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Rules.Exhaust;

/// <summary>
/// Single entry point for exhaust facts: per-player turn history, 浴火 and
/// Ethereal snapshots, and the after-exhaust notification. All "cards
/// exhausted this turn" rules must read this service instead of keeping
/// private counters or scattered static state.
/// </summary>
public static class ExhaustService
{
    private static readonly ConditionalWeakTable<
        ICombatState,
        ExhaustCombatState> States = new();

    public static ExhaustCombatState GetState(ICombatState combatState)
    {
        ArgumentNullException.ThrowIfNull(combatState);
        return States.GetValue(
            combatState,
            static _ => new ExhaustCombatState());
    }

    /// <summary>
    /// Records one finished exhaust and then notifies
    /// <see cref="IAfterCanAoCardExhausted"/> listeners. Called exclusively
    /// from CanAoCombatRules.AfterCardExhausted so every game exhaust path
    /// (normal play, other card effects, Ethereal, 浴火 resolution) funnels
    /// through here.
    /// </summary>
    public static async Task<ExhaustRecord?> RecordAndNotify(
        PlayerChoiceContext choiceContext,
        ICombatState combatState,
        CardModel card,
        bool causedByEthereal)
    {
        ArgumentNullException.ThrowIfNull(choiceContext);
        ArgumentNullException.ThrowIfNull(combatState);
        ArgumentNullException.ThrowIfNull(card);

        Player? owner = card.Owner;
        if (owner == null)
            return null;

        // The game's hook dispatcher pushed CanAoCombatRules on top of the
        // choice stack, so the entry below it is the best-effort source of
        // this exhaust (e.g. the card whose effect requested it).
        AbstractModel? source =
            choiceContext.ModelStack?.Skip(1).FirstOrDefault();

        ExhaustRecord record = GetState(combatState).Record(
            owner,
            card,
            YuHuoService.HasYuHuo(card, combatState),
            causedByEthereal,
            YuHuoService.IsResolving(card),
            source);

        await NotifyAfterExhausted(choiceContext, record);
        return record;
    }

    public static int GetExhaustedThisTurn(Player player)
    {
        ICombatState? combatState = player.Creature.CombatState;
        return combatState == null
            ? 0
            : GetState(combatState).GetExhaustedThisTurn(player);
    }

    public static bool HasExhaustedThisTurn(Player player) =>
        GetExhaustedThisTurn(player) > 0;

    public static IReadOnlyList<ExhaustRecord> GetRecordsThisTurn(
        Player player)
    {
        ICombatState? combatState = player.Creature.CombatState;
        return combatState == null
            ? []
            : GetState(combatState).GetRecordsThisTurn(player);
    }

    public static void ClearForPlayers(
        ICombatState combatState,
        IEnumerable<Player> players)
    {
        GetState(combatState).ClearForPlayers(players);
    }

    private static async Task NotifyAfterExhausted(
        PlayerChoiceContext choiceContext,
        ExhaustRecord record)
    {
        foreach (IAfterCanAoCardExhausted listener in
                 ExhaustListenerRegistry
                     .GetListeners<IAfterCanAoCardExhausted>(
                         record.Owner,
                         record.Card))
        {
            await listener.AfterCanAoCardExhausted(
                choiceContext,
                record);
        }
    }
}
