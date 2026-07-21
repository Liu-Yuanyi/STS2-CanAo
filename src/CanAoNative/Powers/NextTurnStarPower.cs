using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace CanAoNative.Powers;

/// <summary>
/// 下回合开始时获得 2 星（每层Amount）。由照月成星及其他卡牌复用。
/// </summary>
public sealed class NextTurnStarPower : DeferredResourcePowerBase
{
    protected override async Task ApplyResource(
        PlayerChoiceContext choiceContext)
    {
        await PowerCmd.Apply<StarPower>(
            choiceContext,
            Owner,
            2m,
            Owner,
            cardSource: null);
    }
}
