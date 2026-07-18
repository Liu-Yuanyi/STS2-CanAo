# R9：遗物、药水与密诏调整

> 构建标记：`CANAO_NATIVE_R9_RELICS_POTIONS_20260717`
> 游戏基线：STS2 v0.109.0（遗物/药水 API 已对照 v0.109.0 反编译源码核对）

## 密诏调整（设计变更，含修复）

```text
旧：将 1（2）张诏令加入手牌。消耗。
新：将 1 张诏令加入弃牌堆。不消耗。升级后加入的是诏令+。
```

已知问题与修复：

- **密诏悬浮**：`ExtraHoverTips` 现在传 `IsUpgraded`，密诏+显示诏令+预览；
- **御令瓶悬浮**：固定显示诏令+预览（`upgrade: true`）；
- **密诏文本**：`{IfUpgraded:show:+|}`（冒号语法，与原生 TRUE_GRIT 一致），
  此前误写 `show(+|)` 导致文本异常；
- **"无效果"说明**：日志显示三次密诏正常打出、零异常，且同一
  `EdictService.Generate` 管线（御令瓶，入手牌）验证有效——效果实际
  进入了弃牌堆，但弃牌堆没有即时可见反馈。测试时请在抽牌堆耗尽后
  确认诏令洗回。

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
| 天凤军印 | 普通 | 每次打出诏令后获得 4 格挡 | `IAfterEdictPlayed` |
| 青鸾羽衣 | 罕见 | 回合开始时，若上回合剩余 ≥5 格挡，获得 1 月 | `AfterSideTurnEndLate` 快照格挡 + `AfterPlayerTurnStart` 结算 |
| 合击武典 | 罕见 | 每打出 4 张星月合击，下一张效果翻倍 | `IAfterStarMoonPlayed` 计数 + `IAfterStarMoonGenerated` 双倍化实例数值 |
| 战碑 | 稀有 | 战斗开始获得 2 凤威；第一回合星月合击只造成伤害 | `BeforeSideTurnStart` + `ModifyBlockAdditive` |
| 孤王玉座 | 稀有 | 回合结束时手牌为空，下回合开始获得 1 费和星月合击+ | `AfterSideTurnEndLate` 快照 + `PlayerCmd.GainEnergy` + 升级版生成 |
| 帝国税契 | 商店 | 战斗开始将 1 张诏令加入手牌；每次打出诏令失去 1 金币 | `BeforeSideTurnStart` + `IAfterEdictPlayed` |

浴火触发次数修改走浴火监听注册表（牌 → Power → 遗物），
与凤焰不息共用同一扩展点；星月王冠的"第一次获得凤威"目前由遗物自身
按回合记录，未来若多张牌需要该语义再下沉到 `FengWeiService` 事件。

## R9 药水（注册进 SharedPotionPool 测试）

| 药水 | 稀有度 | 效果 |
| --- | --- | --- |
| 琼浆 | 普通 | 获得 4 月 |
| 凤威酒 | 罕见 | 获得 2 点凤威，在本回合获得 3 点凤威 |
| 御令瓶 | 稀有 | 将 2 张诏令+加入手牌 |

## 存档兼容说明

- 全部 Mod 战斗状态都挂在 `ICombatState` 的 `ConditionalWeakTable` 上，
  战斗结束即释放，不参与存档序列化；
- 遗物锁存字段是战斗/回合作业域的瞬态，不写入存档（原生同类字段同处理）；
- 模型 ID 由类名派生，存档中的卡牌/Power 引用可正常往返。

## 实机验收清单

1. 密诏：牌面无"消耗"关键词；未升级→弃牌堆得诏令；升级→弃牌堆得
   诏令+（数值 2）；悬浮显示对应的诏令/诏令+预览；御令瓶悬浮显示诏令+。
2. 涅槃火种：每场战斗第一次消耗浴火牌时触发 2 次效果；同一战斗第二次
   消耗浴火牌恢复 1 次；下一战斗重置。
3. 星月王冠：每回合第一次获得凤威（示威/凤威酒/承天受命均可）时手牌
   +1 星月合击；同回合第二次获得凤威不再触发；下一回合重置。
4. 天凤军印：每打出 1 张诏令获得 4 格挡。
5. 青鸾羽衣：上回合结束时格挡 ≥5，本回合开始获得 1 月；否则不触发。
6. 合击武典：打出第 4 张星月合击后，下一张星月合击伤害与格挡翻倍。
7. 战碑：战斗开始凤威 +2；第一回合星月合击不给格挡，第二回合恢复。
8. 孤王玉座：回合结束手牌为空 → 下回合开始 +1 能量、+1 张星月合击+。
9. 帝国税契：战斗开始手牌 +1 诏令；每打出诏令 -1 金币（不为负）。
10. 琼浆：获得 4 月。
11. 凤威酒：凤威 +2（永久）、本回合 +3，星月合击数值即时反映。
12. 御令瓶：手牌 +2 张诏令+，其数值为 2。
13. 战斗外不能用药水（CombatOnly）。
14. 存档/读档后继续游戏无异常，Mod 模型 ID 正常解析。
15. 日志含 `CANAO_NATIVE_R9_RELICS_POTIONS_20260717`，无
    `NullReferenceException`、`YUHUO_RESOLVE_FAILED`、`STARMOON_FAILED`。
