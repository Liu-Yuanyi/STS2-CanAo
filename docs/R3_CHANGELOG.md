# R2 → R3 变更摘要

## 新增内容

- `SacrificialPreparationCard`：牺牲准备，选择 2/3 张非能力牌，本回合获得浴火，消耗。
- `FengYanBuXiCard`：凤焰不息，3(2)费稀有能力牌。
- `YuHuoResolutionContext`：整次浴火结算上下文。
- 四个浴火生命周期事件接口。
- `YuHuoListenerRegistry`：按卡牌、Power、遗物顺序收集监听者。
- `Verify-R3.ps1`：R3 源码、manifest 和本地化检查。

## 修改内容

- 浴火每次触发建立 `YuHuoExecutionContext`，并记录是否真正完成出牌。
- `YuHuoResolver` 调度可扩展事件，不再认识具体 Power 或遗物。
- 触发次数修改器支持卡牌、Power 和遗物。
- 临时浴火增加统一的 `GrantUntilTurnEnd` 入口。
- manifest 更新为 `0.0.3`，构建标记更新为 R3。
- 部署脚本在编译前自动执行 R3 验证。
- `CLAUDE.md`、README、中期总结与安装文档同步到 R3。

## 保持不变的安全主链

R2 已验证的关键修复没有回退：

```text
Harmony Prefix 命中浴火
→ 把 YuHuoResolver Task 赋给 ref Task __result
→ 调用者继续 await
→ 自动打出和最终消耗完成
→ 外层卡牌继续剩余效果
```

没有重新引入 fire-and-forget、`Task.Yield()`、`.Wait()`、`.Result` 或全局单一重入布尔值。
