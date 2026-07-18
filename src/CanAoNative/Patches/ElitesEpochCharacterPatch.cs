using CanAoNative.Characters;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Saves.Managers;

namespace CanAoNative.Patches;

/// <summary>
/// CheckFifteenElitesDefeatedEpoch is a hardcoded vanilla character chain
/// that throws ArgumentOutOfRangeException for unknown characters, killing
/// the elite reward flow. 残傲 has no epoch to unlock here, so skip her.
/// </summary>
[HarmonyPatch(typeof(ProgressSaveManager), "CheckFifteenElitesDefeatedEpoch")]
public static class ElitesEpochCharacterPatch
{
    public static bool Prefix(Player localPlayer)
    {
        return localPlayer.Character is not CanAo;
    }
}
