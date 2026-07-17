# R6 静态核验记录

## 已核对

- R5 日志构建标记正确，未出现浴火/初始化异常；
- R5 核心文件通过文本 SHA-256 锁定；
- `AbstractModel` v0.108.0 反编译签名包含：
  - `AfterCardPlayedLate(PlayerChoiceContext, CardPlay)`；
  - `AfterSideTurnEndLate(PlayerChoiceContext, CombatSide, IEnumerable<Creature>)`；
- `CreatureCmd` v0.108.0 反编译签名包含：
  - `GainBlock(Creature, decimal, ValueProp, CardPlay?, bool)`；
  - `Damage(PlayerChoiceContext, IEnumerable<Creature>, decimal, ValueProp, Creature)`；
- JSON 均为 UTF-8 且可解析；
- 未引入 BaseLib、`ModelDb.Inject`、fire-and-forget、`.Wait()` 或 `.Result`；
- 浴火 Task 返回契约文件未改变。

## 当前环境限制

当前生成环境没有 .NET SDK、STS2 游戏运行时和 Godot，无法执行 `dotnet build`、PCK 实际加载或战斗实测。

用户本机首次构建时，以编译器的第一个错误为准；首次运行时，以 R6 构建标记和完整日志为准。

本轮上传的 `sts2.zip` 实际只包含 v0.107.0 架构指南和 ILSpy 工程入口，不含可供逐文件核对的反编译源码。R6 新 API 签名依据此前同一会话中提供的 v0.108.0 反编译源码，以及已在 R5 编译通过的现有 API。
