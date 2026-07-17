# CanAoNative R2 编译修复

## 修复内容

`CanAoCombatRules` 直接继承 `AbstractModel`。在当前 STS2 v0.108.0 中，
`AbstractModel.ShouldReceiveCombatHooks` 是抽象属性，所有直接派生类都必须实现。

本版本加入：

```csharp
public override bool ShouldReceiveCombatHooks => true;
```

必须设置为 `true`，因为该模型通过 `ModHelper.SubscribeForCombatStateHooks`
订阅战斗 Hook，并负责：

- `AfterPowerAmountChanged`：统一检查星/月组合；
- `BeforeSideTurnEnd`：清理到期的临时浴火状态。

若设置为 `false`，即使能够编译，这两项规则也不会正常收到战斗 Hook。

## 构建标记

新日志应包含：

```text
CANAO_NATIVE_FIX_20260716_R2
```

## 本次范围

本版本只修复编译阻断问题，没有改变浴火异步结算、星月组合或凤威数值逻辑。
