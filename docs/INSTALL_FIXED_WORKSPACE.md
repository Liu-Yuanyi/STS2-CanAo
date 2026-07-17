# 安装这份修复工作区

## 1. 备份旧工程

假设旧工程位于：

```text
C:\Users\32880\RiderProjects\CanAoNative
```

关闭 Rider、游戏和 Godot，然后在 PowerShell 中执行：

```powershell
Rename-Item `
  "C:\Users\32880\RiderProjects\CanAoNative" `
  "CanAoNative_backup_20260716"
```

## 2. 解压新工作区

把压缩包中的 `CanAoNative` 文件夹解压到：

```text
C:\Users\32880\RiderProjects\CanAoNative
```

确认下面文件存在：

```text
C:\Users\32880\RiderProjects\CanAoNative\
  src\CanAoNative\CanAoNative.csproj
  scripts\Deploy-Mod.ps1
  packaging\CanAoNative.json
```

不要把目录解压成双层：

```text
CanAoNative\CanAoNative\src
```

## 3. 设置游戏目录

当前 PowerShell 会话执行：

```powershell
$env:STS2_GAME_DIR =
  "E:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2"
```

## 4. 部署

```powershell
cd C:\Users\32880\RiderProjects\CanAoNative

Set-ExecutionPolicy -Scope Process Bypass

.\scripts\Deploy-Mod.ps1 -Configuration Release
```

脚本必须显示：

```text
Built SHA256 == Staged SHA256 == Installed SHA256
Deployment verified.
Expected runtime log marker:
CANAO_NATIVE_FIX_20260716_R1
```

## 5. 第一次测试

首次测试只启用 CanAoNative。暂时移走或关闭：

```text
SpeedX
Rewind
DamageMeter
BetterSpire2
RemoveMultiplayerPlayerLimit
```

并删除这个无效文件：

```text
...\mods\RemoveMultiplayerPlayerLimit\mod_manifest.json
```

测试顺序：

1. Burning Pact 消耗普通牌；
2. Burning Pact 消耗浴火斩；
3. 直接打出浴火斩；
4. 添加凤焰不息后重复测试；
5. 星和月各获得一次，确认只生成一张星月合击；
6. 打出星月合击，确认虚无与消耗生效。

## 6. 验证 DLL 与日志

退出游戏后执行：

```powershell
.\scripts\Verify-Deployment.ps1 `
  -GameDir $env:STS2_GAME_DIR `
  -Configuration Release `
  -LogPath "你的最新 godot 日志完整路径"
```

必须显示：

```text
Deployment verification passed.
Runtime log marker and failure scan passed.
```
