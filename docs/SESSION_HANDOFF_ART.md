# 美术阶段 Session 交接（2026-07-29，git HEAD 91565c4）

> 给下一个 session 的开工简报。先读：`CanAoNative/CLAUDE.md`（工程规则）、本文。

## 一、项目与角色一句话

StS2 原生 Mod「残傲」：男性东方凤凰帝王，天凤帝国十二族青鸾七天凤。
叙事以 `docs/canao_character_bible.md`（心理结构/对白规范）与
`docs/canao_timeline.md`（对齐根目录 `新大事年表.txt`，止于 569 年）为最高依据；
视觉以 `docs/canao_visual_bible.md` 为最高依据。

## 二、当前完成状态

- **叙事**：双文档已定稿（经用户 12 条指示修订：九兄妹含尘星、寒羽为八妹、无"望舒"、
  湮门内无时间流逝、世昌之死为辉雪下令残傲默许、依絮长居并无涯、删泪痣/烧崖/年级第一）。
- **本地化**：zhs/eng `characters.json` 性别与简介已修正；`Characters/CanAo.cs` 性别=Masculine；
  报告 `docs/localization_changes.md`。
- **角色三图**（最终版，用户已认可）：
  - 海报 `art/concepts/canao_poster_v03_1672x941.png`
  - 写实概念图 `art/concepts/canao_concept_v03_1086x1448.png`
  - 局内静态图 `art/source/canao_ingame_static_v04_1024.png`（透明）
- **卡图 16 张已通过并实装进游戏**（pck 92 文件已部署到游戏目录，16 PNG+16 import+16 ctex 核验齐全）：
  打击v03、防御v03、凤羽残火v02、祭火v02、星月合击v02、诏令v02、尘土之战v02、青鸾勾法v03、
  凤焰不息v03、承天受命v02、浴火军旗v01、月斩v03、传令v01、天凤形态v01、涅槃v02、旧王复临v02。
  逐卡状态见 `docs/card_art_index.csv`；复核记录 `docs/card_art_round1.md`、`card_art_round2.md`。
- **回收站**：`art/rejected/`（32 张否决/取代图 + `prompts/archived_prompts.md` 全部原始提示词 +
  `rejected_index.csv` + README 恢复流程）。**只进不出，否决必登记**。
- **git**：工作区干净，HEAD `91565c4`。

## 三、可用的两条生图通道

1. **aizex-image-bot**（本地 Playwright 批量文生图）：`aizex-image-bot/`，
  用法与铁律见根目录 `mcp.md`（单实例、120~360s 拟人间隔、勿动浏览器窗口）。
  提示词写 `prompts.txt` 一行一条，`node generate.js prompts.txt` 后台跑，收 `output/NNN.png`。
2. **新增：图像编辑 MCP（img2img）**（用户 2026-07-29 配置）——可喂参考图做编辑，
  适合：以概念图 v03 为基准保持角色一致、按参考图精确画怪物、局部修改。
  文生图仍走 aizex-image-bot；需要参考图时用该 MCP。

## 四、卡图工作规则速查（都已固化在视觉圣经 §五）

- 残傲出镜：提示词必须**原样**包含 §4.1 中文锚点（现行版：无泪痣！青鸾蓝衣+冷银白甲）。
  **面部禁止任何痣/额饰/面纹**（泪痣已被用户枪毙）。
- 每条卡图提示词结构：**"画面比例4:3横图（宽大于高）"写最前** + 内容 + §5.1 风格骨架 +
  §5.4 负面词。竖幅是最大历史坑（批次 6 全军覆没过一次）。
- 画风 = StS2 原生赛璐璐硬边风（依据 `../sts2_card_portraits_collection/`，22+23 张样本）。
- 家族 A~F 只是松散题材指引（§5.3）；**每张卡独立选色与创新**，禁默认纯黑背景。
- 原生联动：雾菇、电球头（Globe Head，参考图 `../sts2_enemy_collection/`）已用两例。
- 已知构图坑：背视角度下冠易读成双角（拉开两叉角度）；剑气必须写明颜色（月斩曾变红）。

## 五、实装管线（已验证，照做即可）

1. 定稿 PNG 放 `art/cards/raw/card_<id>_vNN.png`（CSV 同步）；
2. 裁 25:19（高×25/19 居中裁）存 `godot/images/card_portraits/canao/<id>.png`；
3. 卡牌类覆写 `PortraitPath`/`PortraitPngPath` 指向该 res:// 路径
   （**注意 Verify-R11 冻结哈希**：改动冻结类后须用
   `Get-NormalizedTextSha256` 重算哈希更新 `scripts/Verify-R11.ps1` 并注释"有意修改"）；
4. 跑 Godot 导入：`E:\Godot_v4.5.1-stable_mono_win64\..._console.exe --headless --path godot --import`
   （占位 .import 已有 git 保护，删了可 `git checkout` 恢复）；
5. `scripts/Deploy-Mod.ps1`（自动 build+Verify+Pack-Pck+部署到游戏目录）；
   Pack-Pck ROOTS 已含 `godot/.godot/imported`（ctex 不入 git，重建随时可再导）。

## 六、下一步候选（按 asset_inventory.md 优先级）

1. 实机验收 16 张卡图（用户开一局看图鉴/战斗渲染）；
2. 第三批卡图（≤6 张/批，走 §5.6 节奏；候选：观星问月、暂避锋芒、燃羽突袭、桂轮、
   碎月一击、星月终式、布诏、王权等；先填 §5.3 模板再生成）；
3. 局内图实装：把 `can_ao` creature_visuals 场景从铁甲占位换成
   `art/source/canao_ingame_static_v04_1024.png`（静态 PNG 方案，注意 Bounds/定位节点）；
4. 角色选择图（解锁/锁定 44:65）、选择背景 16:9、营火图、商店图（可由海报/概念图衍生）；
5. 遗物 9 件三件套（85/85轮廓/256）、Power 31 个两件套（64/256）、能量球五层；
6. 工程待办（CLAUDE.md）：星月合击图鉴不可见的排查。

## 七、不要重做 / 用户偏好红线

- 不要新增人物（"望舒"事件）；新增设定必须显式标注并经用户确认。
- 不要泪痣/面纹/额饰；不要默认纯黑背景；不要竖幅卡图；不要"六大家族锁色"式死板；
  不要普通站立肖像当格挡/能力牌卡图。
- 打击=只有武器，防御=盾或格挡动作（原生极简哲学）。
- 机制数值一律以代码为准，不从本地化反推。
