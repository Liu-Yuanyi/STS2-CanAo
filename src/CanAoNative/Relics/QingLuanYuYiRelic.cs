using CanAoNative.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Relics;

/// <summary>
/// 青鸾羽衣：回合开始时，若上回合剩余至少 5 点格挡，获得 1 月。
/// Block is still intact at the owner's side-turn end and only clears at the
/// next turn start, so the snapshot happens in AfterSideTurnEndLate.
/// </summary>
public sealed class QingLuanYuYiRelic : RelicModel
{
    private bool _hadEnoughBlockLastTurn;

    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5m, ValueProp.Unpowered)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Block),
        HoverTipFactory.FromPower<MoonPower>()
    ];

    public override Task AfterSideTurnEndLate(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        // Only snapshot on the OWNER's side-turn end. Assigning on every
        // side-turn end would let the enemy's later turn end wipe the flag
        // before the owner's next turn starts.
        if (side == Owner.Creature.Side
            && participants.Contains(Owner.Creature))
        {
            _hadEnoughBlockLastTurn =
                Owner.Creature.Block >= DynamicVars.Block.BaseValue;
        }

        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (!_hadEnoughBlockLastTurn
            || !ReferenceEquals(player, Owner))
        {
            return;
        }

        _hadEnoughBlockLastTurn = false;
        Flash();

        await PowerCmd.Apply<MoonPower>(
            choiceContext,
            Owner.Creature,
            1m,
            Owner.Creature,
            null);
    }
}
