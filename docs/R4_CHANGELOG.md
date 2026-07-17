# R4 变更摘要

版本：`0.0.4`

构建标记：

```text
CANAO_NATIVE_R4_YUHUO_REAL_CARDS_20260717
```

## 新增

- `FeatherRanksCard`（羽列千军）
  - 固有浴火；
  - 正常打出攻击单体；
  - 因浴火触发时改为攻击所有敌人。

- `YuHuoBannerCard`（浴火军旗）
- `YuHuoBannerPower`
  - 通过 `IAfterYuHuoTrigger` 监听浴火；
  - 只在牌效果实际执行后生效。

- `YuHuoBannerTemporaryStrengthPower`
  - 继承游戏原生 `TemporaryStrengthPower`；
  - 自动添加并在回合结束移除力量。

## 固化修复

- PowerShell JSON 和源码读取显式使用 UTF-8。
- 移除牺牲准备中导致 `List<object>` 的 `Distinct` 推断。
- 清理部分 Owner 空引用警告。
- manifest 与程序集版本更新为 0.0.4。

## 未包含

- 永久/临时凤威拆分；
- 残傲专属卡池；
- 自定义角色；
- Power 图标 atlas。
