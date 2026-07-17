# 安装 CanAoNative R3 工作区

## 1. 备份旧工作区

关闭 Rider、游戏和 Godot，然后在 PowerShell 中执行：

```powershell
$Old = "C:\Users\32880\RiderProjects\CanAoNative"
$Stamp = Get-Date -Format "yyyyMMdd-HHmmss"
$Backup = "C:\Users\32880\RiderProjects\CanAoNative_R2_backup_$Stamp"

if (Test-Path $Old) {
    Rename-Item $Old $Backup
    Write-Host ("R2 backup: {0}" -f $Backup)
}
```

不要把 R3 直接解压到 R2 目录上覆盖。完整替换能避免已经删除或改名的源码残留。

## 2. 解压 R3

将下载的 ZIP 解压。最终必须是：

```text
C:\Users\32880\RiderProjects\CanAoNative\README.md
C:\Users\32880\RiderProjects\CanAoNative\src\CanAoNative\CanAoNative.csproj
C:\Users\32880\RiderProjects\CanAoNative\scripts\Deploy-Mod.ps1
```

避免形成：

```text
C:\Users\32880\RiderProjects\CanAoNative\CanAoNative\src\...
```

## 3. 部署

```powershell
cd C:\Users\32880\RiderProjects\CanAoNative

$env:STS2_GAME_DIR = `
    "E:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2"

Set-ExecutionPolicy -Scope Process Bypass -Force
.\scripts\Deploy-Mod.ps1 -Configuration Release
```

脚本会依次：

- 扫描 BaseLib 和危险异步补丁残留；
- 验证 R3 文件及本地化；
- 编译 DLL；
- 打包 PCK；
- 删除游戏目录中的旧 CanAoNative；
- 安装新文件；
- 比较构建、暂存和安装 DLL 的 SHA-256。

## 4. 运行验收

新日志必须包含：

```text
CANAO_NATIVE_R3_YUHUO_EVENTS_20260717
```

不应包含：

```text
YUHUO_RESOLVE_FAILED
YUHUO_FALLBACK_EXHAUST_FAILED
NullReferenceException
Exception thrown when calling mod initializer
```

## 5. 第一轮测试

第一次只启用 CanAoNative，按以下顺序测试：

1. 燃烧契约消耗浴火斩，确认继续抽 2 张牌；
2. 牺牲准备选择普通攻击牌，再用燃烧契约消耗；
3. 两张同名牌只选择一张，验证实例隔离；
4. 结束回合，验证临时浴火到期；
5. 打出凤焰不息，验证浴火额外触发一次。

测试结束后运行：

```powershell
.\scripts\Verify-Deployment.ps1 `
    -Configuration Release `
    -LogPath "你的最新godot日志完整路径"
```
