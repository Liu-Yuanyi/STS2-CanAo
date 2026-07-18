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
/// 青鸾羽衣：在你的回合开始时，若敌方攻击后你仍剩余至少 5 点格挡，
/// 获得 1 月。敌方 side-turn 结束（敌人已行动完）是"被打完之后"的
/// 快照点；己方回合开始时结算。
/// </summary>
public sealed class QingLuanYuYiRelic : RelicModel
{
    private bool _qualifiedAfterEnemyTurn;

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
        // The enemy's side-turn end is "after the enemy finished attacking".
        // Evaluating here captures the block that actually survived.
        if (side != Owner.Creature.Side)
        {
            _qualifiedAfterEnemyTurn =
                Owner.Creature.Block >= DynamicVars.Block.BaseValue;
        }

        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (!_qualifiedAfterEnemyTurn
            || !ReferenceEquals(player, Owner))
        {
            return;
        }

        _qualifiedAfterEnemyTurn = false;
        Flash();

        await PowerCmd.Apply<MoonPower>(
            choiceContext,
            Owner.Creature,
            1m,
            Owner.Creature,
            null);
    }
}
