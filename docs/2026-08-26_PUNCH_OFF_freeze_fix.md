# 2026-08-26 重拳出击（PUNCH_OFF）卡死修复记录

## 症状

玩家使用残傲角色触发原版事件「重拳出击」（PUNCH_OFF），选择「顺手牵羊」
（拿受伤诅咒牌 + 随机遗物）后游戏卡死：

- 画面正中心出现**受伤**卡牌；
- 卡面描述却是传令（CHUAN_LING_CARD）的「将 1 张【诏令】加入手牌。」；
- 事件不再推进，只能退回主菜单（软锁，非进程崩溃）。

## 日志定位

游戏日志目录：`%APPDATA%\SlayTheSpire2\logs\`（godot.log 为当前会话，
滚动文件为 `godot<启动时间>.log`）。

在 2026-08-26 15:52 会话的 godot.log 中发现两次完全相同的异常
（行 570 / 647，对应两次重试）：

```text
ERROR: System.NullReferenceException: Object reference not set to an instance of an object.
   at CanAoNative.Rules.YuHuo.YuHuoService.HasYuHuo(CardModel, ICombatState)
        in YuHuoService.cs:line 85
   at CanAoNative.Patches.YuHuoDescriptionPatch.Postfix(CardModel, String&)
        in YuHuoDescriptionPatch.cs:line 26
   at CardModel.GetDescriptionForPile_Patch1(...)
   at NCard.UpdateVisuals(...)
   at CardCmd.PreviewInternal(...)
   at CardPileCmd.AddCurseToDeck[T](Player)
   at PunchOff.Nab()
   at EventOption.Chosen() → NEventRoom.OptionButtonClicked(...)
```

## 根因

1. `PunchOff.Nab()` 通过 `CardPileCmd.AddCurseToDeck<Injury>()` 在**战斗外**
   把受伤加入牌组，并渲染卡牌预览。
2. mod 的 `YuHuoDescriptionPatch`（浴火关键词描述补丁）拦截所有卡牌描述
   渲染，经 `YuHuoDisplay.HasYuHuo` 调用 `YuHuoService.HasYuHuo`。
3. `YuHuoDisplay` 的 combatState 兜底 `card.Owner?.Creature?.CombatState`
   在战斗外**非 null**（生物残留上一场战斗的引用），通过了非空检查；
4. `YuHuoService.HasYuHuo` 随后访问 `owner.PlayerCombatState.TurnNumber`，
   而战斗外 `PlayerCombatState == null` → NRE。
5. 异常中断 `Nab()` 异步链：事件永不 `SetEventFinished` → 软锁；
   `NCard.UpdateVisuals` 在换描述文本前中断，卡面节点保留上一张渲染卡
   （传令）的旧文本 → 受伤卡面 + 传令描述。

## 修复

`src/CanAoNative/Rules/YuHuo/YuHuoService.cs` 的 `HasYuHuo` 在
`owner.PlayerCombatState == null`（非战斗）时提前返回 `false`：

- 永久/临时浴火均为战斗作用域状态，非战斗场景本就不该显示；
- 同时避免用生物残留战斗引用读到已结束战斗的记录；
- 战斗内行为完全不变（战斗中 PlayerCombatState 恒非 null）。

`scripts/Verify-R11.ps1` 冻结哈希同步更新（`Rules\YuHuo\YuHuoService.cs`
`0c5fbffa…` → `c4b58d17…`）并加注说明；Verify-R11 全量验证通过。

## 提交与发布

- `377cabd` chore: 存档工作区状态（修复前基线，136 个 WIP 文件）
- `6619bf9` fix: YuHuoService.HasYuHuo 非战斗 NRE 导致 PUNCH_OFF 事件卡死
- 构建 `dotnet build -c Release` 0 错误，DLL/PDB 部署至游戏
  `mods\CanAoNative\`（SHA-256 校验一致）；PCK 无改动。
- 推送至 GitHub 新仓库 `Liu-Yuanyi/STS2-CanAo`（公开，SSH over
  ssh.github.com:443 —— 大陆直连 22 端口传输大包会被重置）。

## 验证建议

重开游戏后再次触发 PUNCH_OFF → 顺手牵羊：应正常弹出受伤预览并给出
遗物奖励。若仍卡死，取新会话 godot.log 复查。
