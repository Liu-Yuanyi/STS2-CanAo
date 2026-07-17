# R4：浴火事件系统的正式卡牌验证

## 羽列千军

设计：

```text
1费，普通攻击。
浴火。造成10/14点伤害。
若本牌因浴火而触发，改为对所有敌人造成伤害。
```

实现要点：

```csharp
YuHuoService.IsTriggeredByYuHuo(this)
```

返回真时使用：

```csharp
TargetingAllOpponents(combatState)
```

否则使用正常的 `cardPlay.Target`。

这验证“自动打出”与“浴火触发”没有混为一谈。

## 浴火军旗

设计：

```text
2/1费，罕见能力。
每次因浴火触发牌效果后，本回合获得2点力量。
```

`YuHuoBannerPower` 实现：

```csharp
IAfterYuHuoTrigger
```

并检查：

```csharp
context.EffectExecuted
```

只有 AutoPlay 真正执行了卡牌效果，才应用临时力量。

临时力量由：

```csharp
YuHuoBannerTemporaryStrengthPower : TemporaryStrengthPower
```

管理。该游戏原生基类负责：

1. 应用时增加 Strength；
2. 多次触发时正确叠层；
3. 对应玩家回合结束时撤销全部临时 Strength。

## 兼容性

- 浴火军旗不写入 `YuHuoResolver`。
- 凤焰不息通过触发次数修改器增加触发次数。
- 每次触发后浴火军旗单独获得一次临时力量。
- 燃烧契约仍在同一 Task 链中等待浴火完成。
