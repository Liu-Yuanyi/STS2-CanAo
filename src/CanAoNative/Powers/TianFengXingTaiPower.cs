using CanAoNative.Rules.Edict;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Powers;

/// <summary>
/// 天凤形态：每回合开始时，将 Amount 张诏令加入手牌。
/// </summary>
public sealed class TianFengXingTaiPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (!ReferenceEquals(player.Creature, Owner)
            || Amount <= 0)
        {
            return;
        }

        Flash();

        await EdictService.Generate(
            choiceContext,
            player,
            (int)Amount);
    }
}
