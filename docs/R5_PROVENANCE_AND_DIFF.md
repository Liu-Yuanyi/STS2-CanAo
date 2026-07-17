# R5 源码基线与差异说明

## 输入文件

已验证 R4：

```text
CanAoNative_R4_20260717.zip
SHA-256: 5AB56C240C6302F3BD7ECA48DDA61A52C7388A5843287DA6D25310F583843BA6
```

先前重建版 R5（仅用于提取设计意图，不作为源码基线）：

```text
CanAoNative_R5_reconstructed_20260717.zip
SHA-256: 99EE61C28F8898C9ED35B318CA3E723B1F4053A2CCF87EA59BA761AE4760706F
```

## 重建版曾额外改写的 R4 文件

逐文件比较显示，先前重建版修改过：

- `FeatherRanksCard.cs`；
- `SacrificialPreparationCard.cs`；
- `YuHuoBannerCard.cs`；
- `YuHuoBannerPower.cs`；
- `YuHuoBannerTemporaryStrengthPower.cs`；
- `CanAoCombatRules.cs`；
- `YuHuoCombatState.cs`；
- `YuHuoService.cs`；
- 多份 R4 脚本和文档。

真实基线版 R5 已撤销上述非必要改写。

## 真实 R5 的源码策略

1. 完整复制已验证 R4。
2. 对关键 R4 文件记录规范化文本 SHA-256。
3. 只新增凤威 R5 文件。
4. 只修改版本、注册表、测试卡、manifest、本地化和当前版本脚本。
5. Power 图标补丁使用反射定位 getter；目标不存在时跳过，而不是让初始化失败。
