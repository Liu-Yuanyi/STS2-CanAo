# 修改前后差异摘要（2026-07-26 人物背景重写与卡面制作启动）

基线：HEAD `5dafd00`。本摘要为总览；逐条本地化差异见 `localization_changes.md`。

> **修订轮（2026-07-26 当日，用户复核后）**：年表源文件修正（湮门归来年龄
> 117/27、删 570 年条目）；双叙事文档按 12 条指示修订（九兄妹含尘星、寒羽为八妹、
> 望舒删除、世昌之死为辉雪下令残傲默许、依絮长居并无涯、删烧崖/不再教人/年级第一、
> 姐弟婚为接受度而非旧制）；视觉设定删除泪痣、服装改青鸾蓝+月白渐变+星金点缀、
> 盔甲改冷银白；三图重生成至 v03/v04。下文文件清单均为修订后状态。

## 修改的文件（3 + 1 工作区源文件）

| 文件 | 变化 | 说明 |
|---|---|---|
| `godot/.../zhs/characters.json` | 6 键 | 女帝→青鸾七天凤、她→他（详见变更报告） |
| `godot/.../eng/characters.json` | 6 键 | empress→Heaven Phoenix、her/she→he/him/his |
| `src/CanAoNative/Characters/CanAo.cs` | 1 行 | `CharacterGender.Feminine`→`Masculine`（编译与 Verify-R11 均通过） |
| `../新大事年表.txt` | 2 处 | 湮门归来年龄 117/27；删 570 年"考到年级第一"（用户指示） |

## 新增文档（9，均在 `docs/`）

- `canao_character_bible.md`——人物小传/心理结构/对白规范（任务 1）
- `canao_timeline.md`——年表对齐的人物时间线（任务 1）
- `localization_changes.md`——本地化变更报告（任务 2）
- `canao_visual_bible.md`——视觉圣经：视觉核心/主题/色彩/锚点/原生联动（任务 3）
- `canao_image_prompts.md`——三张示意图提示词与执行记录（任务 4）
- `canao_visual_review.md`——两批生成复核记录（任务 4）
- `card_art_pipeline.md`——99 卡清点/6 视觉家族/转译原则/文件规范（任务 5）
- `card_art_index.csv`——99 卡逐卡追踪（任务 5）
- `asset_inventory.md`——约 270 文件的完整素材清单（任务 6）

## 新增目录与资产

- `art/{source,concepts,cards/{raw,processed},icons/{raw,processed},rejected}/`、`docs/art_reviews/`
- 最终图 3 张：海报 `art/concepts/canao_poster_v03_1672x941.png`、
  概念图 `art/concepts/canao_concept_v03_1086x1448.png`、
  局内图 `art/source/canao_ingame_static_v04_1024.png`（透明）
- 旧版/不合格图留档：`art/rejected/`（v01/v02）与 `aizex-image-bot/output/`（朝向错误版）
- 局内预览 `docs/art_reviews/sprite_v04_preview_{512,128,64}.png`、`poster_v03_thumb_320.png`

## 工作区外的改动

- `aizex-image-bot/prompts.txt`：四批提示词（工具工作文件）
- `aizex-image-bot/output/`：8 张残傲图（3 最终+5 留档）+ results.jsonl 追加
- `backup/localization_20260726/`：本地化修改前全量备份（14 个 JSON）

## 回退方式

- 代码与本地化：`git checkout -- <file>` 或从 `backup/localization_20260726/` 恢复
- 新增文档/资产：直接删除即可，不影响现有功能
