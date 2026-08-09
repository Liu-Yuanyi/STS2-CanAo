using CanAoNative.Cards;
using Godot;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Pools;

/// <summary>
/// 残傲衍生牌卡池。使用和原版 TokenCardPool 一样的无色牌框与白色卡背，
/// 使星月合击、诏令、火刃在视觉上和小刀、巨石等衍生牌统一。
/// CardRarity.Token 确保这些牌不会出现在奖励掉落中。
/// </summary>
public sealed class CanAoTokenPool : CardPoolModel
{
    public override string Title => "can_ao_token";
    public override string EnergyColorName => "colorless";
    public override string CardFrameMaterialPath => "card_frame_colorless";
    public override Color DeckEntryCardColor => Colors.White;
    public override bool IsColorless => true;

    protected override CardModel[] GenerateAllCards() =>
    [
        // Card registration is handled by CanAoCardPool (for compendium
        // visibility). This pool exists solely to provide colorless
        // frame / energy visuals matching native token cards.
    ];
}
