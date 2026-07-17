# CanAoNative — 杀戮尖塔2“残傲”角色 Mod

基于 STS2 原生 Mod Loader，零 Alchyr BaseLib 依赖。

当前版本：**R7（统一消耗事件层）**。

R7 严格保留已经完成实机验证的 R5/R6 浴火、凤威与星月核心，并新增：

- 战斗作用域、按玩家保存的消耗回合历史；
- `ExhaustService`：消耗事实（类型、浴火快照、虚无、浴火结算、来源）唯一入口；
- `IAfterCanAoCardExhausted` 扩展事件；
- **征召**：浴火。抽 3（4）张牌；
- **浴火打击**：浴火。造成 18（24）伤害。若因浴火触发，获得 1（2）月；
- **焚膏继晷**：消耗不超过 1（2）张手牌。若至少消耗 1 张浴火牌，获得 1 星与 1 月。消耗；
- **清宫**：消耗手牌中所有非浴火技能牌。每消耗 1 张，获得 5（8）格挡。消耗；
- **凤骨再燃**：从消耗堆选择 1 张浴火牌加入手牌，它本回合费用 -1（-2）。消耗。

## 快速部署

```powershell
$env:STS2_GAME_DIR = "E:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2"

cd C:\Users\32880\RiderProjects\CanAoNative
Set-ExecutionPolicy -Scope Process Bypass -Force
.\scripts\Deploy-Mod.ps1 -Configuration Release
```

新日志必须包含：

```text
CANAO_NATIVE_R7_EXHAUST_EVENTS_20260717
```

## R7 推荐测试

1. 回归燃烧契约消耗浴火牌，确认后续抽牌不受影响。
2. 征召被其他牌消耗时自动打出并抽 3 张。
3. 浴火打击仅在被消耗触发浴火时给予 1 月。
4. 焚膏继晷消耗浴火牌后获得 1 星与 1 月；消耗非浴火牌不获得。
5. 清宫不消耗浴火技能牌，每消耗 1 张获得 5 格挡。
6. 凤骨再燃只能选中消耗堆中的浴火牌，拿回后本回合费用 -1。
7. 回合结束后消耗计数清零。

## 文档

- [R7 消耗事件层设计](docs/R7_EXHAUST_EVENTS.md)
- [R6 星月事件层设计](docs/R6_STARMOON_EVENTS.md)
