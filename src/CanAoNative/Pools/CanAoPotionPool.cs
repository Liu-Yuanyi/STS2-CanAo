using CanAoNative.Potions;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Pools;

/// <summary>
/// 残傲专属药水池。
/// </summary>
public sealed class CanAoPotionPool : PotionPoolModel
{
    public override string EnergyColorName => "regent";

    protected override PotionModel[] GenerateAllPotions() =>
    [
        ModelDb.Potion<QiongJiangPotion>(),
        ModelDb.Potion<FengWeiJiuPotion>(),
        ModelDb.Potion<YuLingPingPotion>()
    ];
}
