using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Powers;

/// <summary>
/// Abstract base for powers that grant a resource at the start of the next turn.
/// Each resource type must have its own derived class (separate Power ID),
/// so PowerCmd.Apply stacking works correctly per resource type.
///
/// Derived classes override <see cref="ApplyResource"/> to grant the specific
/// resource (Star, Moon, FengWei, etc.).
/// </summary>
public abstract class DeferredResourcePowerBase : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>
    /// Grant the specific resource. Called once at the start of the owner's
    /// next turn, before the power silently zeros itself out.
    /// </summary>
    protected abstract Task ApplyResource(
        PlayerChoiceContext choiceContext);

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (!ReferenceEquals(player.Creature, Owner) || Amount <= 0)
            return;

        // Grant the resource once per stack.
        for (int i = 0; i < (int)Amount; i++)
        {
            await ApplyResource(choiceContext);
        }

        // Silently zero out so the UI doesn't play a stuck removal animation.
        await PowerCmd.ModifyAmount(
            choiceContext,
            this,
            -Amount,
            Owner,
            cardSource: null,
            silent: true);
    }
}
