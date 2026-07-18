using System.Runtime.CompilerServices;
using CanAoNative.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace CanAoNative.Rules.Edict;

/// <summary>
/// Single entry point for Edict generation and per-player turn history.
/// Future cards, relics and powers that create Edicts must call
/// <see cref="Generate"/> rather than adding the token directly, so all
/// "edicts generated/played this turn" rules read one authoritative state.
/// </summary>
public static class EdictService
{
    private static readonly ConditionalWeakTable<
        ICombatState,
        EdictCombatState> States = new();

    public static EdictCombatState GetState(ICombatState combatState)
    {
        ArgumentNullException.ThrowIfNull(combatState);
        return States.GetValue(
            combatState,
            static _ => new EdictCombatState());
    }

    /// <summary>
    /// Creates and adds concrete Edict instances through the one
    /// authoritative generation pipeline.
    /// </summary>
    public static async Task<IReadOnlyList<EdictCard>> Generate(
        PlayerChoiceContext choiceContext,
        Player player,
        int count)
    {
        ArgumentNullException.ThrowIfNull(player);

        if (count <= 0)
            return [];

        ICombatState? combatState = player.Creature.CombatState;
        if (combatState == null)
        {
            throw new InvalidOperationException(
                "Edict generation requires an active combat.");
        }

        List<EdictCard> generated = [];

        for (int i = 0; i < count; i++)
        {
            EdictCard edict = combatState.CreateCard<EdictCard>(player);

            await CardPileCmd.AddGeneratedCardToCombat(
                edict,
                PileType.Hand,
                player);

            RecordGenerated(combatState, player);
            generated.Add(edict);
        }

        return generated;
    }

    public static int GetGeneratedThisTurn(Player player)
    {
        ICombatState? combatState = player.Creature.CombatState;
        return combatState == null
            ? 0
            : GetState(combatState).GetGeneratedThisTurn(player);
    }

    public static bool HasGeneratedThisTurn(Player player) =>
        GetGeneratedThisTurn(player) > 0;

    public static int GetPlayedThisTurn(Player player)
    {
        ICombatState? combatState = player.Creature.CombatState;
        return combatState == null
            ? 0
            : GetState(combatState).GetPlayedThisTurn(player);
    }

    public static bool HasPlayedThisTurn(Player player) =>
        GetPlayedThisTurn(player) > 0;

    public static void RecordGenerated(
        ICombatState combatState,
        Player player)
    {
        GetState(combatState).RecordGenerated(player);
    }

    public static void RecordPlayed(
        ICombatState combatState,
        Player player)
    {
        GetState(combatState).RecordPlayed(player);
    }

    public static void ClearForPlayers(
        ICombatState combatState,
        IEnumerable<Player> players)
    {
        GetState(combatState).ClearForPlayers(players);
    }

    public static async Task NotifyAfterPlayed(
        PlayerChoiceContext choiceContext,
        EdictPlayedContext context)
    {
        foreach (IAfterEdictPlayed listener in
                 EdictListenerRegistry
                     .GetListeners<IAfterEdictPlayed>(
                         context.Player,
                         context.Card))
        {
            await listener.AfterEdictPlayed(
                choiceContext,
                context);
        }
    }
}
