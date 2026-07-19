# CanAoNative 开发经验与问题档案（Session 交接用）

> 写给下一个接手的 AI/开发者：这里汇总了"残傲"Mod 从 R1 到 R10+ 遇到的所有
> 问题类型、根因、教训与背景知识。先读本文档再动手，可以少踩 90% 的坑。

## 一、项目现状速览

- **项目**：杀戮尖塔 2 "残傲"原生角色 Mod（零 BaseLib），工作区
  `C:\Users\32880\RiderProjects\CanAoNative`
- **游戏版本**：v0.109.0（反编译在 `sts2/decompiled-v0.109.0/`，旧 v0.108 在
  `sts2/decompiled/`）
- **当前构建标记**：`CANAO_NATIVE_R10_CHARACTER_20260717`,manifest 0.0.10
- **已完成**：浴火/凤威/星月/消耗/诏令五大机制层；正式角色 + 专属三池；
  起始卡组与起始遗物；普通/罕见/稀有全部 50+ 张卡；8 遗物 3 药水；
  两张先古牌（焚诀/归隐陨山）；占位视觉资源
- **部署**：`powershell .\scripts\Deploy-Mod.ps1 -Configuration Release`
  （一条命令 = 校验 + 构建 + 打包 + 安装）
- **文档**：`docs/CARD_TEXT_CONVENTIONS.md`（文本/悬浮规范）、
  `docs/CARD_TUNING_GUIDE.md`（数值调整指南）、各阶段 `docs/R*.md`

## 二、原版"硬编码角色"陷阱（最大的一类问题）

STS2 大量代码用 if-else 链或字典硬编码五名原版角色，Mod 角色会踩中：

| 位置 | 现象 | 修法 |
| --- | --- | --- |
| `ModelDb.AllCharacters`（硬编码数组） | 角色不进选将界面 | Harmony Postfix 追加（`CanAoAllCharactersPatch`） |
| `ProgressSaveManager.CheckFifteenElitesDefeatedEpoch` | **精英战后奖励界面卡死**（抛 ArgumentOutOfRange） | Prefix 跳过（`ElitesEpochCharacterPatch`） |
| `SunkenTreasury` 事件 `goldMonologue` | 选"拿更多钱"卡死（LocException） | characters.json 补 `CAN_AO.goldMonologue`，格式 `[sine][color]…[/color][/sine]` |
| `AromaOfChaos` 事件 `aromaPrinciple` | 同类卡死风险 | characters.json 补 `CAN_AO.aromaPrinciple` |
| `SeaGlass` 遗物按角色取标题 | 获得时可能 LocException | relics.json 补 `SEA_GLASS.CAN_AO.title` |
| `LargeCapsule` 遗物找 Basic+Strike/Defend 标签 | 无标签卡 → First() 抛异常 | 基础卡补 `CardTag.Strike/Defend`（`HashSet<CardTag>`，protected） |
| `ArchaicTooth.TranscendenceUpgrades` | 古老牙齿无本角色替换映射 | Postfix 加映射（祭火→焚诀） |
| `TouchOfOrobas.GetUpgradedStarterRelic` | 初始遗物升级回退 Circlet | Postfix 加映射（帝国年表→帝国史册） |
| 图鉴过滤按钮（场景硬编码节点） | 无残傲分类 | Postfix 克隆按钮 + 注册谓词（`CanAoCardLibraryFilterPatch`） |

**教训**：凡涉及角色的原版功能，先 grep `Id.Entry`、`Character.Id.Entry`、
角色类名，找硬编码点，全部打补丁或补 key。

## 三、Hook 语义类问题（第二大类）

1. **`ModifyBlockAdditive`/`ModifyDamageAdditive` 是"增量"通道**（返回 0=不变，
   Dexterity 返回 +N）。翻倍/归零必须走 `ModifyBlockMultiplicative`/
   `ModifyDamageMultiplicative`（返回 1=不变，2=双倍，0=归零）。
   错误地 `return block` 会把全场格挡翻倍。
2. **回合结束管线**:`BeforeSideTurnEnd` → DoTurnEnd（虚无消耗）→
   `FlushPlayerHand`（弃手牌）→ `AfterSideTurnEnd`/`Late`。
   - "回合结束时手牌为空"只能在 `BeforeSideTurnEnd` 检测（之后手牌已被弃）;
   - 回合末快照（格挡、空手）只在**己方 side-turn 结束**赋值——每个
     side-turn 结束都赋值会被**敌方回合结束**清掉;
   - 敌方 side-turn 结束 = "被敌人打完之后"（青鸾羽衣的判定时机）。
3. **临时凤威清理**:PowerCmd 的 `silent` 只管闪光，不抑制 Hook。负临时凤威
   用"加回正数归零"会被当成"获得凤威"→ 用 `PowerCmd.Remove` 移除，
   不产生假增量。
4. **星月王冠式触发**：用锁存（首触即锁），不要在计数器上叠来源去重。
5. **升级改关键词**:`AddKeyword`/`RemoveKeyword`（实例级，持久，自动刷新
   牌面与悬浮）。
6. **异步铁律**(R2 教训）:Harmony Prefix 跳过返回 Task 的原方法必须给
   非空 `ref Task __result`;**禁止** `async void`/`.Wait()`/`.Result`/
   fire-and-forget，一切留在原 Task 链。

## 四、渲染/本地化类问题

1. **卡牌渲染必须有 `Pool`**（框体+能量图标），否则选牌界面抛
   "not in any card pool"——包括**选项令牌**。
2. **关键词只进 `CanonicalKeywords`**（自动金色独立行+悬浮），描述里不要
   手写"消耗。""虚无。"——否则双倍显示。动作义"消耗"要 `[gold]消耗[/gold]`。
3. **专属概念加黄**(`[gold]…[/gold]`）并配悬浮：Power→`FromPower<T>`,
   卡牌→`FromCard<T>`（升级版传 true/IsUpgraded)，通用→`StaticHoverTip.*`,
   浴火→补丁自动加（固有+临时，含图鉴）。
4. **升级预览走 `GetDescriptionForUpgradePreview`**——描述补丁要双管齐下。
5. **loc 按同名表合并**(cards/powers/relics/potions/characters/
   static_hover_tips.json)，不能新建表名；全部 UTF-8 无 BOM。
6. **`{IfUpgraded:show:A|B}` 冒号语法**;**能量**:`{Energy:energyIcons()}` 配
   `EnergyVar`（获取）与 `{energyPrefix:energyIcons(1)}`（费用）。
7. **先古牌**:`CardRarity.Ancient` 入池（奖励生成器排除 Basic/Ancient，
   不进普通奖励）；尘封魔典按角色池 Ancient 稀有度自动授予。

## 五、状态与生命周期问题

1. **战斗状态按 CombatState+Player 隔离**(ConditionalWeakTable)，禁止静态
   战斗状态；回合事实只经各自 Service(YuHuo/StarMoon/FengWei/Exhaust/Edict)。
2. **"每张牌自己记录"是错的**:"本回合消耗过/打出过/生成过"统一读
   ExhaustService/EdictService/StarMoonService。
3. **批量消耗要快照**（清宫、涅槃）：打出那一刻的集合，效果抽上来的不算。
4. **选项令牌不入堆不清理**（原生 Quasar 同款）;`RemoveFromCombat` 只能
   移除在堆里的牌。
5. **自我成长卡**（满目星辰）:`[SavedProperty]` + 写字段时同步
   `DynamicVars[x].BaseValue`（遗传算法模式）。

## 六、打包/部署问题

1. `Deploy-Mod.ps1` 打包范围必须含 `godot/CanAoNative` + `godot/scenes` +
   `godot/images` + `godot/materials`（漏了会导致 AssetLoadException →
   选将静默回退铁甲）。
2. **godotpcktool.exe 被 Windows Defender 误杀**——已用 `scripts/Pack-Pck.py`
   替代（PCK v3 格式：40 字节头+64 保留+目录表（plen/padded path/off/size/
   md5/flags)+16 对齐数据区，MD5=内容原始哈希）。
3. **Godot 官方导出**(`E:\Godot_v4.5.1-stable_mono_win64`):`export_presets.cfg`
   已配好可导出（Windows Desktop 平台 + --export-pack 绝对路径）。适合将来有
   真实美术源时使用；当前会丢无源 .import 并混入引擎缓存，暂不用。
4. 占位视觉：场景 .tscn 直接复制原版；PNG 用 `.png.import` 重映射到原版
   `.ctex`;`IconPath/CharacterSelectIconPath/MapMarkerPath/CharacterSelectSfx`
   可覆盖指回铁甲。
5. **角色必备资源**（缺了就崩）:creature_visuals、energy_counter、
   char_select_bg、char_select 图标、顶栏 icon、map marker、转场材质、
   商店/篝火场景、card_trail、多人手势、characters.json 全套 key
   (title/titleObject/description/4 代词/unlockText/goldMonologue/aromaPrinciple)。

## 七、待办/已知限制

- **星月合击图鉴不可见**（旧问题，CLAUDE.md 有待办与排查方向）;
- 全部美术为占位（卡图/立绘/Power 图标/角色素材）;
- 数值未平衡；多人未实测（需两台设备+两份游戏，联机要求双方同 Mod 版本）;
- 卡库过滤器按钮是硬编码场景节点，残傲分类由补丁克隆（图标沿用无色池）。

## 八、工作方式约定

- 每次只做一个可独立验收的小阶段；每阶段同步 BuildMarker、manifest、
  双语文本、Verify 脚本、文档；小步提交，通过后打 `canao-rN-stable`。
- 改已验证文件会触发 Verify 哈希失败：重算规范化 SHA-256 更新对应行
  (`perl -pe 's/\r\n/\n/g; s/\r/\n/g' 文件 | sha256sum`)。
- 涉及新 API/Hook 先查 `sts2/decompiled-v0.109.0/` 当前源码，不要猜签名。
- 测试看最新 godot.log：构建标记、模型注册、无
  `YUHUO_RESOLVE_FAILED`/`STARMOON_FAILED`/异常堆栈。
