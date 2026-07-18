# R9：遗物、药水与密诏调整

> 构建标记：`CANAO_NATIVE_R9_RELICS_POTIONS_20260717`
> 游戏基线：STS2 v0.109.0（遗物/药水 API 已对照 v0.109.0 反编译源码核对）

## 密诏调整（设计变更）

```text
旧：将 1（2）张诏令加入手牌。消耗。
新：将 1 张诏令加入弃牌堆。消耗。升级后加入的是诏令+。
```

`EdictService.Generate` 相应扩展为：

```csharp
Generate(choiceContext, player, count,
    PileType pileType = PileType.Hand,
    bool upgraded = false)
```

升级令牌通过原生 `CardCmd.Upgrade(edict)` 在入堆前完成，
仍走唯一生成入口，回合计数不受影响。

## R9 遗物（注册进 SharedRelicPool 测试）

| 遗物 | 稀有度 | 效果 | 实现 |
| --- | --- | --- | --- |
| 涅槃火种 | 稀有 | 每场战斗第一次浴火结算时，额外触发 1 次 | `IYuHuoTriggerCountModifier`，战斗内锁存，`BeforeCombatStart`/`AfterCombatEnd` 复位 |
| 星月王冠 | 稀有 | 每回合第一次获得凤威（永久或临时）时，获得 1 张星月合击 | 重写 `AfterPowerAmountChanged`，按回合锁存，`AfterPlayerTurnStart` 复位 |

浴火触发次数修改走浴火监听注册表（牌 → Power → 遗物），
与凤焰不息共用同一扩展点；星月王冠的"第一次获得凤威"目前由遗物自身
按回合记录，未来若多张牌需要该语义再下沉到 `FengWeiService` 事件。

## R9 药水（注册进 SharedPotionPool 测试）

| 药水 | 稀有度 | 效果 |
| --- | --- | --- |
| 凤威酒 | 罕见 | 获得 2 点凤威，在本回合获得 3 点凤威 |
| 御令瓶 | 稀有 | 将 2 张诏令+加入手牌 |

## 存档兼容说明

- 全部 Mod 战斗状态都挂在 `ICombatState` 的 `ConditionalWeakTable` 上，
  战斗结束即释放，不参与存档序列化；
- 遗物锁存字段是战斗/回合作业域的瞬态，不写入存档（原生同类字段同处理）；
- 模型 ID 由类名派生，存档中的卡牌/Power 引用可正常往返。

## 实机验收清单

1. 密诏：未升级时诏令进弃牌堆；升级后弃牌堆中是诏令+（数值 2）。
2. 涅槃火种：每场战斗第一次消耗浴火牌时触发 2 次效果；同一战斗第二次
   消耗浴火牌恢复 1 次；下一战斗重置。
3. 星月王冠：每回合第一次获得凤威（示威/凤威酒/承天受命均可）时手牌
   +1 星月合击；同回合第二次获得凤威不再触发；下一回合重置。
4. 凤威酒：凤威 +2（永久）、本回合 +3，星月合击数值即时反映。
5. 御令瓶：手牌 +2 张诏令+，其数值为 2。
6. 战斗外不能用药水（CombatOnly）。
7. 存档/读档后继续游戏无异常，Mod 模型 ID 正常解析。
8. 日志含 `CANAO_NATIVE_R9_RELICS_POTIONS_20260717`，无
   `NullReferenceException`、`YUHUO_RESOLVE_FAILED`、`STARMOON_FAILED`。
