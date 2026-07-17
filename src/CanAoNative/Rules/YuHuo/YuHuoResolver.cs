using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Rules.YuHuo;

/// <summary>
/// Resolves 浴火 entirely inside the original CardCmd.Exhaust Task chain.
/// The resolver owns ordering and error recovery, while concrete cards,
/// powers and relics extend behavior through the YuHuo event interfaces.
/// </summary>
public static class YuHuoResolver
{
    private static readonly Logger Log =
        new("CanAoNative", LogType.Generic);

    public static async Task ResolveBeforeExhaust(
        PlayerChoiceContext choiceContext,
        CardModel card,
        ICombatState combatState,
        bool causedByEthereal,
        bool skipVisuals)
    {
        YuHuoCombatState state = YuHuoService.GetState(combatState);
        PileType? originalPile = card.Pile?.Type;

        try
        {
            // A temporary grant could theoretically expire between the Prefix
            // check and this async body. Since the Prefix already skipped the
            // original method, fall back to a normal Exhaust in that case.
            if (!YuHuoService.HasYuHuo(card, combatState))
            {
                await CardCmd.Exhaust(
                    choiceContext,
                    card,
                    causedByEthereal,
                    skipVisuals);
                return;
            }

            int triggerCount =
                YuHuoService.GetYuHuoTriggerCount(card);

            YuHuoResolutionContext resolutionContext = new(
                card,
                combatState,
                triggerCount,
                causedByEthereal,
                originalPile);

#if DEBUG
            Log.Info(
                $"YUHUO_RESOLVE: card={card.Id}, " +
                $"triggers={triggerCount}, pile={originalPile}");
#endif

            await YuHuoService.NotifyBeforeResolved(
                choiceContext,
                resolutionContext);

            for (int i = 0; i < triggerCount; i++)
            {
                // TriggerIndex is intentionally one-based for card/relic rules
                // and human-readable diagnostics.
                YuHuoExecutionContext triggerContext = new(
                    card,
                    triggerIndex: i + 1,
                    triggerCount: triggerCount,
                    causedByEthereal: causedByEthereal,
                    originalPile: originalPile);

                state.BeginTrigger(triggerContext);

                void MarkPlayed() =>
                    triggerContext.MarkEffectExecuted();

                card.Played += MarkPlayed;

                try
                {
                    await YuHuoService.NotifyBeforeTrigger(
                        choiceContext,
                        triggerContext);

                    await CardCmd.AutoPlay(
                        choiceContext,
                        card,
                        target: null,
                        type: AutoPlayType.Default,
                        skipXCapture: false,
                        skipCardPileVisuals: skipVisuals);

                    await YuHuoService.NotifyAfterTrigger(
                        choiceContext,
                        triggerContext);
                }
                finally
                {
                    card.Played -= MarkPlayed;
                    state.EndTrigger(card);
                }
            }

            if (card.Pile?.Type != PileType.Exhaust)
            {
                // The recursion guard is still active, so this nested call
                // executes the original CardCmd.Exhaust implementation.
                await CardCmd.Exhaust(
                    choiceContext,
                    card,
                    causedByEthereal,
                    skipVisuals);
            }

            await YuHuoService.NotifyAfterResolved(
                choiceContext,
                resolutionContext);
        }
        catch (Exception ex)
        {
            Log.Error(
                $"YUHUO_RESOLVE_FAILED: card={card.Id}\n{ex}");

            // Best-effort repair while the recursion guard is still active.
            if (card.Pile?.Type != PileType.Exhaust)
            {
                try
                {
                    await CardCmd.Exhaust(
                        choiceContext,
                        card,
                        causedByEthereal,
                        skipVisuals);
                }
                catch (Exception fallbackEx)
                {
                    Log.Error(
                        $"YUHUO_FALLBACK_EXHAUST_FAILED: " +
                        $"card={card.Id}\n{fallbackEx}");
                }
            }

            // Do not hide a failed game action behind a successful Task.
            throw;
        }
        finally
        {
            state.EndResolution(card);
        }
    }
}
