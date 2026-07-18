# CanAoNative — 杀戮尖塔2“残傲”角色 Mod

基于 STS2 原生 Mod Loader，零 Alchyr BaseLib 依赖。

当前版本：**R10（正式角色与专属卡池）**。

残傲已作为可选角色登场：

- 角色选择界面可选（视觉暂为铁甲战士占位）；
- 专属卡池（26 张卡，含打击/防御/凤羽残火/祭火基础卡）、
  专属遗物池（含起始遗物帝国年表）、专属药水池；
- 起始卡组：3 打击 + 3 防御 + 凤羽残火 + 祭火；
- 起始遗物帝国年表：每场战斗首次打出攻击/技能牌各获得 2 星/月；
- Orobas 升级映射：帝国年表 → 帝国史册；
- 探针卡已从玩家可见卡池移除。

## 快速部署

```powershell
$env:STS2_GAME_DIR = "E:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2"

cd C:\Users\32880\RiderProjects\CanAoNative
Set-ExecutionPolicy -Scope Process Bypass -Force
.\scripts\Deploy-Mod.ps1 -Configuration Release
```

新日志必须包含：

```text
CANAO_NATIVE_R10_CHARACTER_20260717
```

## R10 推荐测试

1. 角色界面选中残傲开局，确认 8 张起始手牌与帝国年表。
2. 首次打出攻击/技能牌各获得 2 星/月并合成星月合击。
3. 牌框橙色、能量图标正常，无资源缺失报错。
4. 存档/读档正常。

## 文档

- [卡牌描述与悬浮规范](docs/CARD_TEXT_CONVENTIONS.md)
- [R10 正式角色与专属卡池](docs/R10_CHARACTER.md)
- [R9 遗物、药水与密诏调整](docs/R9_RELICS_POTIONS.md)
- [R8 诏令系统设计](docs/R8_EDICT_SYSTEM.md)
- [R7 消耗事件层设计](docs/R7_EXHAUST_EVENTS.md)
- [R6 星月事件层设计](docs/R6_STARMOON_EVENTS.md)
