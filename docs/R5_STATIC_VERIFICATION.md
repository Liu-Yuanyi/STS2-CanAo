# R5 静态验证

本工作区已完成以下离线检查：

- 所有 JSON 使用严格 UTF-8 解析成功；
- R4 关键源文件与上传基线的规范化文本 SHA-256 一致；
- R5 新增文件存在且命名唯一；
- manifest 与 csproj 版本均为 0.0.5；
- 未出现 BaseLib、`ModelDb.Inject`、fire-and-forget、`async void`、`.Wait()` 或 `.Result`；
- 浴火 `ref Task __result` 合约仍存在；
- R5 验证卡仅使用 R4 已实际编译过的 `DamageVar`、`BlockVar`、`CardsVar` 和 Command API；
- 图标补丁在目标 getter 不存在时会安全跳过。

当前生成环境没有 .NET SDK、游戏程序集和 Godot，因此不能声称完成本机编译或游戏内实测。最终验收以用户本机的首个编译错误和运行日志为准。
