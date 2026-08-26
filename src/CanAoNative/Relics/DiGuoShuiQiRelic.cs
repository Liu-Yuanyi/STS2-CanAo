using CanAoNative.Cards;
using CanAoNative.Rules.Edict;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;

namespace CanAoNative.Relics;

/// <summary>
/// 帝国税契（v12 重做，2026-08-16 修正）：每场战斗开始时，将 1 张诏令
/// 加入手牌。战斗结束时，敌人额外掉落等同于本场战斗中你打出过的
/// 诏令数目的金币——走战斗奖励系统（TryModifyRewards + GoldReward，
/// 参考原生紫水晶茄子），不再是战斗结束瞬间直接加金币。
/// 打出计数经 EdictService 事件层（IAfterEdictPlayed）。
/// </summary>
public sealed class DiGuoShuiQiRelic :
    RelicModel,
    IAfterEdictPlayed
{
    private bool _combatStartPending;
    private int _edictsPlayedThisCombat;

    public override RelicRarity Rarity => RelicRarity.Shop;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<EdictCard>()
    ];

    public override Task BeforeCombatStart()
    {
        _combatStartPending = true;
        _edictsPlayedThisCombat = 0;
        return Task.CompletedTask;
    }

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!_combatStartPending
            || side != Owner.Creature.Side)
        {
            return;
        }

        _combatStartPending = false;
        Flash();

        await EdictService.Generate(
            choiceContext,
            Owner,
            1);
    }

    public Task AfterEdictPlayed(
        PlayerChoiceContext choiceContext,
        EdictPlayedContext context)
    {
        if (context.Player != Owner)
            return Task.CompletedTask;

        _edictsPlayedThisCombat++;
        return Task.CompletedTask;
    }

    public override bool TryModifyRewards(
        Player player,
        List<Reward> rewards,
        AbstractRoom? room)
    {
        if (player != Owner
            || room == null
            || !room.RoomType.IsCombatRoom()
            || _edictsPlayedThisCombat <= 0)
        {
            return false;
        }

        rewards.Add(new GoldReward(_edictsPlayedThisCombat, player));
        return true;
    }

    public override Task AfterModifyingRewards()
    {
        Flash();
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        _edictsPlayedThisCombat = 0;
        return Task.CompletedTask;
    }
}
