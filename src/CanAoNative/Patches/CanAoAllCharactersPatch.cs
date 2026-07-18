using CanAoNative.Characters;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Patches;

/// <summary>
/// Appends 残傲 to the hardcoded character list so she appears on the
/// character select screen. UnlockState.Characters derives from this list
/// and only removes vanilla characters, so she is unlocked by default.
/// </summary>
[HarmonyPatch(typeof(ModelDb), nameof(ModelDb.AllCharacters), MethodType.Getter)]
public static class CanAoAllCharactersPatch
{
    public static void Postfix(ref IEnumerable<CharacterModel> __result)
    {
        if (__result.Any(character => character is CanAo))
            return;

        __result = __result.Concat([ModelDb.Character<CanAo>()]);
    }
}
