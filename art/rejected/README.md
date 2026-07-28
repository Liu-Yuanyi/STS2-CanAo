# 回收站（art/rejected/）使用说明

这里存放**被否决或被取代**的图片，以及与它们一一对应的原始提示词。
目的：任何被丢弃的方案都可追溯、可翻回来复用，且后续批次不重复踩坑。

## 结构

- `*.png`——被否决/被取代的图片本体，文件名标注原因：
  - `*_wrongstyle`：画风与原生不符（批次 5，写实油画风）；
  - `*_rightstyle_wrongratio`：画风正确但画幅错误（批次 6，竖幅，可作先古牌竖图参考）；
  - `*_wrongfacing`：朝向错误；`*_moleissue`：泪痣错位；`*_superseded_by_*`：被新版本取代。
- `prompts/archived_prompts.md`——每张图对应的**原始提示词全文**，按批次组织。
- `rejected_index.csv`——逐图索引：文件名、类别、对应卡牌/素材、批次、否决原因、
  提示词位置、日期。

## 如何恢复某张图

1. 在 `rejected_index.csv` 找到目标图；
2. 直接复制回本目录对应的使用位置（卡图 → `art/cards/raw/`；角色图 → `art/concepts/` 或
   `art/source/`），按当时版本重新编号（如 `_v02r`）；
3. 若想以同一方向重新生成：从 `prompts/archived_prompts.md` 取该图的原始提示词，
   按需替换锚点/骨架为现行版（视觉圣经 §4.1、§5.1）后投入 `prompts.txt`；
4. 恢复后在 `docs/card_art_index.csv`（或对应索引）更新状态，并在本 CSV 标记"已恢复"。

## 规则

- 只进不出：被否决的图**只移入、不删除**（磁盘成本远低于重新生成）。
- 每次否决必须当日登记 CSV 并归档提示词，禁止"裸丢图"。
- 已通过的最终资产不放这里；finals 在 `art/cards/raw/`、`art/concepts/`、`art/source/`。
