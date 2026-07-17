using CanAoNative.Rules.StarMoon;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Powers;

/// <summary>
/// 盘旋：本回合每生成一张星月合击，获得 Amount 点格挡。
/// Multiple applications add their per-generation block values together.
/// </summary>
public sealed class PanXuanPower :
    PowerModel,
    IAfterStarMoonGenerated
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task AfterStarMoonGenerated(
        PlayerChoiceContext choiceContext,
        StarMoonGenerationContext context)
    {
        if (!ReferenceEquals(context.Player.Creature, Owner)
            || Amount <= 0)
        {
            return;
        }

        Flash();

        await CreatureCmd.GainBlock(
            Owner,
            Amount,
            ValueProp.Unpowered,
            (CardPlay?)null);
    }

    public override async Task AfterSideTurnEndLate(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (participants.All(creature => creature != Owner)
            || Amount == 0)
        {
            return;
        }

        await PowerCmd.ModifyAmount(
            choiceContext,
            this,
            -Amount,
            Owner,
            cardSource: null,
            silent: true);
    }
}
