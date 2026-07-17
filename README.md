# CanAoNative — 杀戮尖塔2“残傲”角色 Mod

基于 STS2 原生 Mod Loader，零 Alchyr BaseLib 依赖。

当前版本：**R6（星月事件层）**。

R6 严格保留已经完成实机验证的 R5 浴火与凤威核心，并新增：

- 战斗作用域、按玩家保存的星月回合历史；
- 星月合击生成前、生成后、打出后的扩展事件；
- **盘旋**：5（7）格挡；本回合每生成一张星月合击，再获得 3（4）格挡；
- **星月伐魔**：10（14）伤害；若本回合生成过星月合击，获得 1 星和 1 月；
- **天凤军阵**：每生成一张星月合击，对所有敌人造成 6（9）伤害；
- 临时浴火、临时凤威和星月回合历史统一在完整回合结束后清理。

## 快速部署

```powershell
$env:STS2_GAME_DIR = "E:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2"

cd C:\Users\32880\RiderProjects\CanAoNative
Set-ExecutionPolicy -Scope Process Bypass -Force
.\scripts\Deploy-Mod.ps1 -Configuration Release
```

新日志必须包含：

```text
CANAO_NATIVE_R6_STARMOON_EVENTS_20260717
```

## R6 推荐测试

1. 回归燃烧契约消耗浴火牌，确认后续抽牌不受影响。
2. 打出盘旋，再生成一张星月合击，确认额外获得 3 点格挡。
3. 同回合再次生成星月合击，确认盘旋再次触发。
4. 升级盘旋，确认初始格挡为 7、每次生成获得 4 格挡。
5. 未生成星月合击时打出星月伐魔，只造成伤害。
6. 先生成星月合击，再打出星月伐魔，确认获得星和月并立即再生成一张星月合击。
7. 打出天凤军阵后生成星月合击，确认所有敌人受到 6 点伤害；升级后为 9。
8. 回合结束后，盘旋消失，下一回合星月伐魔的“本回合生成过”条件重置。

## 文档

- [R6 安装与验收](docs/INSTALL_R6.md)
- [R6 星月事件层设计](docs/R6_STARMOON_EVENTS.md)
- [R6 静态核验](docs/R6_STATIC_VERIFICATION.md)
