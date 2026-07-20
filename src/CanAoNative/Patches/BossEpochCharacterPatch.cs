using CanAoNative.Characters;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Saves.Managers;

namespace CanAoNative.Patches;

/// <summary>
/// ObtainCharUnlockEpoch looks up "{CHARACTER}{act+2}_EPOCH" from the
/// hardcoded vanilla epoch registry. 残傲 has no registered epochs yet,
/// so the lookup throws ArgumentException and stalls the post-boss flow.
/// Skip her until dedicated epochs exist.
/// </summary>
[HarmonyPatch(typeof(ProgressSaveManager), "ObtainCharUnlockEpoch")]
public static class ObtainCharUnlockEpochPatch
{
    public static bool Prefix(Player localPlayer)
    {
        return localPlayer.Character is not CanAo;
    }
}

/// <summary>
/// CheckFifteenBossesDefeatedEpoch is a hardcoded vanilla character chain
/// that throws ArgumentOutOfRangeException for unknown characters, killing
/// the post-boss flow. 残傲 has no epoch to unlock here, so skip her.
/// </summary>
[HarmonyPatch(typeof(ProgressSaveManager), "CheckFifteenBossesDefeatedEpoch")]
public static class BossEpochCharacterPatch
{
    public static bool Prefix(Player localPlayer)
    {
        return localPlayer.Character is not CanAo;
    }
}
