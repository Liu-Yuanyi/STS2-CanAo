# CanAoNative — 杀戮尖塔2“残傲”角色 Mod

基于 STS2 原生 Mod Loader，零 Alchyr BaseLib 依赖。

当前版本：**R8（诏令系统）**。

R8 严格保留已经完成实机验证的 R5–R7 浴火、凤威、星月与消耗事件核心，并新增：

- 衍生牌【诏令】：保留，消耗。消耗 1 张手牌，按类型获得星/月；
- `EdictService`：诏令生成/打出回合历史唯一入口；
- `IAfterEdictPlayed` 扩展事件；
- **传令**：将 1 张诏令加入手牌。消耗（升级移除消耗）；
- **密诏**：将 1（2）张诏令加入手牌。消耗；
- **王权**：消耗手牌中所有诏令。每消耗 1 张，获得 1 点凤威并抽 1 张牌；
- **帝国余威**：每回合第二次打出诏令时，获得 1 点凤威（升级获得固有）；
- **承天受命**：获得 3 点凤威。将 2 张诏令加入手牌。消耗；
- **天凤形态**：获得 3（5）点凤威。每回合开始时，将 1 张诏令加入手牌。

## 快速部署

```powershell
$env:STS2_GAME_DIR = "E:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2"

cd C:\Users\32880\RiderProjects\CanAoNative
Set-ExecutionPolicy -Scope Process Bypass -Force
.\scripts\Deploy-Mod.ps1 -Configuration Release
```

新日志必须包含：

```text
CANAO_NATIVE_R8_EDICT_SYSTEM_20260717
```

## R8 推荐测试

1. 回归燃烧契约消耗浴火牌，确认后续抽牌不受影响。
2. 打出诏令，选攻击/技能/能力牌各一次，确认星/月正确且组合立即解析。
3. 传令升级后不再消耗；密诏升级后给 2 张诏令。
4. 王权消耗 N 张诏令 → N 点凤威 + 抽 N 张牌。
5. 帝国余威仅在每回合第二次打出诏令时给凤威。
6. 天凤形态每回合开始手牌 +1 诏令。
7. 回合结束后诏令计数清零。

## 文档

- [卡牌描述与悬浮规范](docs/CARD_TEXT_CONVENTIONS.md)
- [R8 诏令系统设计](docs/R8_EDICT_SYSTEM.md)
- [R7 消耗事件层设计](docs/R7_EXHAUST_EVENTS.md)
- [R6 星月事件层设计](docs/R6_STARMOON_EVENTS.md)
