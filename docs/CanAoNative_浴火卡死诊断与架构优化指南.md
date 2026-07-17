# CanAoNative：燃烧契约卡住问题诊断、浴火修复与架构优化指南

> 适用工作区：用户提供的 `CanAoNative` 工作区  
> 核对游戏版本：STS2 v0.108.0，commit `58694f64`  
> 核对程序集 MVID：`F7D2A9E0-F1AE-4213-B874-1504473AAEDB`  
> 目标：先修复 `Burning Pact -> Exhaust(浴火牌) -> 不再抽牌`，再逐步改善残傲机制架构。  
> 原则：P0 修复必须单独提交并实测；不要把 P1/P2 重构混入同一次提交。

---

## 一、最终诊断

### 1. Burning Pact 的真实执行顺序

反编译的 `BurningPact.OnPlay` 是：

```csharp
CardModel card = (
    await CardSelectCmd.FromHand(...)
).FirstOrDefault();

if (card != null)
    await CardCmd.Exhaust(choiceContext, card);

await CreatureCmd.TriggerAnim(...);
await CardPileCmd.Draw(choiceContext, Cards, Owner);
```

因此，只有 `await CardCmd.Exhaust(...)` 正常完成后，燃烧契约才会继续动画和抽牌。

### 2. 当前 Harmony Prefix 破坏了 `Task` 返回契约

当前 `YuHuoExhaustPatch`：

```csharp
TaskHelper.RunSafely(
    YuHuoResolver.ResolveBeforeExhaust(...));

return false;
```

`CardCmd.Exhaust` 的返回类型是 `Task`。Harmony Prefix 返回 `false` 会跳过原方法；但当前 Prefix 没有设置 `ref Task __result`。

结果：

```text
BurningPact
  -> await CardCmd.Exhaust(...)
  -> Harmony 跳过原方法
  -> 返回值保持引用类型默认值 null
  -> BurningPact 实际执行 await null
  -> NullReferenceException
  -> 后面的 TriggerAnim 和 Draw 永远不执行
```

这就是“浴火牌自动打出了，但燃烧契约始终不抽牌”的直接原因。

### 3. fire-and-forget 又制造了第二个错误

`TaskHelper.RunSafely(...)` 让浴火结算脱离原卡牌行动的异步链：

```text
燃烧契约所在 GameAction 已经异常/结束
浴火 Resolver 仍在后台继续使用同一个 PlayerChoiceContext 和卡牌
```

`Task.Yield()` 进一步扩大了竞态窗口。日志中的大量：

```text
YUHUO_RESOLVE
YUHUO_FAILED
NullReferenceException in CardModel.OnPlayWrapper
```

不是正常的多次触发，而是异步脱链后重复进入失败状态。

当前日志约有：

```text
78,280 次 YUHUO_RESOLVE
426,862 次 YUHUO_FAILED
```

必须先修复异步契约，再分析任何后续机制问题。

---

## 二、P0：立即修复

### 修改 1：Prefix 通过 `ref Task __result` 返回替代任务

`YuHuoExhaustPatch.cs` 必须满足：

```csharp
public static bool Prefix(..., ref Task __result)
```

命中浴火时：

```csharp
__result = YuHuoResolver.ResolveBeforeExhaust(...);
return false;
```

不要再使用：

```csharp
TaskHelper.RunSafely(...)
Task.Yield()
```

关键规则：

```text
原方法返回 Task
+ Prefix 跳过原方法
=> Prefix 必须给 __result 一个非 null Task
```

### 修改 2：Resolver 必须留在调用者的 await 链中

正确链条：

```text
BurningPact.OnPlay
  await CardCmd.Exhaust
    await YuHuoResolver
      await CardCmd.AutoPlay
      await 原始 CardCmd.Exhaust
  await TriggerAnim
  await Draw(2)
```

### 修改 3：嵌套 Exhaust 仍调用原方法

在浴火自动打出过程中，卡牌若有“消耗”，`OnPlayWrapper` 会再次调用 `CardCmd.Exhaust`。

此时 `_resolving` 中已有该卡：

```csharp
if (!state.TryBeginResolution(card))
    return true;
```

`return true` 的含义是让嵌套调用执行游戏原始 Exhaust。这样：

```text
外层 Exhaust -> 浴火 Resolver
自动打出 -> 内层 Exhaust -> 原始 Exhaust
Resolver 发现已在 Exhaust pile -> 结束
```

不会递归触发第二套浴火。

### 修改 4：异常日志必须记录完整堆栈

错误：

```csharp
Log.Error($"{ex.GetType().Name}: {ex.Message}");
```

正确：

```csharp
Log.Error(ex.ToString());
```

否则看不到本地代码行和内部异常。

### 可直接应用的补丁

使用随本指南生成的：

```text
CanAoNative_YuHuo_async_contract_fix.patch
```

在仓库根目录执行：

```powershell
git apply --check `
  "C:\路径\CanAoNative_YuHuo_async_contract_fix.patch"

git apply `
  "C:\路径\CanAoNative_YuHuo_async_contract_fix.patch"
```

---

## 三、P0 验收测试

测试前：

```powershell
git add -A
git commit -m "snapshot: before yuhuo async fix"

.\scripts\Deploy-Mod.ps1 -Configuration Release
```

第一次只启用：

```text
CanAoNative
```

不要同时启用 SpeedX、Rewind、伤害统计等会修改 GameAction 或时间流的 Mod。基础测试通过后再逐个恢复兼容性测试。

### 测试 A：普通浴火牌，不带消耗

预期：

```text
正常打出：1 次效果
被燃烧契约消耗：1 次免费自动打出，然后进入消耗堆
燃烧契约继续抽 2 张牌
```

### 测试 B：浴火牌，带消耗

`浴火斩` 当前属于这一类。

正常打出预期：

```text
手动效果 1 次
打出后的消耗触发浴火
免费自动打出 1 次
最终只进入消耗堆一次
总效果 2 次
```

燃烧契约消耗预期：

```text
免费自动打出 1 次
进入消耗堆
燃烧契约抽 2 张牌
总效果 1 次
```

### 测试 C：凤焰不息 1 层

正常打出“浴火 + 消耗”牌：

```text
手动效果 1 次
浴火基础触发 1 次
凤焰不息额外触发 1 次
总效果 3 次
```

燃烧契约消耗：

```text
浴火基础触发 1 次
凤焰不息额外触发 1 次
总效果 2 次
燃烧契约仍抽牌
```

### 日志验收

必须不再出现：

```text
await null 导致的 NullReferenceException
数万次 YUHUO_RESOLVE
数十万次 YUHUO_FAILED
```

一次燃烧契约测试应只有一组或两组有限日志。

---

## 四、部署与调试基础设施必须补强

### 1. 发布 PDB

当前部署脚本只复制 DLL、JSON、PCK，不复制 PDB，导致堆栈缺少本地源代码行号。

在 `Deploy-Mod.ps1` 中增加：

```powershell
$BuiltPdb = [IO.Path]::ChangeExtension($BuiltDll, ".pdb")

if (Test-Path $BuiltPdb) {
    Copy-Item $BuiltPdb `
        (Join-Path $StageDir "CanAoNative.pdb") `
        -Force
}
```

开发期保留 PDB。发布 Workshop 时再决定是否移除。

### 2. 修复验证脚本变量名

`Verify-NoBaseLib.ps1` 第 1 行当前是：

```powershell
$ErrorActionPrevention = "Stop"
```

应改为：

```powershell
$ErrorActionPreference = "Stop"
```

### 3. manifest 增加最低游戏版本

日志明确提示未声明最低版本。当前反编译和测试基线是 v0.108.0：

```json
"min_game_version": "0.108.0"
```

### 4. Release 禁止逐次高频日志

不要在每个循环、每个失败帧打印 Info。建议：

```csharp
#if DEBUG
Log.Info(...);
#endif
```

或只输出一次完整结算摘要：

```text
card, owner, source pile, trigger count, final pile, elapsed time
```

---

## 五、现有架构评估

## 做得好的部分

1. 已完全移除 BaseLib。
2. 游戏 DLL、Harmony、GodotSharp 都设置 `Private=false`。
3. 使用 `ModHelper.AddModelToPool`，未手动 `ModelDb.Inject`。
4. 战斗内生卡使用 `CombatState.CreateCard`。
5. 伤害与格挡使用游戏 Commands。
6. 部署脚本校验构建 DLL 与安装 DLL 哈希。
7. 浴火已拆成 Service / State / Resolver / Patch，而不是把全部逻辑塞进单个卡类。

这些方向应该保留。

---

## 六、P1：必须尽快修复的架构问题

### 1. `YuHuoService` 的单槽全局缓存不够稳

当前：

```csharp
private static ICombatState? _activeCombat;
private static YuHuoCombatState? _activeState;
```

问题：

- Rewind/重放可能复用或替换 CombatState；
- 日后测试框架可能同时持有多个 CombatState；
- 生命周期不显式；
- 状态无法参与存档和多人同步。

近期最小改进：

```csharp
private static readonly ConditionalWeakTable<
    ICombatState,
    YuHuoCombatState> States = new();
```

如果接口类型不接受，使用它的实际引用类型或封装对象键。

最终方案：创建 `CanAoCombatRules : AbstractModel`，通过：

```csharp
ModHelper.SubscribeForCombatStateHooks(...)
```

让规则对象成为原生战斗 Hook listener，并由每个 CombatState 持有对应状态。

### 2. 使用明确的引用相等比较器

当前字典和 HashSet 应明确使用实例身份：

```csharp
new Dictionary<CardModel, int>(
    ReferenceEqualityComparer.Instance);

new HashSet<CardModel>(
    ReferenceEqualityComparer.Instance);
```

虽然当前 `CardModel` 没有覆盖 `Equals`，明确声明意图能避免未来游戏版本改变行为。

### 3. 临时浴火没有真正接入回合结束

`RemoveExpired()` 当前没有调用者，临时浴火仅在查询时懒清理。

必须在规则对象的回合结束 Hook 中清理，并按玩家区分：

```text
玩家 A 回合结束不能清除玩家 B 的授予
```

每条授予至少保存：

```text
card instance
owner
turn sequence / turn number
expiration policy
```

### 4. 浴火执行上下文缺失

后续卡牌需要判断：

```text
这张牌是否“因浴火而触发”
当前是第几次浴火触发
本次总触发次数是多少
触发来源是固有、临时、Power 还是遗物
```

建立：

```csharp
public sealed record YuHuoExecutionContext(
    CardModel Card,
    int TriggerIndex,
    int TriggerCount,
    bool CausedByEthereal,
    PileType? OriginalPile);
```

Resolver 在每次 AutoPlay 前压栈、结束后出栈。

卡牌效果统一查询：

```csharp
YuHuoService.IsResolvingThisCard(this)
YuHuoService.CurrentContext(this)
```

这将支持：

- 羽列千军：因浴火触发时改为 AOE；
- 浴火打击：因浴火触发时获得月；
- 浴火军旗：每次浴火效果后获得临时力量；
- 涅槃火种：本场第一次浴火额外触发。

### 5. 触发次数不应硬编码 `FengYanBuXiPower`

当前：

```csharp
FengYanBuXiPower? power = ...
return 1 + power.Amount;
```

这无法扩展“涅槃火种”等来源。

改为修改器管线：

```csharp
public interface IYuHuoTriggerCountModifier
{
    int ModifyYuHuoTriggerCount(
        CardModel card,
        int currentCount);
}
```

Resolver 遍历当前 CombatState 的 hook listeners：

```text
凤焰不息 Power：+1
涅槃火种 Relic：本场第一次 +1
其他未来机制：按规则修改
```

并记录哪些模型修改了次数，便于日志与多人确定性。

### 6. 建立浴火自定义事件

至少定义：

```csharp
BeforeYuHuoResolved(...)
BeforeEachYuHuoTrigger(...)
AfterEachYuHuoTrigger(...)
AfterYuHuoResolved(...)
```

Power、遗物和卡牌通过接口监听，而不是让 Resolver 直接引用所有具体类。

---

## 七、P1：星/月/星月合击问题

### 1. 当前合成触发点分散且不完整

`GainStarCard`、`GainMoonCard` 手动调用：

```csharp
StarMoonHelper.CheckAndResolve(Owner)
```

未来以下来源会忘记调用：

```text
遗物获得星/月
药水获得星/月
回合开始 Power 获得星/月
消耗牌效果获得星/月
多人效果获得星/月
```

应由统一规则对象监听：

```csharp
AfterPowerAmountChanged(...)
```

当变化的 Power 是 `StarPower` 或 `MoonPower` 且变化为正时，统一尝试合成。

卡牌本身不再调用 `StarMoonHelper`。

### 2. 必须增加重入保护

合成时降低 Star/Moon 数量也会触发 Power amount hook。

使用每玩家保护：

```csharp
HashSet<Player> _resolvingStarMoon;
```

或状态枚举：

```text
Idle
Resolving
```

并放在 `try/finally` 中恢复。

### 3. 不要直接 `SetAmount`

当前：

```csharp
star.SetAmount(star.Amount - 1);
moon.SetAmount(moon.Amount - 1);
```

这绕过：

- BeforePowerAmountChanged；
- ModifyPowerAmount；
- AfterPowerAmountChanged；
- History；
- 自动移除；
- 其他 Mod 的 Power 监听。

改为：

```csharp
await PowerCmd.ModifyAmount(
    choiceContext,
    star,
    -pairCount,
    applier: null,
    cardSource: null);
```

Moon 同理。

### 4. 一次计算 pairCount

避免 `while` 每次重新读取和等待：

```csharp
int pairCount = Math.Min(star.Amount, moon.Amount);
```

一次扣除，然后顺序生成 `pairCount` 张衍生牌。

### 5. `StarMoonStrike` 实际缺少消耗关键字

设计与本地化写的是：

```text
虚无。……消耗。
```

当前代码只有：

```csharp
CardKeyword.Ethereal
```

必须改成：

```csharp
public override IEnumerable<CardKeyword> CanonicalKeywords =>
[
    CardKeyword.Ethereal,
    CardKeyword.Exhaust
];
```

否则正常打出后会进入弃牌堆，与设计不符。

---

## 八、P1：凤威架构目前无法支持完整卡表

设计中同时存在：

```text
永久凤威
本回合获得凤威
本回合失去凤威
复辟：忽略临时凤威
本回合是否获得过凤威
每回合第一次获得凤威
```

单个 `FengWeiPower` 无法表达这些语义。

建议拆成：

```text
FengWeiPower
  永久凤威，可正可负

TemporaryFengWeiPower
  本回合修正，可正可负，回合结束清除

CanAoTurnState.IgnoreTemporaryFengWei
  复辟使用

CanAoTurnState.FengWeiGainCount
  本回合获得记录
```

统一入口：

```csharp
FengWeiService.GetEffectiveAmount(player)
FengWeiService.GainPermanent(...)
FengWeiService.ModifyTemporary(...)
FengWeiService.SetPermanent(...)
```

有效凤威：

```csharp
permanent +
    (turnState.IgnoreTemporaryFengWei
        ? 0
        : temporary)
```

`FengWeiPower.ModifyDamageAdditive` 和格挡 Hook 只调用 Service，不直接读取自己 Amount。

---

## 九、P2：建立统一的残傲战斗状态

卡表需要大量“本回合/本场战斗”事实：

```text
本回合消耗过几张牌
本回合第一次消耗
本回合生成过几张星月合击
本回合打出过几张诏令
本回合获得过几次凤威
本场第一次浴火
上一回合结束时剩余多少格挡
回合结束时手牌是否为空
```

不要让每张卡、Power、遗物各自建立静态变量。

建议：

```csharp
CanAoCombatRules : AbstractModel
  Dictionary<Player, CanAoPlayerCombatState>

CanAoPlayerCombatState
  TurnNumber
  ExhaustedThisTurn
  YuHuoTriggersThisTurn
  StarMoonGeneratedThisTurn
  StarMoonPlayedThisTurn
  EdictsPlayedThisTurn
  FengWeiGainedThisTurn
  FirstYuHuoConsumedThisCombat
  IgnoreTemporaryFengWeiThisTurn
  BlockAtPreviousTurnEnd
```

`CanAoCombatRules` 通过原生 hooks 更新：

```text
AfterCardExhausted
AfterCardPlayed / AfterCardPlayedLate
AfterPowerAmountChanged
BeforeSideTurnStart
BeforeSideTurnEnd
```

卡牌只读取状态，不自行维护重复计数。

---

## 十、浴火的最终推荐执行管线

```text
CardCmd.Exhaust 被调用
  |
  +-- 没有浴火 --------------------> 原始 Exhaust
  |
  +-- 当前已在浴火解析 ------------> 原始 Exhaust
  |
  +-- 开始浴火解析
        |
        +-- 创建 YuHuoExecutionContext
        +-- 计算触发次数修改器
        |
        +-- for each trigger
        |     +-- BeforeEachYuHuoTrigger
        |     +-- AutoPlay（仍在同一 await 链）
        |     +-- AfterEachYuHuoTrigger
        |
        +-- 若未进入 Exhaust pile，执行原始 Exhaust
        +-- AfterYuHuoResolved
        +-- finally 清理上下文与重入锁
```

绝对禁止：

```text
fire-and-forget
Task.Yield 逃离调用者
async void
.Wait()
.Result
全局 bool IsResolving
跳过 Task 方法却不设置 __result
```

---

## 十一、建议提交顺序

```text
P0-1 fix: preserve Task result when intercepting CardCmd.Exhaust
P0-2 test: add Burning Pact regression checklist and PDB deployment
P1-1 refactor: introduce YuHuo execution context and listener interfaces
P1-2 refactor: move YuHuo state to combat-scoped rules object
P1-3 fix: add Exhaust keyword to StarMoonStrike
P1-4 refactor: centralize Star/Moon combination on power changes
P1-5 refactor: split permanent and temporary FengWei
P2-1 feat: introduce CanAo per-player combat/turn state
P2-2 feat: implement temporary YuHuo grants
P2-3 feat: implement Sacrificial Preparation
P2-4 feat: implement FengYanBuXi through trigger modifier interface
```

每个提交都要：

```powershell
dotnet build
.\scripts\Deploy-Mod.ps1
只启用 CanAoNative 测试
保存日志
确认 DLL 哈希
```

---

## 十二、交给本地 AI 的硬性约束

1. 不得重新引入 BaseLib。
2. 不得调用 `ModelDb.Inject`。
3. 不得使用 fire-and-forget 处理卡牌、选择、抽牌、移动牌堆或 Power 命令。
4. Patch 返回 `Task` 的方法时，跳过原方法必须设置 `ref Task __result`。
5. 不得使用 `.Wait()` 或 `.Result`。
6. 不得一次同时重构浴火、星月、凤威三个系统。
7. P0 修复完成后必须先做 Burning Pact 回归测试。
8. 所有异常日志使用 `ex.ToString()`。
9. 所有高频日志在 Release 中关闭或限流。
10. 修改方法签名前必须以当前 v0.108.0 反编译源码为准。
