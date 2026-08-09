# 美术阶段 Session 交接（2026-08-01，git HEAD 0f7612a + 大量未提交改动）

> 给下一个 session 的开工简报。先读：`CanAoNative/CLAUDE.md`（工程规则）、本文、
> `docs/card_art_pipeline.md` §5.6（评审铁律）、`docs/canao_visual_bible.md`（视觉最高依据）。

## 一、当前完成状态

- **叙事/本地化**：已定稿（同前）。
- **卡图全部完成**：92 张需制卡图全部定稿并实装（34 张早期 + 本轮 58 张新定稿/替换），
  `docs/card_art_index.csv` 为唯一追踪源；复核记录 `card_art_round1~5.md`。
- **角色三图**：海报 v03、写实概念图 v03、局内静态图 v04（透明，已实装进 creature_visuals）。
- **打击（援军版）整卡已删**：援军改产 3 张火刃+（浴火）；代码/本地化/Verify-R11 已同步。
- **回收站**：`art/rejected/` 共 170 张否决/未选/取代图 + 索引 + 提示词档案，只进不出。
- **git**：HEAD `0f7612a`；本轮全部改动未提交（代码 40+ 文件、卡图 100+、文档若干，
  等用户实机验收后提交）。
- **pck 待重新部署**：本交接编写时大量新图已裁入 godot 但未 import/部署（见下"立即待办"）。

## 二、生图通道与铁律（管线 §5.6 为准）

1. aizex generate.js（批量文生图）与 **aizex 图像编辑 MCP**（img2img，喂参考图）；
   Kimi 侧经 `aizex-image-bot/mcp_batch.js` 驱动（`node mcp_batch.js jobs.json [120 240]`，
   结果在 `batchNN_jobs_results.jsonl`）。防封铁律：单实例、120~360s 拟人间隔、勿动浏览器。
2. **双方案铁律**：每卡 ≥2 正交方案，拼版呈交用户二选一；难卡 4 方案。
3. **修改稿必审**：img2img 修改完成立即出审查图呈用户，批准后方可实装。
4. **信任通道**：MCP 出图即为成品，**Kimi 禁止逐张 ReadMediaFile 审查**（省 token）；
   只核文件/尺寸/任务对应，内容评审归用户（拼版路径呈报）。
5. img2img 参考图库：`../sts2_enemy_collection/images/`（81 张怪物官方立绘+manifest.csv）、
   `../sts2_character_collection/images/`（五原生角色，PNG 已转）。
   联动提示词结构：比例 + 参考图特征锁定句 + §4.1 锚点 + 内容 + §5.1 骨架 + §5.4 负面词。

## 三、卡图规则速查（视觉圣经 §五 + §6.4）

- 特写纪律：人物默认只出局部；全身须叙事理由；人怪同框两者都被镜头裁切；禁全身挥砍。
- 配色/构图去重：禁"剑左劈右怪/一剑斜贯"连发；禁一批内黑+蓝扎堆（色域锚点见 round5）。
- 锚点原样引用（残傲出镜时）；面部禁痣/额饰/面纹；月必须残；火焰以青蓝为主（凤火军械红刃、
  火刃红羽、浴火打击红底战场为用户特批先例）。
- 先古牌（焚诀/归隐陨山）走 3:4 竖版全画幅，实装按 250:351 裁切（1031×1448）。

## 四、实装管线（照做即可）

1. 定稿 PNG → `art/cards/raw/card_<id>_vNN.png`（CSV 同步）；
2. `python scripts/crop_card_art.py <raw.png> <id> [...]`（25:19 居中裁，写 godot+processed）；
3. 卡牌类覆写 `PortraitPath`/`PortraitPngPath`（冻结类改后重算 Verify-R11 哈希并注释）；
4. `Godot ..._console.exe --headless --path godot --import`；
5. `scripts/Deploy-Mod.ps1` → pck 二进制抽查新 PNG/import/ctex 齐全。

## 五、立即待办与下一步

1. **立即**：`Godot --import` → `Deploy-Mod.ps1` → pck 核验（本轮约 60 张新/换图未部署）；
2. 实机验收：全部卡图图鉴/战斗渲染、局内静态图、星月终式段数修复（ClearForPlayers 已改）、
   援军火刃产出；
3. 提交 git（用户验收后）；
4. 下一步候选：角色选择图（解锁/锁定 44:65）、选择背景 16:9、营火图、商店图；
   遗物 9 件三件套（85/85轮廓/256）、Power 31 个两件套（64/256）、能量球五层；
   工程待办：星月合击图鉴不可见排查（CLAUDE.md）。

## 六、不要重做 / 红线

- 不新增人物（家属面部规则未确立；灵魂云霜模式=半透明模糊无五官，已验证可行）；
- 不泪痣/面纹/额饰；不默认纯黑背景；不竖幅普通卡图（先古牌除外）；不锁死家族色；
- 机制数值一律以代码为准；怪物联动只作对手/彩蛋；删除内容必须三处同步（代码/本地化/Verify）。
