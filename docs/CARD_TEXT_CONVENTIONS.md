# 卡牌描述与悬浮提示规范

> 适用范围：CanAoNative 全部卡牌、Power、遗物、药水的文本与展示。
> 依据：STS2 v0.109.0 反编译源码与原生本地化（`sts2/loc_extracted/`）。
> 新增任何文本前必须先对照本文档；本文档与游戏机制冲突时以游戏源码为准。

## 1. 关键词（消耗、虚无、保留、固有、奇巧、不能被打出、永恒）

- 原生关键词**只写代码不写文本**：加入 `CanonicalKeywords` 后，游戏自动在
  描述前/后追加 `[gold]标题[/gold]。`，占**独立一行**，并自动附带悬浮提示。
- **禁止**在 `description` 里手写"消耗。""虚无。"等关键词句子——会出现两份。
- 关键词作为**动作**使用时是文本，加黄（见第 2 节）：
  原生例 `[gold]消耗[/gold]1张牌。`（燃烧契约）。

## 2. 金色高亮（`[gold]...[/gold]`）

以下词类在描述中出现时**必须**加黄（原生例见括号）：

- 牌堆与区域：手牌、抽牌堆、弃牌堆、消耗牌堆、牌组
  （刀刃之舞：`到你的[gold]手牌[/gold]`）
- 动作关键词：消耗/被消耗（`[gold]消耗[/gold]1张牌`）、丢弃、保留
- 资源与数值名词：格挡、力量（燃烧：`获得{n}点[gold]力量[/gold]`）
- 被引用的卡牌名：小刀（刀刃之舞：`添加{n}张[gold]小刀[/gold]`）
- 本 Mod 专属概念：**星、月、凤威、星月合击、浴火**
- Power 名称：与 `ExtraHoverTips` 里的悬浮提示一一对应

不加黄的：普通动词（造成、获得、抽、打出）、伤害数值、普通牌型名
（攻击牌/技能牌/能力牌，原生不黄）。

**禁止**使用设计稿写法 `【星月合击】`、`【诏令】`——描述里一律
`[gold]星月合击[/gold]`，没有其他括号形式。

## 3. 描述格式

- 数值一律走 `DynamicVar` + 占位符：`{Damage:diff()}`、`{Block:diff()}`、
  `{Cards:diff()}`，禁止把数值硬编码进文本。
- 一个完整句子一行，用 `\n` 分行（原生支持，见燃烧契约、坚毅）。
- 句式对齐原生：
  - 中文：`造成{Damage:diff()}点伤害。` / `获得{Block:diff()}点[gold]格挡[/gold]。` /
    `抽{Cards:diff()}张牌。`
  - 条件句：`若……，` 前缀（星月伐魔：`若本回合生成过[gold]星月合击[/gold]，`）
- 能量图标：`{energyPrefix:energyIcons(1)}`；不要手写"1费"。
- 选择界面提示（`selectionScreenPrompt`）是操作指引，**不加黄**，
  用 `{Amount}` 表示数量。

## 4. 悬浮提示（HoverTips）

悬浮提示是悬停整卡时右侧弹出的提示列表，**描述文本本身不可交互**。
数据来自 `CardModel.HoverTips` = `ExtraHoverTips` + 关键词自动生成。

- **提到 Power**（星/月/凤威/力量）→ override `ExtraHoverTips` 加
  `HoverTipFactory.FromPower<XxxPower>()`。文本与提示一一对应。
- **提到另一张牌**（星月合击/小刀）→ 加 `HoverTipFactory.FromCard<XxxCard>()`，
  悬浮显示该牌预览（原生：刀刃之舞 → 小刀）。
- **浴火**：不需要手写——`YuHuoHoverTipPatch` 会给所有当前拥有浴火的牌
  （固有 + 临时）自动追加浴火提示；只是在文本中**提到**浴火的牌
  （如清宫、凤骨再燃）加 `CanAoHoverTips.YuHuo`。
- 自定义提示文本放 `localization/{lang}/static_hover_tips.json`
  （合并进原生同名表），key 形式 `XXX.title` / `XXX.description`。
- Power 的悬浮文本来自 `powers.json` 的 `POWER.title/description`，
  新增 Power 必须双语配齐。

## 5. 浴火的"类关键词"展示

`CardKeyword` 是封闭枚举，浴火无法成为真关键词。等效实现（已建好，
禁止绕开）：

- **牌面**:`YuHuoDescriptionPatch` 自动在描述前插入
  `[gold]浴火。[/gold]` 独立一行（句号取原生 `card_keywords:PERIOD`）。
  固有浴火（`IIntrinsicYuHuo`）与临时浴火都覆盖，图鉴模板卡也覆盖。
- **禁止**在 `description` 里手写"浴火。"作为牌属性前缀；
  文本中**提及**浴火概念时用 `[gold]浴火[/gold]`。
- **悬浮**：见第 4 节。
- 关键词文本在 `cards.json` 的 `YU_HUO_KEYWORD`。

## 6. 衍生牌与图鉴

- 衍生牌（星月合击、未来的诏令/火刃）：`CardRarity.Token`，
  **不要**传 `shouldShowInCardLibrary: false`——原生小刀可见于图鉴
  Misc 区（Event/Token/Status/Curse/Quest  rarity 都归入 Misc）。
- 图鉴只收 `ModelDb.AllCards` 中 `ShouldShowInCardLibrary == true` 的牌；
  未发现的牌显示为剪影，属正常三态（Locked/NotSeen/Visible）。
- 衍生牌需要被引用方牌的 `ExtraHoverTips` 里加 `FromCard` 才有预览。

## 7. 本地化技术规则

- 每张卡/每个 Power 必须同时提供 eng + zhs 的 `.title` 和 `.description`；
  有选择界面的卡另加 `.selectionScreenPrompt`。
- Mod 本地化文件按**同名表合并**：`cards.json`、`powers.json`、
  `static_hover_tips.json` 会并入原生表；不能新建表名。
- 全部文件必须 UTF-8 无 BOM；PowerShell 读写一律显式 UTF-8。
- 模型 ID 由类名派生（`QingGongCard` → `QING_GONG_CARD`），改名即改 ID。

## 8. 新卡检查清单

1. 关键词只进 `CanonicalKeywords`，文本不重复写。
2. 文本提及的专属概念全部加黄，无 `【】`。
3. `ExtraHoverTips` 与文本提及一一对应（Power/Card/浴火）。
4. 数值走 `DynamicVar`；双语四个 key 配齐。
5. 浴火牌实现 `IIntrinsicYuHuo`，文本不写"浴火。"前缀。
6. 衍生牌 `Token` 稀有度且图鉴可见。
7. 跑 `scripts/Verify-R7.ps1`（或当前阶段 Verify）通过。
