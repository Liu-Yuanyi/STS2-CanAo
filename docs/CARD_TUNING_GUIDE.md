# 卡牌数值与描述调整指南

> 面向：想自行微调残傲卡牌数值或文本的人。
> 前提：改完任何 `.cs` 后跑 `.\scripts\Deploy-Mod.ps1 -Configuration Release` 即完成构建+部署，无需其他步骤。

## 1. 数值在哪里

每张卡是一个 `src/CanAoNative/Cards/XxxCard.cs`，数值集中在两处：

```csharp
protected override IEnumerable<DynamicVar> CanonicalVars =>
[
    new DamageVar(10m, ValueProp.Move),   // 基础伤害 10
    new BlockVar(5m, ValueProp.Move),     // 基础格挡 5
    new CardsVar(2)                       // 通用整数（抽牌/星月/次数等）
];

protected override void OnUpgrade()
{
    DynamicVars.Damage.UpgradeValueBy(4m); // 升级后伤害 +4 → 14
    DynamicVars.Block.UpgradeValueBy(3m);  // 升级后格挡 +3 → 8
    DynamicVars.Cards.UpgradeValueBy(1m);  // 升级后整数 +1 → 3
}
```

- **改数值**只需改 `CanonicalVars` 里的基础值和 `OnUpgrade` 里的增量。
  描述里的 `{Damage:diff()}`/`{Block:diff()}`/`{Cards:diff()}` 占位符
  **自动跟随变量并高亮升级差异，不用改文本**。
- 费用：构造函数 `canonicalEnergyCost: N`；升级改费用：
  `EnergyCost.UpgradeBy(-1)`（如王权 2→1）。
- 变量类型：`DamageVar`（伤害）、`BlockVar`（格挡）、`CardsVar`（通用整数）、
  `PowerVar<XxxPower>`（给 Power 的层数，占位符为类型名）、
  `EnergyVar`（能量，配 `{Energy:energyIcons()}` 图标）。
- 不要给固定数值新建变量：数值只出现一次且不升级时，直接写进文本即可
  （如"获得 1 星"），保持简单。

## 2. 描述在哪里

`godot/CanAoNative/localization/zhs/cards.json` 和 `eng/cards.json`，
每个卡 ID 两个 key：

```json
"QING_LUAN_GOU_FA_CARD.title": "青鸾勾法",
"QING_LUAN_GOU_FA_CARD.description": "造成{Damage:diff()}点伤害。获得1点[gold]星[/gold]。",
```

- ID 由类名派生：`QingLuanGouFaCard` → `QING_LUAN_GOU_FA_CARD`。
- 规则全文见 `docs/CARD_TEXT_CONVENTIONS.md`，要点：
  - `[gold]…[/gold]` 给关键概念加黄（星/月/凤威/星月合击/浴火/手牌/格挡等）；
  - 消耗/虚无等关键词**不要写进描述**，由 `CanonicalKeywords` 自动生成；
  - 浴火牌**不要写"浴火"**，补丁自动在描述前加金色"浴火。"行；
  - 中文句号，双语必须同步改。
- 选牌提示另加 `XXX.selectionScreenPrompt` key。
- Power 文本在 `powers.json`、遗物在 `relics.json`（含 `.flavor`）、
  药水在 `potions.json`、悬浮词条在 `static_hover_tips.json`。

## 3. 悬浮提示配对

文本里提到的概念要有悬浮（规范第 4 节）：

```csharp
protected override IEnumerable<IHoverTip> ExtraHoverTips =>
[
    HoverTipFactory.FromPower<StarPower>(),          // 提到星
    HoverTipFactory.FromCard<StarMoonStrike>(),      // 提到星月合击
    CanAoHoverTips.YuHuo                             // 提到浴火（非浴火牌）
];
```

浴火牌自身不需要写——补丁自动追加浴火提示。

## 4. 改完之后

1. `.\scripts\Deploy-Mod.ps1 -Configuration Release`（会自动跑全部校验）；
2. 进游戏确认日志含当前构建标记；
3. 测试牌面数值、升级数值（铁匠铺升级或 DevConsole `upgrade`）、
   悬浮提示与加黄是否齐全；
4. `git commit` 留底。

注意：Verify 脚本冻结了已验证文件的哈希。改动这些文件属于正常演进，
部署时若报哈希不匹配，重算该文件的规范化 SHA-256 并更新
`scripts/Verify-R10.ps1` 中对应行（重算方法见脚本头部注释或用
`perl -pe 's/\r\n/\n/g; s/\r/\n/g' 文件 | sha256sum`）。
