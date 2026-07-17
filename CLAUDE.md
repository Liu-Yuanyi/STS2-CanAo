# CLAUDE.md — CanAoNative 本地 AI 工作上下文

## 项目目标

为《杀戮尖塔 2》v0.108.0 开发“残傲”原生 Mod。

当前版本：**R7，基线为用户实机验证通过的 R6 工作区**。

构建标记：

```text
CANAO_NATIVE_R7_EXHAUST_EVENTS_20260717
```

## 已完成

- 零 BaseLib 原生工程；
- 稳定的浴火异步补丁、执行上下文和事件层；
- 临时浴火、牺牲准备、凤焰不息；
- 羽列千军、浴火军旗；
- 永久凤威、本回合凤威和 `FengWeiService`；
- 示威、暂避锋芒；
- 星月合击生成/打出事件层；
- 按战斗、按玩家保存的星月回合计数；
- 盘旋、星月伐魔、天凤军阵；
- 统一消耗事件层和按玩家消耗回合历史；
- 征召、浴火打击、焚膏继晷、清宫、凤骨再燃。

## 消耗事件层

统一使用：

```text
ExhaustService
ExhaustCombatState
ExhaustRecord
IAfterCanAoCardExhausted
```

记录顺序必须保持：

```text
游戏完成 CardCmd.Exhaust
→ CanAoCombatRules.AfterCardExhausted
→ 快照 HadYuHuo / ResolvedByYuHuo / SourceModel
→ 写入按战斗、按玩家的回合历史
→ IAfterCanAoCardExhausted
```

所有未来"本回合消耗过牌"的卡牌、Power、遗物都必须读取
`ExhaustService`，不得自行使用静态变量或分散计数。

消耗来源分类读取 `ExhaustRecord.Cause`；`SourceModel` 是
best-effort 信息字段，禁止把关键逻辑建立在它之上。

## 星月事件层

统一使用：

```text
StarMoonService
StarMoonCombatState
StarMoonGenerationContext
StarMoonPlayedContext
IBeforeStarMoonGenerated
IAfterStarMoonGenerated
IAfterStarMoonPlayed
```

生成顺序必须保持：

```text
扣除星/月
→ 创建具体 StarMoonStrike 实例
→ BeforeStarMoonGenerated
→ AddGeneratedCardToCombat
→ RecordGenerated
→ AfterStarMoonGenerated
```

打出顺序：

```text
游戏正常完成 StarMoonStrike CardPlay
→ CanAoCombatRules.AfterCardPlayedLate
→ RecordPlayed
→ AfterStarMoonPlayed
```

所有未来“本回合生成过/打出过几张星月合击”的卡牌都必须读取
`StarMoonService`，不得自行使用静态变量或分散计数。

## 回合清理语义

临时浴火、星月回合历史和消耗回合历史由：

```text
CanAoCombatRules.AfterSideTurnEndLate
```

清理。这样临时浴火能够覆盖虚无消耗，正常 `AfterSideTurnEnd` 监听者也能在清理前读取本回合历史。

`TemporaryFengWeiPower` 和 `PanXuanPower` 同样在
`AfterSideTurnEndLate` 归零。

## R5/R6 基线保护

`scripts/Verify-R7.ps1` 会验证未被 R7 有意修改的 R5+R6 核心文件 SHA-256，尤其包括：

- 浴火 Patch、Service、State、Resolver；
- 羽列千军、牺牲准备、浴火军旗；
- 永久凤威与 `FengWeiService`；
- 示威、暂避锋芒；
- 星月合击本体与星月事件层；
- 盘旋、星月伐魔、天凤军阵。

## 不可违反的规则

1. 禁止引用 BaseLib 或 `Alchyr.Sts2.ModAnalyzers`。
2. 禁止 `ModelDb.Inject`、`InjectModels` 或直接构造 canonical 模型。
3. 禁止 fire-and-forget、`async void`、`.Wait()`、`.Result`。
4. Harmony Prefix 跳过返回 `Task` 的原方法时，必须设置非空 `ref Task __result`。
5. 临时浴火按战斗、玩家和卡牌实例保存。
6. 浴火来源只通过 `YuHuoService` 和浴火执行上下文判断。
7. 星月生成与打出事实只通过 `StarMoonService` 记录和查询。
8. 星/月数量变化必须使用 `PowerCmd`。
9. 凤威读写优先通过 `FengWeiService`。
10. 监听者执行必须留在原始 `Task` 链中。
11. 涉及新 API/Hook/Harmony 目标时，先核对当前反编译源码。
12. PowerShell 读取源码与 JSON 时显式使用 UTF-8。
13. 每次只做一个可独立验收的小阶段。
14. 消耗事实只通过 `ExhaustService` 记录和查询。

## 下一阶段建议

R8 建立诏令系统（衍生牌【诏令】与 `EdictService`），优先支持：

- 传令；
- 密诏；
- 王权；
- 帝国余威；
- 承天受命；
- 天凤形态。
