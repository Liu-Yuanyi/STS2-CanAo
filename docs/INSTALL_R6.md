# R6 安装与验收

## 1. 备份 R5

```powershell
$Old = "C:\Users\32880\RiderProjects\CanAoNative"
$Stamp = Get-Date -Format "yyyyMMdd-HHmmss"
$Backup = "C:\Users\32880\RiderProjects\CanAoNative_R5_backup_$Stamp"

if (Test-Path $Old) {
    Rename-Item $Old $Backup
    Write-Host ("R5 backup: {0}" -f $Backup)
}
```

完整解压 R6，最终路径应为：

```text
C:\Users\32880\RiderProjects\CanAoNative\README.md
C:\Users\32880\RiderProjects\CanAoNative\src\CanAoNative\CanAoNative.csproj
```

## 2. 部署

```powershell
cd C:\Users\32880\RiderProjects\CanAoNative

$env:STS2_GAME_DIR = `
    "E:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2"

Set-ExecutionPolicy -Scope Process Bypass -Force
.\scripts\Deploy-Mod.ps1 -Configuration Release
```

正确输出应包含：

```text
Verified R5 gameplay-core hashes passed.
R6 Star-Moon event, UTF-8, manifest and localization verification passed.
Deployment verified.
```

## 3. 运行标记

日志必须包含：

```text
CANAO_NATIVE_R6_STARMOON_EVENTS_20260717
```

不得包含：

```text
NullReferenceException
STARMOON_FAILED
YUHUO_RESOLVE_FAILED
YUHUO_FALLBACK_EXHAUST_FAILED
Exception thrown when calling mod initializer
```

## 4. 测试顺序

### A. R5 回归

- 燃烧契约消耗浴火牌后仍抽牌；
- 牺牲准备和凤焰不息正常；
- 示威、暂避锋芒的凤威数值和回合清除正常。

### B. 盘旋

1. 打出盘旋：立即获得 5 格挡。
2. 生成一张星月合击：再获得 3 格挡。
3. 同回合再生成一张：再获得 3 格挡。
4. 下一回合生成星月合击：不再获得盘旋格挡。
5. 升级版应为 7 与 4。

### C. 星月伐魔

1. 本回合尚未生成星月合击时打出：只造成 10/14 伤害。
2. 先生成星月合击，再打出：获得 1 星和 1 月，并立即生成一张新的星月合击。
3. 下一回合未生成时，条件应重新为否。

### D. 天凤军阵

1. 打出能力后生成一张星月合击：所有敌人受到 6 点伤害。
2. 升级版为 9 点。
3. 两层/两张能力的伤害应相加。
4. 一次同时生成两张星月合击，应触发两次。

### E. 回合结束边界

用牺牲准备给一张带虚无的牌临时浴火，保留至回合结束。虚无消耗时仍应触发浴火；下一回合临时浴火消失。
