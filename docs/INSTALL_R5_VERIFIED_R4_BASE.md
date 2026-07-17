# R5 安装与验收

## 安装

先关闭游戏、Godot 和 Rider，备份旧目录：

```powershell
$Old = "C:\Users\32880\RiderProjects\CanAoNative"
$Stamp = Get-Date -Format "yyyyMMdd-HHmmss"
$Backup = "C:\Users\32880\RiderProjects\CanAoNative_R4_backup_$Stamp"

if (Test-Path $Old) {
    Rename-Item $Old $Backup
}
```

解压后确认：

```text
C:\Users\32880\RiderProjects\CanAoNative\README.md
C:\Users\32880\RiderProjects\CanAoNative\src\CanAoNative\CanAoNative.csproj
```

部署：

```powershell
cd C:\Users\32880\RiderProjects\CanAoNative
$env:STS2_GAME_DIR = "E:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2"
Set-ExecutionPolicy -Scope Process Bypass -Force
.\scripts\Deploy-Mod.ps1 -Configuration Release
```

## 构建验收

必须依次看到：

```text
Source safety scan passed.
Verified R4 baseline hashes passed.
R5 UTF-8, source, manifest and localization verification passed.
Deployment verified.
```

运行日志必须包含：

```text
CANAO_NATIVE_R5_VERIFIED_R4_BASE_20260717
```

图标补丁若成功，还会出现：

```text
CANAO_POWER_ICON_PATCH_APPLIED: PackedIconPath
CANAO_POWER_ICON_PATCH_APPLIED: ResolvedBigIconPath
```

如果当前游戏版本改名了 UI 属性，会显示 `SKIPPED`；这不应阻止 Mod 初始化。

## 实机测试

1. 完整回归 R4：燃烧契约、牺牲准备、羽列千军、浴火军旗、凤焰不息。
2. 打出获得凤威测试卡，确认永久凤威继续影响星月合击。
3. 打出示威，确认本回合凤威增加 2，升级后增加 3。
4. 打出暂避锋芒，确认本回合凤威减少 2。
5. 同回合依次打出示威与暂避锋芒，临时净值应为 0。
6. 永久凤威 1、临时凤威 2 时，星月合击应获得合计 +3 的伤害与格挡修正。
7. 回合结束后，临时凤威归零；永久凤威保留。
8. 测试负有效凤威，确认伤害与格挡的最终下限行为符合游戏原生规则。
9. 日志不得出现初始化异常、浴火失败或无限重复。
