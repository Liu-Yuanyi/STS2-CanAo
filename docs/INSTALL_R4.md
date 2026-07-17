# CanAoNative R4 安装与验收

## 1. 备份旧工作区

```powershell
$Old = "C:\Users\32880\RiderProjects\CanAoNative"
$Stamp = Get-Date -Format "yyyyMMdd-HHmmss"
$Backup = "C:\Users\32880\RiderProjects\CanAoNative_R3_backup_$Stamp"

if (Test-Path $Old) {
    Rename-Item $Old $Backup
}
```

解压 R4 后，确认：

```text
C:\Users\32880\RiderProjects\CanAoNative\README.md
C:\Users\32880\RiderProjects\CanAoNative\src\CanAoNative\CanAoNative.csproj
```

不要形成双层 `CanAoNative\CanAoNative\...`。

## 2. 部署

```powershell
cd C:\Users\32880\RiderProjects\CanAoNative

$env:STS2_GAME_DIR = `
    "E:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2"

Set-ExecutionPolicy -Scope Process Bypass -Force

.\scripts\Deploy-Mod.ps1 -Configuration Release
```

部署输出必须包含：

```text
R4 UTF-8 source, manifest and localization verification passed.
Deployment verified.
```

## 3. 日志标记

游戏日志必须包含：

```text
CANAO_NATIVE_R4_YUHUO_REAL_CARDS_20260717
```

不能只看到 R2/R3 标记。

## 4. 实机测试

### 羽列千军

```text
card FEATHER_RANKS_CARD
```

- 正常打出：只攻击所选敌人。
- 用燃烧契约消耗：改为攻击所有敌人。
- 凤焰不息存在时：因浴火触发两次，两次均为 AOE。

### 浴火军旗

```text
card YU_HUO_BANNER_CARD
```

- 打出能力后，用燃烧契约消耗羽列千军。
- 浴火效果成功执行后，本回合获得 2 力量。
- 回合结束时这 2 力量应被移除。
- 一层凤焰不息令浴火触发两次，因此获得 4 临时力量。
- 燃烧契约仍需继续抽牌。

## 5. 验证日志

```powershell
.\scripts\Find-FreshLog.ps1

.\scripts\Verify-Deployment.ps1 `
    -Configuration Release `
    -LogPath "最新日志完整路径"
```
