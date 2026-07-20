using CanAoNative.Cards;
using CanAoNative.Rules.StarMoon;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace CanAoNative.Relics;

/// <summary>
/// 孤王玉座：回合结束时，若你手牌为空，下一回合开始时
/// 获得 1 费和一张星月合击+。
/// </summary>
public sealed class GuWangYuZuoRelic : RelicModel
{
    private bool _handWasEmptyAtTurnEnd;

    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<StarMoonStrike>(upgrade: true),
        HoverTipFactory.Static(StaticHoverTip.Energy)
    ];

    /// <summary>
    /// BeforeSideTurnEnd runs before the hand is flushed at turn end
    /// (FlushPlayerHand happens later in EndPlayerTurnPhaseTwoInternal),
    /// so it is the only hook where "hand is empty at turn end" can be
    /// evaluated truthfully.
    /// </summary>
    public override Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side == Owner.Creature.Side
            && participants.Contains(Owner.Creature))
        {
            _handWasEmptyAtTurnEnd =
                Owner.PlayerCombatState.Hand.IsEmpty;
        }

        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (!_handWasEmptyAtTurnEnd
            || !ReferenceEquals(player, Owner))
        {
            return;
        }

        _handWasEmptyAtTurnEnd = false;
        Flash();

        await PlayerCmd.GainEnergy(
            DynamicVars.Energy.IntValue,
            Owner);

        await StarMoonService.Generate(
            choiceContext,
            Owner,
            1,
            applier: null,
            cardSource: null,
            upgraded: true);
    }

    public override Task BeforeCombatStart()
    {
        _handWasEmptyAtTurnEnd = false;
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        _handWasEmptyAtTurnEnd = false;
        return Task.CompletedTask;
    }
}
