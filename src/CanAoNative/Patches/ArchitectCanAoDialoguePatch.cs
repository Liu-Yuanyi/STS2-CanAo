using CanAoNative.Characters;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;

namespace CanAoNative.Patches;

/// <summary>
/// TheArchitect.DefineDialogues is hardcoded to vanilla characters. This
/// Postfix adds 残傲's dialogue entries so her victory visit gets the full
/// architect dialogue instead of an empty proceed button. Line text comes
/// from ancients.json via the native PopulateLocKeys pass that runs after
/// DefineDialogues; speakers and next-button labels follow the same
/// X-Y.ancient/.char/.next key convention as vanilla.
/// </summary>
[HarmonyPatch(typeof(TheArchitect), "DefineDialogues")]
public static class ArchitectCanAoDialoguePatch
{
    public static void Postfix(AncientDialogueSet __result)
    {
        string charKey =
            ModelDb.GetId(typeof(CanAo)).Entry;

        __result.CharacterDialogues[charKey] =
        [
            new AncientDialogue("", "", "")
            {
                VisitIndex = 0,
                EndAttackers = ArchitectAttackers.Both
            },
            new AncientDialogue("", "")
            {
                VisitIndex = 1,
                EndAttackers = ArchitectAttackers.Both
            },
            new AncientDialogue("", "")
            {
                VisitIndex = 2,
                EndAttackers = ArchitectAttackers.Both
            }
        ];
    }
}
