using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace CanAoNative.Rules;

/// <summary>
/// Hover tips for CanAo concepts that are not native keywords. Cards that
/// currently have 浴火 receive the 浴火 tip automatically from
/// YuHuoHoverTipPatch; cards that only mention 浴火 in their rules text add
/// it through <see cref="YuHuo"/>.
/// </summary>
public static class CanAoHoverTips
{
    public static IHoverTip YuHuo { get; } = new HoverTip(
        new LocString("static_hover_tips", "YU_HUO.title"),
        new LocString("static_hover_tips", "YU_HUO.description"));
}
