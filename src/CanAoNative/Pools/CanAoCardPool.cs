using CanAoNative.Cards;
using Godot;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Pools;

/// <summary>
/// 残傲专属卡池。牌框与能量图标暂复用游戏原生资源（橙色牌框）。
/// </summary>
public sealed class CanAoCardPool : CardPoolModel
{
    public override string Title => "can_ao";
    public override string EnergyColorName => "regent";
    public override string CardFrameMaterialPath => "card_frame_orange";
    public override Color DeckEntryCardColor => new("C88000");
    public override Color EnergyOutlineColor => new("804820");
    public override bool IsColorless => false;

    protected override CardModel[] GenerateAllCards() =>
    [
        ModelDb.Card<CanAoStrikeCard>(),
        ModelDb.Card<CanAoDefendCard>(),
        ModelDb.Card<FengYuCanHuoCard>(),
        ModelDb.Card<JiHuoCard>(),
        ModelDb.Card<RanYuTuXiCard>(),
        ModelDb.Card<GuJianDaJiCard>(),
        ModelDb.Card<YueZhanCard>(),
        ModelDb.Card<QinWeiFengZhaoCard>(),
        ModelDb.Card<YuQianCaiJueCard>(),
        ModelDb.Card<QingLuanGouFaCard>(),
        ModelDb.Card<ZheYiFanJiCard>(),
        ModelDb.Card<JiuWangFuLinCard>(),
        ModelDb.Card<BaoHuoCard>(),
        ModelDb.Card<QuShiBaiZhongCard>(),
        ModelDb.Card<GuiLunCard>(),
        ModelDb.Card<YueHuaPingZhangCard>(),
        ModelDb.Card<XingHuiHuZhenCard>(),
        ModelDb.Card<GuanXingWenYueCard>(),
        ModelDb.Card<FenYuShouShiCard>(),
        ModelDb.Card<GuYueMingCard>(),
        ModelDb.Card<YuHuoSlashCard>(),
        ModelDb.Card<SacrificialPreparationCard>(),
        ModelDb.Card<FengYanBuXiCard>(),
        ModelDb.Card<FeatherRanksCard>(),
        ModelDb.Card<YuHuoBannerCard>(),
        ModelDb.Card<ShiWeiCard>(),
        ModelDb.Card<ZanBiFengMangCard>(),
        ModelDb.Card<PanXuanCard>(),
        ModelDb.Card<XingYueFaMoCard>(),
        ModelDb.Card<TianFengJunZhenCard>(),
        ModelDb.Card<ZhengZhaoCard>(),
        ModelDb.Card<YuHuoStrikeCard>(),
        ModelDb.Card<FenGaoJiGuiCard>(),
        ModelDb.Card<QingGongCard>(),
        ModelDb.Card<FengGuZaiRanCard>(),
        ModelDb.Card<ChuanLingCard>(),
        ModelDb.Card<MiZhaoCard>(),
        ModelDb.Card<WangQuanCard>(),
        ModelDb.Card<DiGuoYuWeiCard>(),
        ModelDb.Card<ChengTianShouMingCard>(),
        ModelDb.Card<TianFengXingTaiCard>(),
        ModelDb.Card<XingYueWangGuanCard>()
    ];
}
