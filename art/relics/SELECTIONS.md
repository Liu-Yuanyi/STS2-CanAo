# 遗物图标选定记录（2026-08-09/10 用户拍板）

来源批次：batch71（A/B 双版），拼版 `art/relics/relic_sheet_v01.png`，母版 `art/relics/raw/`。

| 遗物 | 选定 | 备注 |
|---|---|---|
| 帝国年表 | **史册A 的图**（relic_帝国史册_A_v01.png） | 与史册互换；**已实装** |
| 帝国史册 | **年表A 的图**（relic_帝国年表_A_v01.png） | 与年表互换；**已实装** |
| 天凤军印 | A | relic_天凤军印_A_v01.png；**已实装** |
| 青鸾羽衣 | B | relic_青鸾羽衣_B_v01.png；**已实装** |
| 合击武典 | B | relic_合击武典_B_v01.png；**已实装** |
| 涅槃火种 | A | relic_涅槃火种_A_v01.png；**已实装** |
| 战碑 | **art/manual/战碑.png（用户自制）** | batch73 v02 弃用；**已实装** |
| 孤王玉座 | **art/manual/孤王玉座.png（用户自制）** | batch73 v02 弃用；**已实装** |
| 帝国税契 | B | relic_帝国税契_B_v01.png；**已实装** |

## 实装记录（2026-08-10）

- `scripts/finalize_icons.py`：裁方 1024 母版（`art/relics/processed/`）→
  `godot/images/relics/`（256）+ `relics/small/`（85）+ `relics/outline/`（85 白色轮廓）；
- 代码：新增 `Patches/CanAoRelicIconPatch.cs`（小图/轮廓 getter 重定向到独立 PNG，
  大图走原生 `relics/<id>.png` 约定自动命中），`ModEntry` 已注册；
  未动 7 个哈希冻结遗物类；
- 已 `Godot --import` + `Deploy-Mod.ps1`，pck 核验 87 图（含 Power）.import/ctex 齐全。
