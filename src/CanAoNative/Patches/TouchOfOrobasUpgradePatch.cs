using CanAoNative.Relics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace CanAoNative.Patches;

/// <summary>
/// Teaches Touch of Orobas the 帝国年表 → 帝国史册 refinement, otherwise
/// unknown starter relics fall back to Circlet.
/// </summary>
[HarmonyPatch(typeof(TouchOfOrobas), "GetUpgradedStarterRelic")]
public static class TouchOfOrobasUpgradePatch
{
    public static void Postfix(
        RelicModel starterRelic,
        ref RelicModel __result)
    {
        if (starterRelic is DiGuoNianBiaoRelic)
            __result = ModelDb.Relic<DiGuoShiCeRelic>().ToMutable();
    }
}
