using CanAoNative.Rules.StarMoon;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Powers;

/// <summary>
/// 星月轮转（v12 新增）：每回合玩家生成第 2 张星月合击时，
/// 获得 Amount 点能量。叠加语义：层数 = 获得的能量数
/// （Counter，R11 叠加统一）。
/// </summary>
public sealed class XingYueLunZhuanPower :
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

        // RecordGenerated 在 AfterStarMoonGenerated 之前完成，
        // 计数已包含当前这张合击。
        if (StarMoonService.GetGeneratedThisTurn(context.Player) != 2)
            return;

        Flash();

        await PlayerCmd.GainEnergy(Amount, context.Player);
    }
}
