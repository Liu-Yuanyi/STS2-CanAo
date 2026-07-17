# CanAoNative R3：浴火事件管线、临时浴火与牺牲准备

构建标记：

```text
CANAO_NATIVE_R3_YUHUO_EVENTS_20260717
```

目标游戏基线：STS2 v0.108.0，程序集 MVID
`F7D2A9E0-F1AE-4213-B874-1504473AAEDB`。

## 本阶段完成内容

### 1. 浴火生命周期事件

新增四个可扩展接口：

- `IBeforeYuHuoResolved`
- `IBeforeYuHuoTrigger`
- `IAfterYuHuoTrigger`
- `IAfterYuHuoResolved`

监听者按固定顺序收集：

1. 当前卡牌；
2. 卡牌拥有者的 Power；
3. 卡牌拥有者的遗物。

收集结果会先创建快照，再逐个 `await`，避免回调过程中 Power 或遗物列表变化导致枚举异常。

### 2. 浴火执行上下文

- `YuHuoResolutionContext`：描述整次浴火结算。
- `YuHuoExecutionContext`：描述其中一次具体触发。
- `TriggerIndex` 从 1 开始。
- `EffectExecuted` 表示本次 AutoPlay 是否真正完成了出牌；不可打出、无合法目标或被 `ShouldPlay` 阻止时为 `false`。
- `YuHuoService.IsTriggeredByYuHuo(card)` 可用于“若本牌因浴火触发”类效果。

### 3. 可扩展触发次数

`YuHuoResolver` 不再认识具体的 `FengYanBuXiPower`。它只调用
`IYuHuoTriggerCountModifier`。

触发次数修改器同样支持：

- 卡牌；
- Power；
- 遗物。

因此未来的“涅槃火种”不需要修改 Resolver。

### 4. 战斗作用域临时浴火

临时浴火按以下三项区分：

- `ICombatState`；
- 玩家；
- 具体 `CardModel` 实例。

两张同名卡只选择其中一张时，只有被选择的实例获得浴火。对应玩家回合结束时，`CanAoCombatRules.BeforeSideTurnEnd` 主动清理到期授予。

### 5. 牺牲准备

实现设计稿中的：

```text
0费。选择手牌中2/3张非能力牌，使其本回合获得浴火。消耗。
```

行为说明：

- 基础选择 2 张，升级后 3 张；
- 能力牌不能被选择；
- 合法候选不足时自动选择全部合法候选；
- 状态绑定卡牌实例，不绑定卡牌类型；
- 不可打出的状态牌或诅咒仍属于“非能力牌”，可以被选择；它们被浴火自动打出时会遵守游戏原生 `AutoPlay` 对不可打出牌的处理。

### 6. 凤焰不息正式卡牌

新增 3 费稀有能力牌，升级后 2 费，施加一层
`FengYanBuXiPower`。每层令浴火额外触发一次。

## 必测回归

### A. R2 基础回归

1. 用燃烧契约消耗浴火斩；
2. 浴火斩触发一次；
3. 燃烧契约继续抽 2 张牌；
4. 日志无 `YUHUO_RESOLVE_FAILED`。

### B. 牺牲准备

1. 手牌放入两张同名打击；
2. 用牺牲准备只选择其中一张；
3. 用燃烧契约分别消耗两张；
4. 只有被选择的具体实例自动打出；
5. 燃烧契约两次都能继续抽牌。

### C. 到期清理

1. 牺牲准备给一张牌临时浴火；
2. 不消耗它，结束回合；
3. 下一回合再消耗；
4. 不应触发浴火。

### D. 凤焰不息

1. 打出凤焰不息；
2. 用燃烧契约消耗临时浴火牌；
3. 该牌应自动打出两次；
4. 燃烧契约仍继续抽牌。

### E. 事件上下文

以后实现“浴火打击”或“羽列千军”时，只允许使用：

```csharp
YuHuoService.IsTriggeredByYuHuo(this)
YuHuoService.GetCurrentContext(this)
```

不要用 `cardPlay.IsAutoPlay` 代替，因为其他机制也会自动打出卡牌。

## 当前已知限制

- 本环境无法连接用户本机游戏 DLL 进行真实编译与启动；本版本依据用户提供的 v0.108.0 反编译源码核对方法签名。
- Power 小图标仍依赖游戏的 packed atlas，星/月等自定义 Power 可能继续显示缺失图标警告；这不影响本阶段机制。
- 临时浴火目前属于战斗运行时状态，不写入中途战斗存档。需要单独验证游戏是否允许在玩家选择过程中保存，以及恢复时是否要序列化临时授予。
