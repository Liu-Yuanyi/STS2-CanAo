# R5 变更摘要：基于已验证 R4

## 与之前重建版 R5 的关键区别

本版本不再从 R3 推测重建 R4。它直接复制用户上传并完成实机验证的 R4 工作区，然后只施加 R5 增量。

以下 R4 代码保持原始内容并由构建前哈希检查保护：

- 羽列千军；
- 牺牲准备；
- 星月合击；
- 浴火军旗及其 Power；
- `CanAoCombatRules`；
- 浴火状态、服务、Resolver 与 Exhaust Patch。

## R5 新增

- `TemporaryFengWeiPower`；
- `FengWeiService`；
- `ShiWeiCard`；
- `ZanBiFengMangCard`；
- 非阻断式 `CanAoPowerIconPatch`；
- R5 manifest、部署标记、验证脚本与文档。

## R5 有意没有做的改写

- 没有把星月合击修正从 Power Hook 搬到 `CanAoCombatRules`；
- 没有改写 R4 的浴火临时状态清理时机；
- 没有改写羽列千军、浴火军旗或牺牲准备；
- 没有使用尚未在 R4 工作区验证的 `PowerVar<TemporaryFengWeiPower>`，验证卡使用已知可编译的 `CardsVar`。

这些选择优先保证“R4 已验证行为不回退”。后续架构调整必须单独立项并实机回归。
