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
    public override string EnergyColorName => "can_ao";
    public override string CardFrameMaterialPath => "card_frame_canao";
    public override Color DeckEntryCardColor => new("A6DFFF");
    public override Color EnergyOutlineColor => new("0E2A3F");
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
        // QingGongCard removed — moved to 弃稿.
        // v12: WangShiQinZhengCard / GuYueMingCard / ChengTianShouMingCard removed — 弃稿.
        ModelDb.Card<FengGuZaiRanCard>(),
        ModelDb.Card<LieKongCard>(),
        ModelDb.Card<FengYanLianZhanCard>(),
        ModelDb.Card<ChuQiaoCard>(),
        ModelDb.Card<JinYuHuiXuanCard>(),
        ModelDb.Card<WangWeiYaJingCard>(),
        ModelDb.Card<HuiJinFengBaoCard>(),
        ModelDb.Card<PoMoRenCard>(),
        ModelDb.Card<PoZhenCard>(),
        ModelDb.Card<ZhuiXingWeiYueCard>(),
        ModelDb.Card<ZhaoYueChengXingCard>(),
        ModelDb.Card<YuanJunCard>(),
        ModelDb.Card<JuJingHuiShenCard>(),
        ModelDb.Card<CuiHuoCard>(),
        ModelDb.Card<ChangMingCard>(),
        ModelDb.Card<BuZhaoCard>(),
        ModelDb.Card<FengHuoCard>(),
        ModelDb.Card<WanLingQiFaCard>(),
        ModelDb.Card<SuiYueYiJiCard>(),
        ModelDb.Card<ZuiHouYiWuCard>(),
        ModelDb.Card<QianYuLieCard>(),
        ModelDb.Card<XingYueZhongShiCard>(),
        ModelDb.Card<FengWaJueZhanCard>(),
        ModelDb.Card<ChenTuZhiZhanCard>(),
        ModelDb.Card<MieShiCard>(),
        ModelDb.Card<ZheGuanCard>(),
        ModelDb.Card<BuSiFengQuCard>(),
        ModelDb.Card<HuiYaoCard>(),
        ModelDb.Card<ManMuXingChenCard>(),
        ModelDb.Card<FengLinJiuTianCard>(),
        ModelDb.Card<FuBiCard>(),
        ModelDb.Card<NiePanCard>(),
        ModelDb.Card<YanQingBaFangCard>(),
        ModelDb.Card<DiGuoDaJiCard>(),
        ModelDb.Card<FenJueCard>(),
        ModelDb.Card<GuiYinYunShanCard>(),
        ModelDb.Card<ChuanLingCard>(),
        ModelDb.Card<MiZhaoCard>(),
        ModelDb.Card<WangQuanCard>(),
        ModelDb.Card<DiGuoYuWeiCard>(),
        ModelDb.Card<TianFengXingTaiCard>(),
        ModelDb.Card<XingYueWangGuanCard>(),
        ModelDb.Card<BuDuoCard>(),
        ModelDb.Card<FengHunCard>(),
        ModelDb.Card<JiaoHuiCard>(),
        ModelDb.Card<ShouQueCard>(),
        ModelDb.Card<DengJiCard>(),
        ModelDb.Card<FengHuoJunXieCard>(),
        ModelDb.Card<ZhongZhangCard>(),
        ModelDb.Card<WaMoYuanZhengCard>(),
        ModelDb.Card<WangZuoGuMingCard>(),
        ModelDb.Card<BuMieWangChaoCard>(),
        ModelDb.Card<WanBangLaiChaoCard>(),
        // v12 新增
        ModelDb.Card<AnJianCard>(),
        ModelDb.Card<XingYueLunZhuanCard>(),

        // Derivative cards (CardRarity.Token) — registered here for
        // compendium visibility. Visuals use CanAoTokenPool (colorless frame).
        ModelDb.Card<StarMoonStrike>(),
        ModelDb.Card<EdictCard>(),
        ModelDb.Card<HuoRenCard>()
    ];
}
