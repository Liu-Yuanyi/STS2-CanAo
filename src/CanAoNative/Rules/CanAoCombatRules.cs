using CanAoNative.Cards;
using CanAoNative.Powers;
using CanAoNative.Rules.StarMoon;
using CanAoNative.Rules.YuHuo;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Rules;

/// <summary>
/// Central combat hook listener for rules that are not owned by one card.
/// </summary>
public sealed class CanAoCombatRules : AbstractModel
{
    public override bool ShouldReceiveCombatHooks => true;

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (amount <= 0
            || power is not (StarPower or MoonPower)
            || power.Owner.Player is not Player player)
        {
            return;
        }

        await StarMoonHelper.CheckAndResolve(
            choiceContext,
            player,
            applier,
            cardSource);
    }

    public override async Task AfterCardPlayedLate(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (cardPlay.Card is not StarMoonStrike strike
            || strike.Owner is not Player player
            || player.Creature.CombatState is not ICombatState combatState)
        {
            return;
        }

        StarMoonService.RecordPlayed(
            combatState,
            player);

        await StarMoonService.NotifyAfterPlayed(
            choiceContext,
            new StarMoonPlayedContext(
                player,
                strike,
                cardPlay));
    }

    /// <summary>
    /// Clear turn-only Mod state only after the complete side-turn pipeline.
    /// This intentionally preserves temporary YuHuo through Ethereal exhaust
    /// and allows normal after-turn listeners to inspect Star-Moon history.
    /// </summary>
    public override Task AfterSideTurnEndLate(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        List<Creature> participantList = participants.ToList();

        List<Player> endingPlayers = participantList
            .Where(creature => creature.IsPlayer)
            .Select(creature => creature.Player)
            .OfType<Player>()
            .ToList();

        ICombatState? combatState =
            participantList.FirstOrDefault()?.CombatState;

        if (combatState != null && endingPlayers.Count > 0)
        {
            YuHuoService
                .GetState(combatState)
                .RemoveExpiredForPlayers(endingPlayers);

            StarMoonService.ClearTurnForPlayers(
                combatState,
                endingPlayers);
        }

        return Task.CompletedTask;
    }
}
