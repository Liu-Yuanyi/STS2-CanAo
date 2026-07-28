# 本地化变更报告（残傲人物背景重写）

日期：2026-07-26
基线：HEAD `5dafd00`（修改前工作区干净，仅新增两个 docs 文件）。
回退方式：① git 回退；② 修改前全量备份位于 `../backup/localization_20260726/{zhs,eng}/`（14 个 JSON 原样复制）。
设定依据：`docs/canao_character_bible.md`、`docs/canao_timeline.md`、`prompt.md`、`新大事年表.txt`。

## 一、修改文件总览

| 文件 | 修改键数 | 性质 |
|---|---|---|
| `godot/CanAoNative/localization/zhs/characters.json` | 6 | 性别/人称修正 + 角色简介重写 |
| `godot/CanAoNative/localization/eng/characters.json` | 6 | 同上（中英同步） |
| `src/CanAoNative/Characters/CanAo.cs` | 1 行 | 代码层性别注册修正（非本地化文件，见第四节） |

其余 12 个本地化文件（cards/relics/potions/powers/ancients/static_hover_tips × zhs/eng）
经全量排查**不含性别错误与旧设定残留**，未做改动（排查结论见第五节）。

## 二、逐条变更

### zhs/characters.json

| 键 | 原文本 | 新文本 | 设定依据 |
|---|---|---|---|
| CAN_AO.description | 陨落的凤凰女帝。她以浴火为道，将每一次失去都化为归来的薪柴。 | 天凤帝国青鸾七天凤。他以浴火为道，将每一次失去都炼作归来的薪柴。 | 年表附 2：残傲为"天凤帝国十二族青鸾七天凤"；性别男（prompt）；人物圣经§三（悲痛只允许燃烧不允许出口=浴火心理学）。删除"陨落"——年表中他从未陨落，是退位归隐。 |
| CAN_AO.pronounObject | 她 | 他 | prompt：残傲为男性 |
| CAN_AO.possessiveAdjective | 她的 | 他的 | 同上 |
| CAN_AO.pronounPossessive | 她的 | 他的 | 同上 |
| CAN_AO.pronounSubject | 她 | 他 | 同上 |
| CAN_AO.eventDeathPrevention | 女帝拒绝倒下。 | 青鸾天凤拒绝倒下。 | 尊号取自年表附 2；人物圣经§5.3 低血量线"朕还站着"同源。 |

### eng/characters.json（与中文逐条对应）

| 键 | 原文本 | 新文本 | 设定依据 |
|---|---|---|---|
| CAN_AO.description | The fallen phoenix empress. Loss is her fuel; every sacrifice stokes the fire of her return. | The Azure Luan Heaven Phoenix of the Tianfeng Empire. Loss is his fuel; every sacrifice stokes the fire of his return. | 与中文同步；"青鸾"定译 Azure Luan，"天凤"定译 Heaven Phoenix（译名统一见第六节）。 |
| CAN_AO.pronounObject | her | him | 性别修正 |
| CAN_AO.possessiveAdjective | her | his | 同上 |
| CAN_AO.pronounPossessive | hers | his | 同上 |
| CAN_AO.pronounSubject | she | he | 同上 |
| CAN_AO.eventDeathPrevention | The empress refuses to fall. | The Heaven Phoenix refuses to fall. | 与中文同步。 |

## 三、审查后决定**不改动**的键（防止无理由重写）

| 键 | 现状 | 保留理由 |
|---|---|---|
| CAN_AO.title / titleObject | 残傲 / Can Ao | 名称无错误，prompt 未要求改名 |
| CAN_AO.cardsModifierTitle / cardsModifierDescription | 残傲卡牌… | 功能性文本，无叙事错误 |
| CAN_AO.unlockText | 含 `{Prerequisite}` | 功能性文本，占位符必须保留 |
| CAN_AO.bestiaryQuote | 灰烬之中，必有复燃。 | 无性别错误；与浴火机制、人物圣经§三完全吻合 |
| CAN_AO.bestiaryKillQuote | 帝国的债，用血来偿。 | 无性别错误；恰合"账簿人格"（人物圣经§5.2-2） |
| CAN_AO.goldMonologue / aromaPrinciple | 含 `[sine][orange]` 富文本 | 无性别错误；语气符合对白规范，富文本标记保留 |
| ancients.json 全部 12 键 | 先古对话（朕自称） | 无任何性别错误；帝王腔与对白规范一致。旧对白风格（短、冷、断言）与新圣经兼容，不重写 |
| relics.json 全部 .flavor | 九条风味文本 | 无性别、无旧设定；与年表锚点吻合（如帝国年表"编年自此始"） |
| cards/potions/powers/static_hover_tips 全部 | 机制描述 | 任务规定机制文本不改动 |

## 四、代码依据与代码侧修正

- `src/CanAoNative/Characters/CanAo.cs:15`：`Gender => CharacterGender.Feminine` →
  `CharacterGender.Masculine`。
  - 性质：与本地化同一处旧设定错误的代码层副本。该属性仅影响语法性别/人称呈现，
    不涉及任何战斗机制（已核对反编译枚举 `CharacterGender`，`Masculine` 为原生值）。
  - 此举不是"通过本地化反向修改机制"，而是同一错误在代码侧的同步修正。
- 其余代码（卡池、数值、效果）未做任何改动；机制文本与代码一致性本次未触发冲突。

## 五、修改后校验结果（全部通过）

1. **JSON 解析**：zhs/eng characters.json 均解析成功，各 15 键。
2. **键集合**：修改前后 zhs 键集合一致、eng 键集合一致、zhs↔eng 键集合一致（无丢键）。
3. **占位符**：`{Prerequisite}` 原样保留；全库扫描确认 cards/powers 中
   `{Damage:diff()}`、`{IfUpgraded:...}`、`[gold]`/`[sine][orange]` 等标记未被触碰
   （这些文件本次零改动）。
4. **残留扫描**：对全部 14 个本地化文件扫描 `女帝|女皇|她|empress|queen|\bher\b|\bshe\b|\bhers\b`，
   **0 命中**。
5. **构建**：`dotnet build` 0 错误（27 个警告均为修改前已存在的 CS8602）。
6. **项目验证**：`scripts/Verify-R11.ps1` 通过（R5–R9 哈希校验 + 本地化校验；
   `Characters\CanAo.cs` 仅作存在性检查，不在冻结哈希列表内，故本次修正不破坏基线保护）。

## 六、译名统一表（本次确立，后续必须沿用）

| 中文 | 英文 | 说明 |
|---|---|---|
| 残傲 | Can Ao | 沿用现有 |
| 青鸾 | Azure Luan | 冷色鸾鸟，区别于天凤 |
| 天凤 | Heaven Phoenix | 不用 Celestial Phoenix，保持两个词 |
| 青鸾七天凤 | the Azure Luan Heaven Phoenix | 全称时可用 Seventh Heaven Phoenix of the Azure Luan line |
| 天凤帝国 | the Tianfeng Empire | 音译+Empire |
| 朕 | We（royal we）/ I | 威压句用 We，其余按语境 |
| 辉雪 / 云霜 / 世昌 / 倩雪 / 望舒 | Huixue / Yunshuang / Shichang / Qianxue / Wangshu | 音译，首次出现可不加注释 |

## 七、尚未解决的问题

1. **角色副标题（subtitle）**：现游戏角色模板无 subtitle 键；若后续需要"青鸾天凤"作副标题，
   需先确认 `CharacterModel` 是否支持，再补键，本次未新增键。
2. **战斗开始/低血量/受创等事件对白**：当前本地化只有 `eventDeathPrevention` 一个战斗事件键，
   人物圣经§5.3 已给出全套样本对白，但**代码中尚无对应事件钩子**；待代码侧补充事件后
   再按圣经口径入本地化，本次未新增未挂接的键。
3. **eng 译名 "Can Ao" 与 "CanAo"**：类名/内部 ID 用 CanAo，显示名用 Can Ao，二者并存；
   属既有约定，未改动。
4. 修改后备份目录 `backup/localization_20260726/` 保留原文件，待本次工作验收后可清理。
