using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace CanAoNative.Patches;

/// <summary>
/// TheArchitect.DefineDialogues is hardcoded to vanilla characters, so a
/// character without a dialogue set gets Dialogue == null and the original
/// WinRun dereferences it (Dialogue.EndAttackers) — the victory flow dies
/// on a NullReferenceException. When there is no dialogue, this Prefix
/// performs the completion essentials (multiplayer wait overlay +
/// act-change readiness) and skips the original method. Characters with a
/// dialogue are untouched.
/// </summary>
[HarmonyPatch(typeof(TheArchitect), "WinRun")]
public static class ArchitectWinRunPatch
{
    public static bool Prefix(TheArchitect __instance)
    {
        if (Traverse.Create(__instance)
                .Field("_dialogue")
                .GetValue<object>() != null)
        {
            return true;
        }

        Player? owner = __instance.Owner;

        if (owner == null || !LocalContext.IsMe(owner))
            return false;

        if (owner.RunState.Players.Count > 1)
        {
            NCombatRoom.Instance?
                .SetWaitingForOtherPlayersOverlayVisible(visible: true);
        }

        RunManager.Instance.ActChangeSynchronizer.SetLocalPlayerReady();
        return false;
    }
}
