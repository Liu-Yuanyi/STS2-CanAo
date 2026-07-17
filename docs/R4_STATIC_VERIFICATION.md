# R4 静态核验记录

## 基线

- STS2：v0.108.0
- 反编译程序集 MVID：`F7D2A9E0-F1AE-4213-B874-1504473AAEDB`
- 参考源码：用户提供的 `现有工作区和反汇编源码.zip`

## 已核对 API

### 条件 AOE

```csharp
AttackCommand.Targeting(Creature target)
AttackCommand.TargetingAllOpponents(ICombatState combatState)
AttackCommand.Execute(PlayerChoiceContext? choiceContext)
```

`FeatherRanksCard` 使用同一个 `AttackCommand`，根据
`YuHuoService.IsTriggeredByYuHuo(this)` 选择单体或全体目标。

### 临时力量

```csharp
TemporaryStrengthPower
PowerCmd.Apply<T>(
    PlayerChoiceContext,
    Creature,
    decimal,
    Creature?,
    CardModel?,
    bool)
```

`TemporaryStrengthPower` 的游戏原生实现负责：

- 首次应用时同步添加 `StrengthPower`；
- 叠层时同步增加 Strength；
- 对应阵营回合结束时移除临时 Power；
- 撤销累计 Strength。

### 浴火事件

```csharp
IAfterYuHuoTrigger.AfterYuHuoTrigger(
    PlayerChoiceContext,
    YuHuoExecutionContext)
```

`YuHuoBannerPower` 检查 `EffectExecuted`，避免 AutoPlay 被拒绝时错误获得力量。

## R3.1 固化修复

- JSON 与源码校验使用 `Get-Content -Encoding UTF8`。
- `SacrificialPreparationCard` 移除导致 `List<object>` 推断的
  `Distinct(ReferenceEqualityComparer.Instance)`。
- 对 Owner 与 CombatState 增加显式守卫。
- 单体攻击在 `Targeting` 前检查 `cardPlay.Target`。

## 静态扫描

已确认源码中不存在：

```text
BaseLib
ModelDb.Inject
InjectModels
TaskHelper.RunSafely
async void
.Wait()
.Result
async Task<bool> Prefix
```

## 未完成的验证

当前执行环境没有 .NET 9 SDK 和 STS2 运行时，未声称完成：

- 本机 `dotnet build`；
- Godot PCK 实际加载；
- 游戏内伤害与临时力量实测。

这些由 `Deploy-Mod.ps1` 和 `INSTALL_R4.md` 的回归流程最终验收。
