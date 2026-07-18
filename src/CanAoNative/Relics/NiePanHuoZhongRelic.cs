using CanAoNative.Rules.YuHuo;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace CanAoNative.Relics;

/// <summary>
/// 涅槃火种：每场战斗第一次浴火结算时，额外触发一次效果。
/// The trigger-count extension runs before the first trigger of the first
/// 浴火 resolution each combat, then latches until combat end.
/// </summary>
public sealed class NiePanHuoZhongRelic :
    RelicModel,
    IYuHuoTriggerCountModifier
{
    private bool _triggeredThisCombat;

    public override RelicRarity Rarity => RelicRarity.Rare;

    public int ModifyYuHuoTriggerCount(
        CardModel card,
        int currentCount)
    {
        if (_triggeredThisCombat || card.Owner != Owner)
            return currentCount;

        _triggeredThisCombat = true;
        Flash();

        return currentCount + 1;
    }

    public override Task BeforeCombatStart()
    {
        _triggeredThisCombat = false;
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        _triggeredThisCombat = false;
        return Task.CompletedTask;
    }
}
