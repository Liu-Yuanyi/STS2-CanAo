using System.Runtime.CompilerServices;
using CanAoNative.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Rules.StarMoon;

/// <summary>
/// Single entry point for Star-Moon generation/play events and turn history.
/// </summary>
public static class StarMoonService
{
    private static readonly ConditionalWeakTable<
        ICombatState,
        StarMoonCombatState> States = new();

    public static StarMoonCombatState GetState(ICombatState combatState)
    {
        ArgumentNullException.ThrowIfNull(combatState);
        return States.GetValue(
            combatState,
            static _ => new StarMoonCombatState());
    }


    /// <summary>
    /// Creates and adds concrete Star-Moon Strike instances through the one
    /// authoritative generation pipeline. Future cards, relics and powers
    /// that create Star-Moon Strikes must call this method rather than adding
    /// the token directly. Tokens may optionally be upgraded (星月合击+).
    /// </summary>
    public static async Task<IReadOnlyList<StarMoonStrike>> Generate(
        PlayerChoiceContext choiceContext,
        Player player,
        int count,
        Creature? applier,
        CardModel? cardSource,
        bool upgraded = false)
    {
        ArgumentNullException.ThrowIfNull(player);

        if (count <= 0)
            return [];

        ICombatState? combatState = player.Creature.CombatState;
        if (combatState == null)
        {
            throw new InvalidOperationException(
                "Star-Moon Strike generation requires an active combat.");
        }

        List<StarMoonStrike> generated = [];

        for (int i = 0; i < count; i++)
        {
            StarMoonStrike strike =
                combatState.CreateCard<StarMoonStrike>(player);

            if (upgraded)
                CardCmd.Upgrade(strike);

            StarMoonGenerationContext generationContext = new(
                player,
                strike,
                GenerationIndex: i + 1,
                GenerationCount: count,
                Applier: applier,
                CardSource: cardSource);

            await NotifyBeforeGenerated(
                choiceContext,
                generationContext);

            await CardPileCmd.AddGeneratedCardToCombat(
                strike,
                PileType.Hand,
                player);

            RecordGenerated(combatState, player);
            generated.Add(strike);

            await NotifyAfterGenerated(
                choiceContext,
                generationContext);
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

    public static void ClearTurnForPlayers(
        ICombatState combatState,
        IEnumerable<Player> players)
    {
        GetState(combatState).ClearForPlayers(players);
    }

    public static async Task NotifyBeforeGenerated(
        PlayerChoiceContext choiceContext,
        StarMoonGenerationContext context)
    {
        foreach (IBeforeStarMoonGenerated listener in
                 StarMoonListenerRegistry
                     .GetListeners<IBeforeStarMoonGenerated>(
                         context.Player,
                         context.Card))
        {
            await listener.BeforeStarMoonGenerated(
                choiceContext,
                context);
        }
    }

    public static async Task NotifyAfterGenerated(
        PlayerChoiceContext choiceContext,
        StarMoonGenerationContext context)
    {
        foreach (IAfterStarMoonGenerated listener in
                 StarMoonListenerRegistry
                     .GetListeners<IAfterStarMoonGenerated>(
                         context.Player,
                         context.Card))
        {
            await listener.AfterStarMoonGenerated(
                choiceContext,
                context);
        }
    }

    public static async Task NotifyAfterPlayed(
        PlayerChoiceContext choiceContext,
        StarMoonPlayedContext context)
    {
        foreach (IAfterStarMoonPlayed listener in
                 StarMoonListenerRegistry
                     .GetListeners<IAfterStarMoonPlayed>(
                         context.Player,
                         context.Card))
        {
            await listener.AfterStarMoonPlayed(
                choiceContext,
                context);
        }
    }
}
