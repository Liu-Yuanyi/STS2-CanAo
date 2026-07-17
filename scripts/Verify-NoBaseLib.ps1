$ErrorActionPreference = "Stop"

$Root = Split-Path $PSScriptRoot -Parent

$ForbiddenPatterns = @(
    "using\s+Alchyr",
    "Alchyr\.Sts2\.BaseLib",
    "Alchyr\.Sts2\.ModAnalyzers",
    "CustomCardModel",
    "CustomPowerModel",
    "CustomRelicModel",
    "CustomPotionModel",
    "PlaceholderCharacterModel",
    "ModelDb\.Inject",
    "InjectModels",
    "async\s+Task<bool>\s+Prefix",
    "TaskHelper\.RunSafely\s*\(\s*YuHuo"
)

$ScanDirs = @(
    (Join-Path $Root "src"),
    (Join-Path $Root "packaging")
)

$Files = $ScanDirs |
    Where-Object { Test-Path $_ } |
    ForEach-Object { Get-ChildItem $_ -Recurse -File } |
    Where-Object {
        $_.Extension -in @(
            ".cs",
            ".csproj",
            ".props",
            ".targets",
            ".json"
        )
    }

$Matches = foreach ($Pattern in $ForbiddenPatterns) {
    $Files | Select-String -Pattern $Pattern
}

if ($Matches) {
    $Matches | Format-Table Path, LineNumber, Line -AutoSize
    throw "Forbidden dependency, injection or unsafe async patch marker found."
}

Write-Host "Source safety scan passed."
