using CanAoNative.Cards;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace CanAoNative.Patches;

/// <summary>
/// Teaches the Archaic Tooth its 祭火 → 焚诀 transcendence, so the ancient
/// starter-card replacement works for 残傲 like Bash → Break.
/// </summary>
[HarmonyPatch(typeof(ArchaicTooth), "TranscendenceUpgrades", MethodType.Getter)]
public static class ArchaicToothTranscendencePatch
{
    public static void Postfix(
        ref Dictionary<ModelId, CardModel> __result)
    {
        __result[ModelDb.Card<JiHuoCard>().Id] =
            ModelDb.Card<FenJueCard>();
    }
}
