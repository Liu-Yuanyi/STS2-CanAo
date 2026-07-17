# CanAoNative R6：本地 AI 接管与后续开发说明

> 项目：《杀戮尖塔 2》“残傲”角色 Mod  
> 当前阶段：R6（星月事件层）  
> 游戏基线：STS2 v0.108.0  
> 当前 Mod ID：`CanAoNative`  
> 当前构建标记：`CANAO_NATIVE_R6_STARMOON_EVENTS_20260717`  
> 技术路线：原生 Mod Loader + 原生模型/Commands/Hooks + 少量 Harmony  
> 明确约束：**零 Alchyr BaseLib 依赖**

---

## 0. 给接管者的结论

后续开发必须以：

```text
CanAoNative_R6_from_verified_R5_20260717.zip
```

为唯一源码基线。

该 R6 是从用户已完成实机验证的 R5 工作区继续开发而来。此前的 R2、R3、R4、重建版 R5、旧 BaseLib 工程、旧补丁包和旧日志只能作为历史资料，不能作为新代码来源。

完整 R6 原始交付包 SHA-256：

```text
E63C7682704D9A6FC24307B167AE9719C7FBFBBA74F0BB9524CBE4AC8D711E47
```

接管后第一件事不是增加卡牌，而是：

1. 解压 R6；
2. 确认目录层级；
3. 初始化或检查 Git；
4. 执行部署脚本；
5.完成 R6 回归测试；
6. 打标签保存稳定基线；
7. 再开始 R7。

---

# 一、项目目标与当前边界

“残傲”最终目标是一个拥有以下内容的完整自定义角色：

- 独立角色模型；
- 独立卡池、遗物池与药水池；
- 起始卡组与起始遗物；
- 核心资源：星、月、凤威；
- 核心机制：浴火；
- 衍生牌：星月合击、诏令；
- 完整本地化、图片和角色 UI；
- 单人存档和多人状态隔离。

**当前 R6 仍是原生机制验证工程，不是完整角色。**

现阶段所有测试卡仍注册到：

```csharp
ColorlessCardPool
```

还没有：

- `CharacterModel`；
- 专属 `CardPoolModel`；
- 专属 `RelicPoolModel`；
- 专属 `PotionPoolModel`；
- 起始遗物；
- 正式角色选择界面；
- 完整卡表；
- 正式专属图片。

不要在 R7 直接开始角色模型。应先完成消耗事件、诏令和核心战斗状态，再进入正式角色阶段。

---

# 二、历史问题与已经确定的技术决策

## 2.1 为什么完全舍弃 BaseLib

旧工程的启动故障涉及 BaseLib 自动内容发现、手动 `ModelDb.Inject`、重复模型注册和版本兼容问题。项目已经决定不再把任何第三方内容框架作为地基。

当前工程只依赖游戏和运行环境自带程序集，例如：

```text
sts2.dll
0Harmony.dll
GodotSharp.dll
```

禁止重新加入：

```text
Alchyr.Sts2.BaseLib
Alchyr.Sts2.ModAnalyzers
```

## 2.2 Harmony 的定位

Harmony 只是运行时方法补丁工具，不是内容注册框架。

当前 Harmony 主要用于：

- 拦截 `CardCmd.Exhaust`，实现浴火；
- 为临时浴火追加描述文本；
- 兼容自定义 Power 图标。

新增功能时优先级必须是：

```text
原生模型虚方法
→ 原生 Commands
→ 原生语义 Hooks
→ 精确 Harmony Postfix/Prefix
→ 最后才考虑 Transpiler
```

不要为了方便把每个机制都写成 Harmony Patch。

## 2.3 浴火不是 `CardKeyword` 枚举成员

Harmony 无法真正向已加载的 CLR `enum` 添加成员。

“浴火”在代码中是独立 Trait 与事件系统：

```text
IIntrinsicYuHuo
YuHuoService
YuHuoCombatState
YuHuoResolver
YuHuoExecutionContext
YuHuoResolutionContext
浴火事件接口
```

表现层会在卡牌描述上追加“浴火”，但机制不能依赖描述文本。

---

# 三、R2 至 R6 的稳定演进

## R2：修复浴火异步契约

旧实现拦截一个返回 `Task` 的 `CardCmd.Exhaust` 后：

```csharp
return false;
```

却没有设置 Harmony 的：

```csharp
ref Task __result
```

这会让燃烧契约执行：

```csharp
await null;
```

导致浴火牌能自动打出，但燃烧契约无法继续抽牌。

稳定规则已经确立：

```text
Prefix 跳过返回 Task 的原方法
=> 必须提供非 null 的 ref Task __result
```

禁止恢复以下实现：

```csharp
TaskHelper.RunSafely(...)
async void
.Wait()
.Result
```

## R3：浴火事件与临时浴火

建立了：

- 浴火整次结算上下文；
- 单次触发上下文；
- 浴火前后事件；
- 按战斗、玩家、具体卡牌实例保存的临时浴火；
- 牺牲准备；
- 凤焰不息的触发次数扩展接口。

## R4：浴火生产卡

加入并验证：

- 羽列千军；
- 浴火军旗；
- 对“因浴火触发”的卡牌效果判断；
- 浴火后事件监听。

## R5：凤威分层

建立：

```text
FengWeiPower             永久凤威
TemporaryFengWeiPower    本回合凤威
FengWeiService           唯一读写入口
```

有效凤威定义：

```text
有效凤威 = 永久凤威 + 本回合凤威
```

加入：

- 示威；
- 暂避锋芒。

## R6：星月事件层

建立：

```text
StarMoonService
StarMoonCombatState
StarMoonGenerationContext
StarMoonPlayedContext
IBeforeStarMoonGenerated
IAfterStarMoonGenerated
IAfterStarMoonPlayed
```

加入：

- 盘旋；
- 星月伐魔；
- 天凤军阵。

同时将临时浴火等回合状态移动到完整回合结束后清理，避免虚无消耗前状态提前消失。

---

# 四、当前工作区结构

```text
CanAoNative/
├── CLAUDE.md
├── README.md
├── docs/
├── godot/
│   └── CanAoNative/
│       ├── images/
│       └── localization/
│           ├── eng/
│           └── zhs/
├── packaging/
│   └── CanAoNative.json
├── scripts/
│   ├── Build-Mod.ps1
│   ├── Deploy-Mod.ps1
│   ├── Find-FreshLog.ps1
│   ├── Verify-Deployment.ps1
│   ├── Verify-NoBaseLib.ps1
│   ├── Verify-R3.ps1
│   ├── Verify-R4.ps1
│   ├── Verify-R5.ps1
│   └── Verify-R6.ps1
└── src/
    └── CanAoNative/
        ├── Cards/
        ├── Patches/
        ├── Powers/
        ├── Rules/
        │   ├── FengWei/
        │   ├── StarMoon/
        │   └── YuHuo/
        ├── CanAoNative.csproj
        └── ModEntry.cs
```

## 4.1 权威入口

```text
src/CanAoNative/ModEntry.cs
```

职责：

- 输出 Build Marker、MVID 和 DLL 路径；
- 注册 `CanAoCombatRules`；
- 将当前测试卡加入无色卡池；
- 应用 Harmony；
- 输出模型 ID。

任何新阶段都应同步更新：

```csharp
BuildMarker
```

以及：

```json
packaging/CanAoNative.json
```

中的版本号和描述。

## 4.2 中央战斗规则

```text
src/CanAoNative/Rules/CanAoCombatRules.cs
```

当前职责：

- 监听星/月 Power 增长并触发星月组合；
- 记录星月合击打出；
- 在 `AfterSideTurnEndLate` 清除：
  - 到期临时浴火；
  - 星月回合计数。

后续跨卡牌、跨 Power 的战斗事实优先集中到战斗规则对象或相应 Service，不能散落成静态变量。

---

# 五、浴火系统：不可破坏的核心

## 5.1 文件地图

```text
Patches/YuHuoExhaustPatch.cs
Patches/YuHuoDescriptionPatch.cs

Rules/YuHuo/IIntrinsicYuHuo.cs
Rules/YuHuo/IYuHuoEvents.cs
Rules/YuHuo/IYuHuoTriggerCountModifier.cs
Rules/YuHuo/YuHuoCombatState.cs
Rules/YuHuo/YuHuoExecutionContext.cs
Rules/YuHuo/YuHuoResolutionContext.cs
Rules/YuHuo/YuHuoListenerRegistry.cs
Rules/YuHuo/YuHuoResolver.cs
Rules/YuHuo/YuHuoService.cs
```

## 5.2 正确调用链

```text
CardCmd.Exhaust(card)
│
├─ 无浴火
│   └─ 执行游戏原始 Exhaust
│
├─ 当前卡牌已处于浴火解析
│   └─ 执行游戏原始 Exhaust
│
└─ 有浴火且未重入
    └─ Harmony 将 YuHuoResolver 的 Task 放入 __result
        ├─ 创建整次结算上下文
        ├─ 计算触发次数
        ├─ 触发 BeforeResolved
        ├─ 循环每次浴火触发
        │   ├─ 设置单次上下文
        │   ├─ BeforeTrigger
        │   ├─ await CardCmd.AutoPlay
        │   ├─ AfterTrigger
        │   └─ finally 清理单次上下文
        ├─ 确保最终只正常消耗一次
        ├─ AfterResolved
        └─ finally 释放重入锁
```

所有步骤都必须留在原调用者的 `Task` 链中。

## 5.3 浴火状态语义

固有浴火：

```csharp
class SomeCard : CardModel, IIntrinsicYuHuo
```

临时浴火：

```text
YuHuoCombatState
按具体 CardModel 实例保存
按 Player 隔离
按 CombatState 隔离
```

查询浴火只能通过：

```csharp
YuHuoService.HasYuHuo(...)
YuHuoService.IsTriggeredByYuHuo(...)
YuHuoService.GetCurrentContext(...)
```

不能用：

```csharp
cardPlay.IsAutoPlay
```

代替浴火来源判断，因为其他机制也可能自动打出牌。

## 5.4 浴火必须回归测试的场景

每次改动浴火、消耗、牌堆移动或回合结束 Hook 后，至少测试：

1. 直接打出“浴火 + 消耗”卡牌；
2. 燃烧契约消耗浴火牌；
3. 燃烧契约必须继续抽牌；
4. 凤焰不息额外触发；
5. 牺牲准备只影响被选中的具体实例；
6. 临时浴火回合结束后消失；
7. 临时浴火 + 虚无在回合结束时仍先触发浴火；
8. 日志没有无限 `YUHUO_RESOLVE` 或失败循环。

---

# 六、星、月与星月合击

## 6.1 资源模型

```text
StarPower
MoonPower
```

星和月是临时战斗资源。

数量变化必须通过游戏原生 Power Commands，例如：

```csharp
PowerCmd.Apply<StarPower>(...)
PowerCmd.Apply<MoonPower>(...)
PowerCmd.ModifyAmount(...)
```

禁止直接：

```csharp
power.SetAmount(...)
```

原因是直接设置会绕过游戏的 Power 变化 Hooks、历史和其他 Mod 的监听。

## 6.2 组合规则

`CanAoCombatRules.AfterPowerAmountChanged` 在星或月增加时调用：

```text
StarMoonHelper.CheckAndResolve
```

组合时计算：

```text
pairCount = min(Star, Moon)
```

然后扣除相同数量的星和月，并生成同等数量的星月合击。

组合过程必须有按玩家重入保护，避免扣除星/月时重新进入组合逻辑。

## 6.3 唯一生成入口

所有星月合击必须通过：

```csharp
StarMoonService.Generate(...)
```

禁止未来代码直接：

```csharp
combatState.CreateCard<StarMoonStrike>()
CardPileCmd.AddGeneratedCardToCombat(...)
```

否则会绕过：

- `GeneratedThisTurn`；
- 盘旋；
- 天凤军阵；
- 未来遗物和 Power；
- 生成前/生成后事件。

权威顺序：

```text
创建具体 StarMoonStrike 实例
→ BeforeStarMoonGenerated
→ 加入战斗
→ RecordGenerated
→ AfterStarMoonGenerated
```

打出记录：

```text
游戏完成 StarMoonStrike CardPlay
→ CanAoCombatRules.AfterCardPlayedLate
→ RecordPlayed
→ AfterStarMoonPlayed
```

## 6.4 当前回合历史

`StarMoonCombatState` 按战斗和玩家保存：

```text
GeneratedThisTurn
PlayedThisTurn
```

回合清理由：

```text
CanAoCombatRules.AfterSideTurnEndLate
```

执行。

不要为“本回合是否生成过星月合击”在各张卡中新增静态 bool。

---

# 七、凤威系统

## 7.1 结构

```text
Powers/FengWeiPower.cs
Powers/TemporaryFengWeiPower.cs
Rules/FengWei/FengWeiService.cs
```

含义：

```text
FengWeiPower
永久凤威，可为负数

TemporaryFengWeiPower
仅本回合生效的凤威修正，可正可负

FengWeiService.GetEffectiveAmount
永久 + 临时
```

## 7.2 唯一读写入口

优先使用：

```csharp
FengWeiService.GetPermanentAmount(player)
FengWeiService.GetTemporaryAmount(player)
FengWeiService.GetEffectiveAmount(player)

FengWeiService.GainPermanent(...)
FengWeiService.ModifyTemporary(...)
```

不要让卡牌直接随意查询或应用两个 Power。

## 7.3 后续尚未实现的凤威语义

设计稿中仍有：

- 本回合是否获得过凤威；
- 本回合第一次获得凤威；
- 复辟：忽略本回合临时凤威；
- 将永久凤威调整到指定数值；
- 因凤威变化生成星月合击。

这些应加入新的按玩家战斗/回合状态和明确事件，不应通过读取描述或扫描日志实现。

---

# 八、当前已实现内容

## 8.1 调试/探针内容

这些内容主要用于验证原生注册和 Power，不一定属于最终卡池：

```text
CanAoProbeCard
CanAoProbePowerCard
CanAoProbePower
GainStarCard
GainMoonCard
GainFengWeiCard
```

进入正式角色阶段前应：

- 从玩家可见卡池移除，或
- 仅在 Debug 构建注册，或
- 迁移到开发者测试命令。

## 8.2 当前机制与卡牌

### 浴火

```text
浴火斩
牺牲准备
凤焰不息
羽列千军
浴火军旗
```

### 凤威

```text
示威
暂避锋芒
```

### 星月

```text
星月合击
盘旋
星月伐魔
天凤军阵
```

所有这些当前都在 `ColorlessCardPool` 中用于测试。

---

# 九、资源、本地化与 UI 现状

## 9.1 本地化

目前提供：

```text
godot/CanAoNative/localization/eng/cards.json
godot/CanAoNative/localization/eng/powers.json
godot/CanAoNative/localization/zhs/cards.json
godot/CanAoNative/localization/zhs/powers.json
```

PowerShell 读取 JSON 时必须显式使用严格 UTF-8，避免中文被系统默认编码破坏。

新增卡牌或 Power 时必须同时添加：

```text
英文 title
英文 description
简体中文 title
简体中文 description
```

有选择界面的卡牌还要提供相应 prompt key。

## 9.2 图片

当前只有有限的探针图片。很多卡牌仍复用通用路径或占位资源。

`CanAoPowerIconPatch` 当前用于规避自定义 Power 图标缺失，部分 Power 暂时复用已确认存在的力量图标。

这只是过渡方案。正式阶段需要：

- 自有 Power 图标；
- 自有卡图；
- 明确 PCK 导入路径；
- 删除不必要的图标兼容 Patch。

## 9.3 当前已知 Godot 日志噪声

历史日志中出现过：

```text
Invalid Task ID
```

它没有进入 CanAo 浴火、凤威、星月或初始化调用栈，也没有中断战斗。目前没有证据证明由本 Mod 导致。

处理原则：

- 不要为了消除该日志随意修改浴火异步链；
- 若要调查，先用仅启用 CanAoNative 的干净环境复现；
- 提供错误前后至少 100 行日志；
- 找到明确 CanAo 调用栈后才修改代码。

---

# 十、绝对禁止事项

本节是交接中最重要的约束。

## 10.1 禁止重新引入 BaseLib

禁止：

```text
Alchyr.Sts2.BaseLib
Alchyr.Sts2.ModAnalyzers
CustomCardModel
CustomPowerModel
PlaceholderCharacterModel
```

## 10.2 禁止手动注入模型

禁止：

```csharp
ModelDb.Inject(...)
InjectModels()
new SomeCanonicalModel()
```

模型注册使用游戏原生发现和：

```csharp
ModHelper.AddModelToPool<...>()
```

## 10.3 禁止破坏异步命令链

禁止：

```csharp
TaskHelper.RunSafely(...)
async void
task.Wait()
task.Result
_ = SomeAsyncMethod()
```

用于卡牌行动、选牌、Power、抽牌、移动牌堆或浴火结算。

## 10.4 禁止用全局静态状态保存战斗事实

禁止：

```csharp
static bool HasGeneratedStarMoon;
static int ExtraTriggers;
static HashSet<CardModel> TemporaryYuHuo;
```

所有状态必须按：

```text
CombatState
Player
具体卡牌实例
```

隔离。

## 10.5 禁止分散绕过 Service

禁止绕过：

```text
YuHuoService
StarMoonService
FengWeiService
```

直接复制同类逻辑。

## 10.6 禁止一次开发多个大系统

每个阶段只完成一个可以独立测试和回滚的主题，例如：

```text
R7：消耗事件
R8：诏令
R9：遗物/药水与存档
R10：角色与专属池
```

不要在同一提交中同时重构浴火、星月、凤威和角色 UI。

---

# 十一、接管后的本地安装与稳定基线

## 11.1 备份旧工作区

```powershell
$Old = "C:\Users\32880\RiderProjects\CanAoNative"
$Stamp = Get-Date -Format "yyyyMMdd-HHmmss"
$Backup = "C:\Users\32880\RiderProjects\CanAoNative_before_handoff_$Stamp"

if (Test-Path $Old) {
    Rename-Item $Old $Backup
    Write-Host ("Backup: {0}" -f $Backup)
}
```

## 11.2 解压

最终结构必须是：

```text
C:\Users\32880\RiderProjects\CanAoNative\README.md
C:\Users\32880\RiderProjects\CanAoNative\CLAUDE.md
C:\Users\32880\RiderProjects\CanAoNative\src\CanAoNative\CanAoNative.csproj
```

不能出现双层：

```text
C:\Users\32880\RiderProjects\CanAoNative\CanAoNative\src\...
```

## 11.3 建立 Git 基线

若压缩包不含 `.git`：

```powershell
cd C:\Users\32880\RiderProjects\CanAoNative

git init
git add -A
git commit -m "baseline: verified R6 handoff"
git tag canao-r6-handoff
```

之后每个小阶段单独提交。

## 11.4 部署

```powershell
cd C:\Users\32880\RiderProjects\CanAoNative

$env:STS2_GAME_DIR = `
    "E:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2"

Set-ExecutionPolicy -Scope Process Bypass -Force

.\scripts\Deploy-Mod.ps1 -Configuration Release
```

正确输出应包含：

```text
Verified R5 gameplay-core hashes passed.
R6 Star-Moon event, UTF-8, manifest and localization verification passed.
Deployment verified.
```

运行日志必须包含：

```text
CANAO_NATIVE_R6_STARMOON_EVENTS_20260717
```

---

# 十二、R6 交接验收测试

在开始 R7 前，本地 AI 应要求用户完成以下测试，并读取完整日志。

## 12.1 浴火回归

- 燃烧契约消耗浴火斩；
- 浴火斩自动打出；
- 燃烧契约继续抽 2 张牌；
- 无 `NullReferenceException`；
- 无 `YUHUO_RESOLVE_FAILED`；
- 无无限触发。

## 12.2 临时浴火

- 牺牲准备选择两张具体卡；
- 同名两张只选择一张时，另一张不获得浴火；
- 下一回合临时浴火消失；
- 虚无牌在回合结束消耗时仍触发浴火。

## 12.3 凤威

- 永久凤威保留；
- 示威增加本回合凤威；
- 暂避锋芒降低本回合凤威；
- 回合末临时凤威清除；
- 星月合击读取有效凤威。

## 12.4 星月事件

- 盘旋立即获得基础格挡；
- 每生成星月合击，盘旋重复获得格挡；
- 下一回合盘旋失效；
- 星月伐魔只在本回合已生成时获得星和月；
- 天凤军阵每生成一张星月合击造成一次 AOE；
- 一次生成多张时按张数触发。

日志不得包含：

```text
NullReferenceException
STARMOON_FAILED
YUHUO_RESOLVE_FAILED
YUHUO_FALLBACK_EXHAUST_FAILED
Exception thrown when calling mod initializer
```

---

# 十三、推荐的 R7 设计：统一消耗事件层

R7 不应直接先写五张卡。应先建立统一的消耗事件和按玩家回合状态。

## 13.1 目标能力

至少支持：

```text
本回合消耗总数
本回合第一次消耗
最近消耗的卡牌
被消耗牌的类型
被消耗牌是否拥有浴火
消耗来源：
  正常打出后消耗
  其他卡牌消耗
  虚无
  浴火自动打出后的消耗
```

## 13.2 推荐结构

```text
Rules/Exhaust/
├── ExhaustCombatState.cs
├── ExhaustEventContext.cs
├── ExhaustService.cs
├── ExhaustListenerRegistry.cs
└── IExhaustEvents.cs
```

事件可以包含：

```csharp
IBeforeCanAoCardExhausted
IAfterCanAoCardExhausted
IFirstExhaustThisTurn
```

名称应避免与游戏可能已有的 Hook 混淆。

上下文至少包含：

```text
Card
Owner
OriginalPile
CardType
HadYuHuo
CausedByEthereal
TriggeredByYuHuo
SequenceNumberThisTurn
```

## 13.3 如何监听消耗

首先检查 v0.108.0 反编译源码是否已有可靠的：

```text
AfterCardExhausted
AfterCardMoved
AfterCardAddedToPile
```

语义 Hook。

若没有，优先扩展现有 `YuHuoExhaustPatch` 附近的最窄公共消耗边界，但不能让消耗事件依赖“只有浴火牌才会经过”的路径。

必须保证普通牌、虚无牌、浴火牌和其他卡牌效果造成的消耗都能统一记录。

## 13.4 R7 第一批卡牌

事件层稳定后，按难度实施：

### 征召

```text
浴火。抽 3/4 张牌。
```

用途：继续验证浴火自动打出和抽牌异步链。

### 浴火打击

```text
浴火。造成 18/24 点伤害。
若本牌因浴火触发，获得 1/2 月。
```

用途：验证 `YuHuoService.IsTriggeredByYuHuo`。

### 焚膏继晷

```text
消耗不超过 1/2 张手牌。
若至少消耗 1 张浴火牌，获得 1 星与 1 月。
```

用途：验证多选、消耗批次和浴火属性快照。

### 清宫

```text
消耗手牌中所有非浴火技能牌。
每消耗 1 张，获得 5/8 格挡。
消耗。
```

用途：验证筛选、批量消耗和按张结算。

### 凤骨再燃

```text
从消耗牌堆选择 1 张浴火牌加入手牌，
它本回合费用 -1/-2。消耗。
```

用途：验证从消耗堆选牌、浴火查询和临时费用修改。

R7 不要同时加入诏令。

---

# 十四、R8 及后续路线图

## R8：诏令系统

建立衍生牌：

```text
诏令
0费
保留
选择并消耗一张手牌
按卡牌类型获得星/月
自身消耗
```

同时建立：

```text
EdictService
EdictCombatState
GeneratedEdictsThisTurn
PlayedEdictsThisTurn
```

首批卡：

- 传令；
- 密诏；
- 王权；
- 帝国余威；
- 承天受命；
- 天凤形态。

## R9：遗物、药水、存档和多人

重点：

- 涅槃火种；
- 星月王冠；
- 浴火和星月相关遗物；
- 凤威酒；
- 御令瓶；
- 中途存档；
- 两名玩家状态隔离；
- Rewind/重放兼容。

## R10：正式角色与专属卡池

在至少 20–30 张卡稳定后再做：

- `CharacterModel`；
- 专属卡池；
- 起始卡组；
- 起始遗物；
- 角色选择；
- 专属图片和颜色；
- 独立存档测试。

## R11+：完整卡表和平衡

- 完成剩余卡牌；
- 删除测试探针；
- 调整数值；
- 专属遗物和药水；
- 多人专属卡；
- Workshop 发布材料。

---

# 十五、本地 AI 每次开发必须遵循的工作流

## 15.1 开始前

1. 阅读本文件、`CLAUDE.md` 和当前阶段文档；
2. 检查 Git 状态；
3. 运行现有 Verify 脚本；
4. 查阅当前游戏版本反编译签名；
5. 明确本阶段唯一目标；
6. 写出验收清单。

## 15.2 修改时

每新增模型必须同时处理：

```text
C# 类
ModEntry 注册
中英文 localization
必要图片/PCK
Verify 脚本
README/CLAUDE/阶段文档
BuildMarker
manifest version
```

每次新增事件系统必须明确：

```text
状态归属
玩家隔离
战斗隔离
触发时机
异步顺序
重入保护
回合/战斗清理
存档策略
多人确定性
```

## 15.3 修改后

执行：

```powershell
.\scripts\Deploy-Mod.ps1 -Configuration Release
```

然后：

1. 记录编译器第一个错误；
2. 不要同时修十个推测问题；
3. 运行最小测试；
4. 找最新日志；
5. 检查 Build Marker；
6. 检查失败关键字；
7. 完成实机数值测试；
8. 提交 Git；
9. 打阶段标签；
10. 导出完整工作区 ZIP 和差异补丁。

---

# 十六、建议的 Git 习惯

稳定基线：

```powershell
git tag canao-r6-handoff
```

R7 推荐提交：

```text
refactor: add combat-scoped exhaust event state
feat: implement recruitment yuhuo draw card
feat: implement yuhuo strike conditional moon gain
feat: implement multi-card exhaust selection
feat: implement palace purge
feat: implement phoenix bone rekindle
docs: add r7 verification and handoff notes
```

不要使用一个提交完成整个 R7。

每个阶段通过实机测试后：

```powershell
git tag canao-r7-stable
```

---

# 十七、交接给本地 AI 时应提供的文件

至少提供：

```text
CanAoNative_R6_from_verified_R5_20260717.zip
CanAoNative_R6_本地AI交接说明.md
残傲.md
当前 v0.108.0 反编译源码或 ILSpy 工程
最新一次通过测试的 Godot 日志
```

不要同时把多个旧工作区放在本地 AI 的当前编辑目录。旧包应放到：

```text
archive/
```

并明确写上：

```text
历史参考，禁止作为源码基线
```

---

# 十八、可直接粘贴给本地 AI 的首轮指令

```text
你现在接管 CanAoNative 项目。

唯一源码基线是 CanAoNative R6，构建标记：
CANAO_NATIVE_R6_STARMOON_EVENTS_20260717

先完整阅读：
1. CanAoNative_R6_本地AI交接说明.md
2. CLAUDE.md
3. README.md
4. docs/INSTALL_R6.md
5. docs/R6_STARMOON_EVENTS.md
6. docs/R6_STATIC_VERIFICATION.md
7. 残傲.md

硬性规则：
- 不得引入 BaseLib 或 Alchyr analyzers。
- 不得使用 ModelDb.Inject。
- 不得直接 new canonical 模型。
- 不得使用 fire-and-forget、async void、Wait 或 Result。
- 不得破坏 YuHuoExhaustPatch 的 ref Task __result 契约。
- 浴火、星月、凤威必须分别通过 YuHuoService、
  StarMoonService、FengWeiService。
- 战斗状态必须按 CombatState 和 Player 隔离。
- 所有新 API/Hook 必须先核对 v0.108.0 反编译源码。
- 每次只实现一个可独立验收的小阶段。
- 修改后必须更新 BuildMarker、manifest、双语本地化、
  Verify 脚本和阶段文档。

第一项工作不是直接添加卡牌，而是：
1. 编译并部署未修改的 R6；
2. 验证构建标记和 R6 回归测试；
3. 建立 Git 标签 canao-r6-handoff；
4. 设计 R7 的统一消耗事件层；
5. 在获得确认前不要开始诏令或角色模型。

输出应包括：
- 对现有架构的复核结果；
- R7 的文件级设计；
- 需要核对的游戏 API 签名；
- 风险与回归测试；
- 小提交计划。
```

---

# 十九、用户侧交接清单

切换到本地 AI 后，请你本人保留以下习惯：

- 每次测试都确认日志中的 Build Marker；
- 每次发日志前把它复制并改名为阶段名，例如：
  `CanAoNative-R7-test-01.log`；
- 不要只说“测试没问题”，同时记录：
  - 测试了哪张牌；
  - 手牌和 Power 初始状态；
  - 预期数值；
  - 实际数值；
  - 是否保存/读档；
- 每个稳定阶段保留完整 ZIP；
- 每个稳定阶段记录 SHA-256；
- 不要覆盖上一个稳定工作区；
- 首次出现异常时保留完整日志，不要连续重复启动覆盖证据。

---

# 二十、当前事实与未验证事项

## 已由历史实机测试支持

- 移除 BaseLib 后可正常进入游戏；
- 浴火不会再让燃烧契约卡在抽牌前；
- 临时浴火、凤焰不息、羽列千军、浴火军旗主流程可运行；
- 凤威正负临时修改主流程可运行；
- 星月合击生成和相关测试卡主流程可运行；
- R5 日志没有浴火或初始化异常。

## R6 交付时完成的是静态核验

R6 生成环境没有连接用户的：

- .NET SDK；
- 游戏运行时；
- Godot；
- 实际 STS2 安装。

因此 R6 的：

- 编译；
- PCK 加载；
- 盘旋；
- 星月伐魔；
- 天凤军阵；
- 晚期回合清理；

仍应以用户本机的 R6 编译与实机日志作为最终依据。

如果这些已经由用户完成测试，应由本地 AI在接管后的第一份项目记录中补写：

```text
R6 实机验证日期
测试日志路径
测试场景
结果
已知偏差
```

---

## 最终交接原则

这套工程最重要的资产不是某一张卡，而是已经形成的三个稳定中心：

```text
YuHuoService
StarMoonService
FengWeiService
```

以及中央的：

```text
CanAoCombatRules
```

后续每个新机制都应围绕“统一入口、战斗作用域、玩家隔离、完整 Task 链、可回归测试”扩展。不要退回到“每张卡自己维护状态、静态变量、后台异步任务、手动注入模型”的旧模式。
