# R3 静态核验记录

日期：2026-07-17  
工作区来源：`CanAoNative_fixed_20260716_R2.zip`  
目标游戏基线：STS2 v0.108.0  
反编译程序集 MVID：`F7D2A9E0-F1AE-4213-B874-1504473AAEDB`

## 已核对的游戏 API

- `CardSelectCmd.FromHand(PlayerChoiceContext, Player, CardSelectorPrefs, Func<CardModel, bool>?, AbstractModel)`
- `CardSelectorPrefs(LocString, int)` 与 `PretendCardsCanBePlayed`
- `CardModel.SelectionScreenPrompt`
- `CardModel.Played`
- `CardCmd.AutoPlay(...)`
- `CardCmd.Exhaust(PlayerChoiceContext, CardModel, bool, bool)`
- `ModHelper.SubscribeForCombatStateHooks(string, CombatHookSubscriptionDelegate)`
- `AbstractModel.ShouldReceiveCombatHooks`
- `Player.Relics` 与 `RelicStatus.Disabled`
- `PlayerCombatState.TurnNumber`

## 已执行的源码检查

- 全部 JSON 可解析。
- 新卡牌、本地化键、manifest 版本和构建标记齐全。
- 不存在 `ModelDb.Inject`、`InjectModels`、`TaskHelper.RunSafely`、`async void`、`.Wait()` 或 `.Result`。
- 浴火 Prefix 仍通过 `ref Task __result` 返回替代任务。
- `YuHuoResolver` 没有硬编码 `FengYanBuXiPower`。
- 临时浴火使用 `ReferenceEqualityComparer.Instance` 区分卡牌实例。
- 回合结束清理由 `CanAoCombatRules.BeforeSideTurnEnd` 主动执行。
- 浴火事件监听者按“卡牌、Power、遗物”建立快照并顺序等待。

## 无法在当前环境完成的项目

当前执行环境没有用户本机的 .NET SDK、游戏运行时与 Steam/Godot，因此没有声称完成：

- `dotnet build`；
- PCK 实际打包运行；
- 游戏内选择界面测试；
- 多人同步测试；
- 战斗中途存档/读取测试。

这些项目由 `docs/INSTALL_R3.md` 中的本机部署与回归流程验收。若本机构建失败，应以编译器首个错误和 v0.108.0 反编译签名为准，避免一次修改多个系统。
