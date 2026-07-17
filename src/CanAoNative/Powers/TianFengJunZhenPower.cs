using CanAoNative.Rules.StarMoon;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Powers;

/// <summary>
/// 天凤军阵：每生成一张星月合击，对所有敌人造成 Amount 点非攻击伤害。
/// Multiple copies stack by adding their damage values into one Power amount.
/// </summary>
public sealed class TianFengJunZhenPower :
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

        await CreatureCmd.Damage(
            choiceContext,
            combatState.HittableEnemies,
            Amount,
            ValueProp.Unpowered,
            Owner);
    }
}
