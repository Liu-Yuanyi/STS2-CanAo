using CanAoNative.Rules.Edict;
using CanAoNative.Rules.FengWei;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Powers;

/// <summary>
/// 帝国余威：每回合第二次打出诏令时，获得 Amount 点凤威。
/// </summary>
public sealed class DiGuoYuWeiPower :
    PowerModel,
    IAfterEdictPlayed
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task AfterEdictPlayed(
        PlayerChoiceContext choiceContext,
        EdictPlayedContext context)
    {
        if (!ReferenceEquals(context.Player.Creature, Owner)
            || Amount <= 0)
        {
            return;
        }

        if (EdictService.GetPlayedThisTurn(context.Player) != 2)
            return;

        Flash();

        await FengWeiService.GainPermanent(
            choiceContext,
            context.Player,
            Amount,
            cardSource: null);
    }
}
