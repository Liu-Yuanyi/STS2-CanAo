using CanAoNative.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Powers;

/// <summary>
/// 凤火军械+：每回合抽牌前，将 Amount 张火刃+加入手牌（与无尽刀刃的 BeforeHandDraw 统一）。
/// 与 FengHuoJunXiePower 是两个独立 Power，
/// 未升级与升级版凤火军械分别叠层、分别产牌。
/// </summary>
public sealed class FengHuoJunXieUpgradedPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (player != Owner.Player
            || Amount <= 0)
        {
            return;
        }

        Flash();

        for (int i = 0; i < (int)Amount; i++)
        {
            HuoRenCard blade = combatState.CreateCard<HuoRenCard>(player);
            CardCmd.Upgrade(blade);

            await CardPileCmd.AddGeneratedCardToCombat(
                blade,
                PileType.Hand,
                player);
        }
    }
}
