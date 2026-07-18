using CanAoNative.Rules.StarMoon;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Relics;

/// <summary>
/// 孤王玉座：回合结束时，若你手牌为空，下一回合开始时
/// 获得 1 费和一张星月合击+。
/// </summary>
public sealed class GuWangYuZuoRelic : RelicModel
{
    private bool _handWasEmptyAtTurnEnd;

    public override RelicRarity Rarity => RelicRarity.Rare;

    public override Task AfterSideTurnEndLate(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        _handWasEmptyAtTurnEnd =
            side == Owner.Creature.Side
            && participants.Contains(Owner.Creature)
            && Owner.PlayerCombatState.Hand.IsEmpty;

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

        await PlayerCmd.GainEnergy(1m, Owner);

        await StarMoonService.Generate(
            choiceContext,
            Owner,
            1,
            applier: null,
            cardSource: null,
            upgraded: true);
    }
}
