# CanAoNative — 杀戮尖塔2“残傲”角色 Mod

基于 STS2 原生 Mod Loader，零 Alchyr BaseLib 依赖。

当前版本：**R9（遗物、药水与密诏调整）**。

R9 严格保留已经完成实机验证的 R5–R8 浴火、凤威、星月、消耗与诏令核心，并新增：

- **密诏调整**：将 1 张诏令加入弃牌堆。不消耗。升级后加入的是诏令+；
- **天凤军印**（普通遗物）：每次打出诏令后获得 4 格挡；
- **青鸾羽衣**（罕见遗物）：回合开始时，若上回合剩余 ≥5 格挡，获得 1 月；
- **合击武典**（罕见遗物）：每打出 4 张星月合击，下一张效果翻倍；
- **涅槃火种**（稀有遗物）：每场战斗第一次浴火结算时，额外触发 1 次；
- **战碑**（稀有遗物）：战斗开始获得 2 凤威；第一回合星月合击只造成伤害；
- **孤王玉座**（稀有遗物）：回合结束手牌为空，下回合获得 1 费和星月合击+；
- **星月王冠**（稀有遗物）：每回合第一次获得凤威时，获得 1 张星月合击；
- **帝国税契**（商店遗物）：战斗开始给 1 张诏令；每次打出诏令失去 1 金币；
- **琼浆**（普通药水）：获得 4 月；
- **凤威酒**（罕见药水）：获得 2 点凤威，在本回合获得 3 点凤威；
- **御令瓶**（稀有药水）：将 2 张诏令+加入手牌。

## 快速部署

```powershell
$env:STS2_GAME_DIR = "E:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2"

cd C:\Users\32880\RiderProjects\CanAoNative
Set-ExecutionPolicy -Scope Process Bypass -Force
.\scripts\Deploy-Mod.ps1 -Configuration Release
```

新日志必须包含：

```text
CANAO_NATIVE_R9_RELICS_POTIONS_20260717
```

## R9 推荐测试

1. 回归燃烧契约消耗浴火牌，确认后续抽牌不受影响。
2. 密诏未升级→弃牌堆得诏令；升级→弃牌堆得诏令+。
3. 涅槃火种只在每场战斗第一次浴火时额外触发，下一战斗重置。
4. 星月王冠只在每回合第一次获得凤威时给星月合击。
5. 凤威酒永久 +2、本回合 +3 凤威；御令瓶给 2 张诏令+。
6. 存档/读档后继续游戏无异常。

## 文档

- [卡牌描述与悬浮规范](docs/CARD_TEXT_CONVENTIONS.md)
- [R9 遗物、药水与密诏调整](docs/R9_RELICS_POTIONS.md)
- [R8 诏令系统设计](docs/R8_EDICT_SYSTEM.md)
- [R7 消耗事件层设计](docs/R7_EXHAUST_EVENTS.md)
- [R6 星月事件层设计](docs/R6_STARMOON_EVENTS.md)
