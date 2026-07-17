# 2026-07-16 修复摘要

本工作区以较完整的 `CanAoNative` 源码快照为基准重建，故意删除了所有旧的 `bin`、`obj` 和 `build` 产物，避免再次把旧 DLL 与新源码混在一起。

## 已修复

- `CardCmd.Exhaust` Harmony Prefix 固定为精确重载。
- 跳过原方法时通过 `ref Task __result` 返回替代任务。
- 删除 `async Task<bool> Prefix` 和 fire-and-forget 浴火结算。
- Resolver 不再吞掉异常；失败时尝试恢复牌堆并重新抛出。
- 浴火状态改为按 `ICombatState` 的弱引用表，不再使用单槽全局缓存。
- 临时浴火按卡牌实例与玩家保存，并在玩家回合结束时清理。
- 新增浴火执行上下文，供“因浴火触发”类卡牌查询。
- 凤焰不息通过触发次数修改接口接入，不再硬编码进 Resolver。
- 星月合成迁移到统一 Power 变化 Hook。
- 星/月扣除改用 `PowerCmd.ModifyAmount`。
- 星月合成增加按玩家重入锁，并一次性计算配对数。
- 测试卡不再手动调用星月合成。
- 星月合击保留 `Ethereal + Exhaust`。
- manifest 增加 `min_game_version: 0.108.0`。
- 部署脚本复制 PDB，并比较构建、暂存、安装三份 DLL 哈希。
- 新增唯一运行标记 `CANAO_NATIVE_FIX_20260716_R1`。
- 验证脚本会阻止 BaseLib、手动注入和危险的异步 Prefix 回归。

## 尚未实现

- 完整角色、卡池、遗物、药水与正式美术。
- 临时凤威和永久凤威的完整拆分。
- “牺牲准备”选择 2/3 张手牌并授予临时浴火。
- 浴火自定义 Hover Tip 和图标。
- 多人网络序列化的专项验证。
