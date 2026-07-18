using CanAoNative.Characters;
using CanAoNative.Pools;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;

namespace CanAoNative.Patches;

/// <summary>
/// The card library's pool filter buttons are hardcoded for vanilla
/// characters. This clones the colorless filter, labels it for 残傲 and
/// registers it with the same toggle pipeline so her pool gets a category.
/// </summary>
[HarmonyPatch(typeof(NCardLibrary), "_Ready")]
public static class CanAoCardLibraryFilterPatch
{
    public static void Postfix(NCardLibrary __instance)
    {
        Traverse traverse = Traverse.Create(__instance);

        var poolFilters =
            traverse.Field<Dictionary<NCardPoolFilter, Func<CardModel, bool>>>(
                "_poolFilters").Value;

        var cardPoolFilters =
            traverse.Field<Dictionary<CharacterModel, NCardPoolFilter>>(
                "_cardPoolFilters").Value;

        NCardPoolFilter? source =
            traverse.Field<NCardPoolFilter>("_colorlessFilter").Value;

        if (poolFilters == null
            || cardPoolFilters == null
            || source == null
            || cardPoolFilters.ContainsKey(ModelDb.Character<CanAo>()))
        {
            return;
        }

        NCardPoolFilter clone =
            (NCardPoolFilter)source.Duplicate();

        clone.Name = "CanAoPool";
        clone.Loc = new LocString("characters", "CAN_AO.title");

        source.GetParent().AddChild(clone);

        clone.Connect(
            NCardPoolFilter.SignalName.Toggled,
            new Callable(__instance, "UpdateCardPoolFilter"));

        poolFilters.Add(
            clone,
            card => card.Pool is CanAoCardPool);

        cardPoolFilters.Add(ModelDb.Character<CanAo>(), clone);
    }
}
