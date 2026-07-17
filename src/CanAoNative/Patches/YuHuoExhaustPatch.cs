using CanAoNative.Rules.YuHuo;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Patches;

/// <summary>
/// Intercepts the exact CardCmd.Exhaust overload used by the game.
///
/// CardCmd.Exhaust returns Task. If a Harmony Prefix skips the original method,
/// it must assign a non-null replacement Task to __result. Otherwise callers
/// such as Burning Pact execute "await null" and never reach their remaining
/// effects.
/// </summary>
[HarmonyPatch(
    typeof(CardCmd),
    nameof(CardCmd.Exhaust),
    new Type[]
    {
        typeof(PlayerChoiceContext),
        typeof(CardModel),
        typeof(bool),
        typeof(bool)
    })]
public static class YuHuoExhaustPatch
{
    public static bool Prefix(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool causedByEthereal,
        bool skipVisuals,
        ref Task __result)
    {
        ICombatState? combatState =
            card.CombatState ?? card.Owner?.Creature?.CombatState;

        if (combatState == null)
            return true;

        if (!YuHuoService.HasYuHuo(card, combatState))
            return true;

        YuHuoCombatState state = YuHuoService.GetState(combatState);

        // Nested Exhaust calls produced by the auto-play must execute the real
        // game method. This is the recursion guard.
        if (!state.TryBeginResolution(card))
            return true;

        __result = YuHuoResolver.ResolveBeforeExhaust(
            choiceContext,
            card,
            combatState,
            causedByEthereal,
            skipVisuals);

        return false;
    }
}
