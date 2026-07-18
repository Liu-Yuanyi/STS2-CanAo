# R8：诏令系统

> 构建标记：`CANAO_NATIVE_R8_EDICT_SYSTEM_20260717`
> 游戏基线：STS2 v0.109.0（新 API 已对照 v0.109.0 反编译源码核对：
> `CardModel.AddKeyword/RemoveKeyword`、`AfterPlayerTurnStart`）

## 目标

建立衍生牌【诏令】与统一生成/打出事实层，替代未来的分散计数：

```text
EdictService
EdictCombatState        GeneratedThisTurn / PlayedThisTurn（按战斗、按玩家）
EdictPlayedContext
IAfterEdictPlayed
```

## 权威顺序

```text
生成：EdictService.Generate
→ combatState.CreateCard<EdictCard>
→ CardPileCmd.AddGeneratedCardToCombat(Hand)
→ RecordGenerated

打出：游戏完成 EdictCard CardPlay
→ CanAoCombatRules.AfterCardPlayedLate
→ RecordPlayed
→ IAfterEdictPlayed
```

回合清理由 `CanAoCombatRules.AfterSideTurnEndLate` 执行
（`EdictService.ClearForPlayers`），与其他回合计数同一时点。

所有未来生成诏令的卡牌、遗物、Power 必须走 `EdictService.Generate`；
所有"本回合第几次打出诏令"的判断必须读 `EdictService`。

## 诏令本体

0 费衍生技能，`Token` 稀有度，关键词：保留 + 消耗。
打出时选择 1 张手牌消耗：攻击牌 → 1（2）星；技能牌 → 1（2）月；
能力牌 → 1（2）星与 1（2）月。升级数值由 `CardsVar(1)` 承载。

## R8 卡牌（均在 ColorlessCardPool 测试）

| 卡牌 | 费用 | 效果 | 验证点 |
| --- | --- | --- | --- |
| 传令 | 1 | 将 1 张诏令加入手牌。消耗。**升级移除消耗** | `RemoveKeyword(CardKeyword.Exhaust)` |
| 密诏 | 0 | 将 1（2）张诏令加入手牌。消耗 | 批量生成计数 |
| 王权 | 2→1 | 消耗手牌中所有诏令。每消耗 1 张，获得 1 点凤威并抽 1 张牌 | 筛选批量消耗、逐张结算 |
| 帝国余威 | 1 | 每回合第二次打出诏令时，获得 1 点凤威。**升级获得固有** | `AddKeyword(CardKeyword.Innate)`、`PlayedThisTurn == 2` |
| 承天受命 | 3→2 | 获得 3 点凤威。将 2 张诏令加入手牌。消耗 | 永久凤威 + 生成 |
| 天凤形态 | 3 | 获得 3（5）点凤威。每回合开始时，将 1 张诏令加入手牌 | `AfterPlayerTurnStart` |

## 实机验收清单

1. 诏令本体：保留在回合末不弃牌；打出后选 1 张手牌消耗，按类型给星/月，
   星月组合立即解析；自身进入消耗堆。
2. 升级诏令（如经其他效果升级）：数值变 2。
3. 传令升级后牌面不再出现"消耗"。
4. 密诏升级后给 2 张诏令。
5. 王权消耗 2 张诏令 → 获得 2 点凤威、抽 2 张牌；手牌无诏令时无效果。
6. 帝国余威：每回合仅第二次打出诏令时触发 1 次凤威；第三、四次不触发；
   升级后带固有。
7. 承天受命：3 点永久凤威 + 2 张诏令；升级后费用 2。
8. 天凤形态：打出后每回合开始手牌 +1 诏令。
9. 回合结束后 `EdictService` 计数清零（帝国余威下回合重新计第二次）。
10. 日志无 `NullReferenceException`、`STARMOON_FAILED`、
    `YUHUO_RESOLVE_FAILED`、`EDICT` 相关错误。

## 风险与备忘

- 王权消耗带浴火的诏令时不会触发浴火（诏令本身无浴火）；若消耗其他
  浴火牌则正常走浴火结算，由消耗事件层记录。
- 帝国余威通过 `IAfterEdictPlayed` 读取已更新的计数，避免与
  `AfterCardPlayedLate` 里记录顺序的竞争。
- `RemoveKeyword`/`AddKeyword` 会触发 `KeywordsChanged`，牌面关键词行
  与悬浮提示自动刷新，无需额外处理。
