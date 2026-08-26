# Power 图标选定记录（2026-08-09/10 用户拍板）

来源批次：batch72/R（v01 首轮）+ batch73（v02 重做）+ batch74/75（v01 追加）+
batch76（凤威/登基 v03 迭代，进行中）。
拼版：power_sheet_v01/v02/v03.png、redo_sheet_v02.png。母版 `art/powers/raw/`。

## 已定稿并实装（2026-08-10）

| Power | 文件 |
|---|---|
| 星 STAR_POWER | **art/manual/星.png（用户自制）** |
| 月 MOON_POWER | **art/manual/月.png（用户自制）** |
| 下回合星 NEXT_TURN_STAR_POWER | **art/manual/下回合星.png（用户自制）** |
| 凤威 FENG_WEI_POWER | power_fengwei_v03_A.png（**定稿 2026-08-15**） |
| 临时凤威 TEMPORARY_FENG_WEI_POWER | 程序派生：凤威降饱和 |
| 凤焰不息 FENG_YAN_BU_XI_POWER | power_fengyanbuxi_v02.png（**初稿**） |
| 浴火军旗 YU_HUO_BANNER_POWER | power_yuhuobanner_B_v01.png |
| 浴火军旗临时力量 YU_HUO_BANNER_TEMPORARY_STRENGTH_POWER | 程序派生：军旗降饱和 |
| 不堕 BU_DUO_POWER | power_buduo_A_v01.png |
| 交辉 JIAO_HUI_POWER | power_jiaohui_A_v01.png |
| 登基 DENG_JI_POWER | power_dengji_v03_D.png（**定稿 2026-08-15**） |
| 不死凤躯 BU_SI_FENG_QU_POWER | power_busifengqu_B_v01.png |
| 星月王冠 XING_YUE_WANG_GUAN_POWER | power_xingyuewangguan_A_v01.png |
| 天凤形态 TIAN_FENG_XING_TAI_POWER | power_tianfengxingtai_B_v01.png |
| 归隐陨山 GUI_YIN_YUN_SHAN_POWER | power_guiyinyunshan_A_v01.png |
| 凤魂 FENG_HUN_POWER | power_fenghun_B_v01.png |
| 守缺 SHOU_QUE_POWER | power_shouque_A_v01.png |
| 复辟 FU_BI_POWER | power_fubi_A_v01.png |
| 终章 ZHONG_ZHANG_POWER | power_zhongzhang_A_v01.png |
| 王座孤明 WANG_ZUO_GU_MING_POWER | power_wangzuoguming_A_v01.png |
| 不灭王朝 BU_MIE_WANG_CHAO_POWER | power_bumiewangchao_A_v01.png |
| 万邦来朝 WAN_BANG_LAI_CHAO_POWER | power_wanbanglaichao_A_v01.png |
| 盘旋 PAN_XUAN_POWER | power_panxuan_A_v01.png |
| 天凤军阵 TIAN_FENG_JUN_ZHEN_POWER | power_tianfengjunzhen_A_v01.png |
| 帝国余威 DI_GUO_YU_WEI_POWER | power_diguoyuwei_A_v01.png |
| 最后一舞 ZUI_HOU_YI_WU_POWER | power_zuihouyiwu_A_v01.png |
| 凤火军械 FENG_HUO_JUN_XIE_POWER | power_fenghuojunxie_A_v01.png |
| 凤火军械升级版 FENG_HUO_JUN_XIE_UPGRADED_POWER | 程序派生：军械金相偏移 |
| 瓦魔远征 WA_MO_YUAN_ZHENG_POWER | power_wamoyuanzheng_A_v01.png |
| 淬火临时力量 CUI_HUO_TEMPORARY_STRENGTH_POWER | power_cuihuo_A_v01.png（batch77 A 剑胚入水，**定稿 2026-08-15**，已实装） |
| 探针 CAN_AO_PROBE_POWER | 沿用原生占位（测试用，不制图） |

## 实装记录（2026-08-10）

- `scripts/finalize_icons.py`：裁方 1024 母版（`art/powers/processed/`）→
  `godot/images/powers/`（256）+ `powers/small/`（64）；4 张派生图同管线；
- 派生效果图 `art/powers/derived_sheet.png` 待用户过目（非审批）；
- 代码：`Patches/CanAoPowerIconPatch.cs` 由占位映射改为按 id 映射
  `res://images/powers/small/<id>.png` 与 `res://images/powers/<id>.png`，
  缺失自动回落原生占位（探针等不受影响）；
- 已 `Godot --import` + `Deploy-Mod.ps1`，pck 核验齐全。

| 星月轮转 XING_YUE_LUN_ZHUAN_POWER | power_xingyuelunzhuan_B_v01.png（batch80 B 双彗尾回旋，**定稿 2026-08-16**，已实装） |

（Power 31/31 全部定稿实装）

## 待办

- 凤威=v03_A、登基=v03_D 已定稿实装（2026-08-15），临时凤威已按新凤威重新派生；
- 淬火临时力量已定稿实装（2026-08-15）；
- 凤焰不息如需继续优化，同 finalize_icons.py 流程。
