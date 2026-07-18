using CanAoNative.Rules.YuHuo;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Powers;

/// <summary>
/// 浴火军旗：每次成功的浴火触发后，给予 Amount 点本回合力量。
/// The temporary Strength lifecycle is delegated to the game's native
/// TemporaryStrengthPower implementation.
/// </summary>
public sealed class YuHuoBannerPower :
    PowerModel,
    IAfterYuHuoTrigger
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task AfterYuHuoTrigger(
        PlayerChoiceContext choiceContext,
        YuHuoExecutionContext context)
    {
        if (!context.EffectExecuted || Amount <= 0)
            return;

        Flash();

        await PowerCmd.Apply<YuHuoBannerTemporaryStrengthPower>(
            choiceContext,
            Owner,
            Amount,
            Owner,
            context.Card);
    }
}
