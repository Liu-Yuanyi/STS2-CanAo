using CanAoNative.Relics;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Pools;

/// <summary>
/// 残傲专属遗物池。
/// </summary>
public sealed class CanAoRelicPool : RelicPoolModel
{
    public override string EnergyColorName => "regent";

    protected override RelicModel[] GenerateAllRelics() =>
    [
        ModelDb.Relic<DiGuoNianBiaoRelic>(),
        ModelDb.Relic<DiGuoShiCeRelic>(),
        ModelDb.Relic<TianFengJunYinRelic>(),
        ModelDb.Relic<QingLuanYuYiRelic>(),
        ModelDb.Relic<HeJiWuDianRelic>(),
        ModelDb.Relic<NiePanHuoZhongRelic>(),
        ModelDb.Relic<ZhanBeiRelic>(),
        ModelDb.Relic<GuWangYuZuoRelic>(),
        ModelDb.Relic<DiGuoShuiQiRelic>()
    ];
}
