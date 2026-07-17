# R6：星月事件层与三张生产卡

## 目标

R5 已验证浴火与凤威。R6 不继续把“是否生成过星月合击”分散写进每张卡，而是建立统一、可扩展的星月事件和回合历史。

## 事件

### `IBeforeStarMoonGenerated`

具体 `StarMoonStrike` 已创建，但尚未加入手牌。适合未来修改去向、升级状态或记录生成来源；当前监听者不应自行移动该牌。

### `IAfterStarMoonGenerated`

卡牌已成功加入战斗，且 `GeneratedThisTurn` 已增加。盘旋和天凤军阵在这里触发。

### `IAfterStarMoonPlayed`

`StarMoonStrike` 已完成一次 `CardPlay`，并进入 `AfterCardPlayedLate`。Replay 和 AutoPlay 的每个实际 CardPlay 分别计数。

## 状态

`StarMoonCombatState` 按 `ICombatState` 和 `Player` 保存：

```text
GeneratedThisTurn
PlayedThisTurn
```

它不保存在卡牌或 Power 静态字段中，因此双玩家、额外回合和多场战斗不会共用计数。

## 盘旋

```text
1费，罕见技能
获得 5/7 格挡。
此后本回合每生成 1 张星月合击，获得 3/4 格挡。
```

实现：

- 卡牌先获得基础格挡；
- 应用 `PanXuanPower`，Amount 为 3/4；
- Power 实现 `IAfterStarMoonGenerated`；
- 多次打出按 Amount 相加；
- `AfterSideTurnEndLate` 归零。

## 星月伐魔

```text
1费，罕见攻击
造成 10/14 点伤害。
若本回合生成过星月合击，获得 1 星和 1 月。
```

条件在造成伤害前记录，之后使用 `PowerCmd.Apply` 获得星/月。正常星月规则会立刻将这 1 对资源转化为另一张星月合击。

## 天凤军阵

```text
2费，罕见能力
每当你生成星月合击时，对所有敌人造成 6/9 点伤害。
```

`TianFengJunZhenPower.Amount` 保存每次触发伤害。多张能力按数值相加。伤害使用 `ValueProp.Unpowered`，不会被力量当成攻击伤害增幅。

## 回合结束边界修复

R5 的 `CanAoCombatRules` 仍在 `BeforeSideTurnEnd` 清理临时浴火，可能使具有虚无的临时浴火牌在自动消耗前丢失浴火。

R6 改为 `AfterSideTurnEndLate`：

- 虚无先完成消耗；
- 普通 AfterSideTurnEnd 监听者仍可读取回合历史；
- 最后再清除临时浴火和星月计数。

`TemporaryFengWeiPower` 也移到同一晚期阶段归零。
