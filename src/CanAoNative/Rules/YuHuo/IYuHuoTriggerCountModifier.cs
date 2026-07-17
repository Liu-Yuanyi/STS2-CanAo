using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Rules.YuHuo;

/// <summary>
/// Extension point for cards, powers and relics that modify how many times
/// one 浴火 event resolves. Implementations must be deterministic and should
/// not mutate combat state while the count is being calculated.
/// </summary>
public interface IYuHuoTriggerCountModifier
{
    int ModifyYuHuoTriggerCount(CardModel card, int currentCount);
}
