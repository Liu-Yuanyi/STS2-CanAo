# R7：统一消耗事件层

> 构建标记：`CANAO_NATIVE_R7_EXHAUST_EVENTS_20260717_FIX1`
> 游戏基线：STS2 v0.109.0（API 签名已对照 v0.109.0 反编译源码逐条核对）

## 目标

为所有"本回合消耗过牌"类效果建立唯一事实来源，替代分散的静态计数：

- 本回合消耗总数；
- 本回合第一次消耗（`SequenceNumberThisTurn == 1`）；
- 最近消耗的卡牌与完整记录列表；
- 被消耗牌的类型快照；
- 被消耗时是否拥有浴火（`HadYuHuo`）；
- 消耗来源：`CausedByEthereal`（虚无）、`ResolvedByYuHuo`（浴火结算的最终消耗）、
  `SourceModel`（选择栈上的发起者，best-effort）。

## 关键设计：零 Harmony

v0.109.0 原生已有可靠 Hook，无需新增补丁：

```text
CardCmd.Exhaust
→ CardPileCmd.Add(card, PileType.Exhaust)
→ CombatManager.History.CardExhausted
→ Hook.AfterCardExhausted   ← CanAoCombatRules 在此接收
```

`CanAoCombatRules` 通过 `ModHelper.SubscribeForCombatStateHooks` 注册，
重写 `AbstractModel.AfterCardExhausted(PlayerChoiceContext, CardModel, bool)`
即可收到全部消耗路径（正常打出、其他牌效果、虚无、浴火结算）。

权威记录顺序：

```text
游戏完成 CardCmd.Exhaust
→ CanAoCombatRules.AfterCardExhausted
→ ExhaustService.RecordAndNotify
   ├─ 快照 HadYuHuo（YuHuoService.HasYuHuo）
   ├─ 快照 ResolvedByYuHuo（YuHuoService.IsResolving，
   │   浴火最终消耗仍在重入锁内，见 YuHuoResolver finally）
   ├─ SourceModel = 选择栈第二项（栈顶是 Hook 分发压入的本监听器）
   ├─ 写入按战斗、按玩家的回合历史
   └─ 通知 IAfterCanAoCardExhausted 监听者
```

回合清理由 `CanAoCombatRules.AfterSideTurnEndLate` 执行
（`ExhaustService.ClearForPlayers`），与临时浴火、星月回合计数同一时点，
保证普通 AfterSideTurnEnd 监听者仍能读到本回合消耗历史。

## 文件

```text
Rules/Exhaust/
├── ExhaustRecord.cs            记录 + CanAoExhaustCause 分类
├── IExhaustEvents.cs           IAfterCanAoCardExhausted
├── ExhaustCombatState.cs       按战斗、按玩家的回合记录列表
├── ExhaustListenerRegistry.cs  监听者快照（牌 → Power → 遗物）
└── ExhaustService.cs           唯一读写入口
```

所有未来"本回合消耗过…"的卡牌、Power、遗物必须读取
`ExhaustService`，不得自行使用静态变量或分散计数。

## R7 卡牌（均在 ColorlessCardPool 测试）

| 卡牌 | 费用 | 效果 | 验证点 |
| --- | --- | --- | --- |
| 征召 | 1 | 浴火。抽 3（4）张牌 | 浴火自动打出 + 抽牌异步链 |
| 浴火打击 | 2 | 浴火。造成 18（24）伤害。若因浴火触发，获得 1（2）月 | `YuHuoService.IsTriggeredByYuHuo` |
| 焚膏继晷 | 1 | 消耗不超过 1（2）张手牌。若至少消耗 1 张浴火牌，获得 1 星与 1 月。消耗 | 多选、`HadYuHuo` 快照、批次记录 |
| 清宫 | 1 | 消耗手牌中所有非浴火技能牌。每消耗 1 张，获得 5（8）格挡。消耗 | 筛选、批量消耗、按张结算 |
| 凤骨再燃 | 1 | 从消耗堆选择 1 张浴火牌加入手牌，它本回合费用 -1（-2）。消耗 | 消耗堆选牌、浴火过滤、临时费用 |

## 实机验收清单

1. 燃烧契约消耗浴火斩：自动打出 + 继续抽 2 张（浴火回归）。
2. 征召：正常打出抽 3；被燃烧契约消耗时自动打出抽 3，然后进入消耗堆。
3. 浴火打击：正常打出只造成 18 伤害、不给月；被消耗触发浴火时 18 伤害 + 1 月。
4. 焚膏继晷：选择 0 张可取消；消耗 1 张非浴火牌不给星月；消耗浴火牌（浴火触发后）获得 1 星 1 月。
5. 清宫：手牌含浴火技能牌时该牌不被消耗；每消耗 1 张获得 5 格挡；无合法目标时不获得格挡。
6. 凤骨再燃：消耗堆无浴火牌时不弹选择；选回浴火牌后费用 -1，打出或回合结束后恢复。
7. 回合结束后 `ExhaustService` 计数清零（焚膏继晷/清宫在下一回合重新计数）。
8. 日志无 `NullReferenceException`、`STARMOON_FAILED`、`YUHUO_RESOLVE_FAILED`、
   `YUHUO_FALLBACK_EXHAUST_FAILED`。

## 风险与备忘

- `SourceModel` 依赖 Hook 分发器把监听器压入选择栈这一实现细节（v0.109.0
  `Hook.AfterCardExhausted` 确认如此）。它只是信息性字段，R7 卡牌不依赖它；
  分类不准时退化为 `Unknown`，不影响计数。
- 浴火牌被消耗时先触发浴火再进消耗堆，因此 `HadYuHuo` 与 `ResolvedByYuHuo`
  恒为同时记录；`Cause` 分类里 Ethereal 优先于 YuHuoResolution，原始字段无损。
