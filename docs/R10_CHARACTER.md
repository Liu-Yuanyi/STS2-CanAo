# R10：正式角色与专属卡池

> 构建标记：`CANAO_NATIVE_R10_CHARACTER_20260717`
> 游戏基线：STS2 v0.109.0（角色 API 已对照 v0.109.0 反编译源码核对）

## 交付内容

- **`CanAo : CharacterModel`**：女性，72 生命，99 金币；起始卡组
  3 打击 + 3 防御 + 凤羽残火 + 祭火；起始遗物帝国年表。
- **专属三池**：`CanAoCardPool`（橙框，全部 26 张正式卡）、
  `CanAoRelicPool`（8 遗物 + 2 起始遗物）、`CanAoPotionPool`（3 药水）。
  全部内容从 ColorlessCardPool/SharedRelicPool/SharedPotionPool 迁出；
  探针卡不再注册进任何卡池（仍可由 DevConsole 调出）。
- **`CanAoAllCharactersPatch`**：`ModelDb.AllCharacters` 硬编码数组
  经 Harmony Postfix 追加残傲；`UnlockState.Characters` 从该列表派生，
  角色默认解锁。
- **`TouchOfOrobasUpgradePatch`**：帝国年表 → 帝国史册 的 Orobas
  升级映射（否则回退 Circlet）。
- **起始遗物**：帝国年表（每场战斗首次打出攻击/技能牌各给 2 星/月）、
  帝国史册（每回合首次各给 1 星/月）。
- **基础卡**：打击、防御（Basic）、凤羽残火（浴火）、祭火。

## 视觉资源（占位方案）

缺资源即崩溃，故当前全部复用铁甲战士素材，待专属美术替换：

- 场景直接复制进 PCK：战斗形象、能量计数器、选择界面背景、
  商店/篝火形象、卡牌尾迹、转场材质（`godot/scenes|materials/...`）。
- 顶栏图标与多人手势 PNG：以 `.png.import` 重映射到铁甲战士的
  `.ctex`（`godot/images/...`），无需导出真实 PNG。
- `IconPath`/`CharacterSelectIconPath`/`MapMarkerPath` 用可覆盖的
  virtual 直接指向铁甲战士路径。

## 验收清单

1. 角色选择界面出现残傲（图标为铁甲占位），可选中并开始游戏。
2. 起始手牌 8 张符合设计，帝国年表在遗物栏；战斗首次打出攻击/技能牌
   各获得 2 星/月并正常合成星月合击。
3. 牌框为橙色，能量图标正常显示，无资源缺失崩溃。
4. 日志含 `CANAO_NATIVE_R10_CHARACTER_20260717`，无
   `AssetLoadException`、`NullReferenceException`。
5. 存档/读档后继续正常；Orobas 之触给出帝国史册。

## 已知限制

- 卡图、角色立绘、专属图标未做（全占位）；
- 图鉴过滤按钮为硬编码，残傲卡只在"全部"视图（自有过滤页需 UI 补丁，
  列入待办）；
- 多人需双方同版本 Mod（游戏自带 ModelDb 哈希校验）。
