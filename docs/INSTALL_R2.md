# R2 工作区替换与部署

1. 关闭《杀戮尖塔 2》和 Rider。
2. 将现有目录改名备份，例如：
   `C:\Users\32880\RiderProjects\CanAoNative_backup_R1`
3. 解压本压缩包，把其中的 `CanAoNative` 文件夹放到：
   `C:\Users\32880\RiderProjects\CanAoNative`
4. 在 PowerShell 中执行：

```powershell
cd C:\Users\32880\RiderProjects\CanAoNative
.\scripts\Deploy-Mod.ps1 -Configuration Release
```

5. 启动游戏后确认日志中出现：

```text
CANAO_NATIVE_FIX_20260716_R2
```

若 `STS2_GAME_DIR` 尚未设置，可在当前 PowerShell 会话中先执行：

```powershell
$env:STS2_GAME_DIR =
    "E:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2"
```

不要把新文件逐个覆盖到旧目录。使用完整目录替换可以避免旧源码、旧 `obj`
或旧构建产物残留。
