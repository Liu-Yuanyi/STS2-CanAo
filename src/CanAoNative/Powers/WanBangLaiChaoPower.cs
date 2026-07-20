using CanAoNative.Rules.StarMoon;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CanAoNative.Powers;

/// <summary>
/// 万邦来朝：每生成一张星月合击，对所有敌人施加 Amount 层虚弱。
/// 多张叠加时虚弱层数合并进同一个 Power amount。
/// </summary>
public sealed class WanBangLaiChaoPower :
    PowerModel,
    IAfterStarMoonGenerated
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task AfterStarMoonGenerated(
        PlayerChoiceContext choiceContext,
        StarMoonGenerationContext context)
    {
        ICombatState? combatState = Owner.CombatState;

        if (!ReferenceEquals(context.Player.Creature, Owner)
            || Amount <= 0
            || combatState == null)
        {
            return;
        }

        Flash();

        await PowerCmd.Apply<WeakPower>(
            choiceContext,
            combatState.HittableEnemies,
            Amount,
            Owner,
            cardSource: null);
    }
}
