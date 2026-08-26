# 原生 VFX 参考：铁甲战士 / 储君 全套（StS2 v0.109.0）

> 生成于 2026-08-16，数据源：反编译 `vfx_map.json` + pck 场景解析
> `vfx_scene_map.json`；纹理 PNG 已提取到 `../sts2_assets/vfx_refs/`。
> 卡牌触发方式分三类：
> - **命中**（`WithHitFx`）：伤害落点播放；
> - **出手**（`WithAttackerFx`）：攻击者身上播放；
> - **打出**（`OnEnqueuePlayVfx`/NVfx 场景）：松鼠标出牌瞬间播放。
> 引用格式：`vfx/...` 即 `res://scenes/vfx/... .tscn`；
> 我们自制的 can_ao 特效如需退回，按本表把卡的 WithHitFx 指到对应路径即可。
> 注意：Power/遗物在其 AfterApply/触发钩子里另起的持续性特效不在本表
> （如恶魔形态的回合火焰、地狱狂徒的后续地面火），查对应 Power 源码。

## 铁甲战士 Ironclad

| 卡牌 | 命中 VFX | 出手 VFX | 打出/其他 VFX |
|---|---|---|---|
| 好勇斗狠（Aggression） | — | — | — |
| 愤怒（Anger） | `vfx/vfx_attack_slash`（4 纹理） | — | — |
| 武装（Armaments） | — | — | — |
| 灰烬打击（AshenStrike） | `vfx/vfx_attack_slash`（4 纹理） | — | — |
| 壁垒（Barricade） | — | — | — |
| 痛击（Bash） | `vfx/vfx_attack_blunt`（1 纹理） | — | — |
| 战斗专注（BattleTrance） | — | — | — |
| 炽焰（Blaze） | — | — | — |
| 血墙（BloodWall） | — | — | `vfx/vfx_blood_wall`（3 纹理） |
| 放血（Bloodletting） | — | — | `vfx/vfx_bloody_impact`（1 纹理） |
| 重锤（Bludgeon） | `vfx/vfx_heavy_blunt`（1 纹理） | — | — |
| 全身撞击（BodySlam） | `vfx/vfx_attack_blunt`（1 纹理） | — | — |
| 烙印（Brand） | — | — | — |
| 破击（Break） | `vfx/vfx_heavy_blunt`（1 纹理） | — | — |
| 突破（Breakthrough） | `vfx/vfx_attack_blunt`（1 纹理） | — | — |
| 欺凌（Bully） | `vfx/vfx_attack_blunt`（1 纹理） | — | — |
| 燃烧契约（BurningPact） | — | — | — |
| 倾泻（Cascade） | — | — | — |
| 余烬（Cinder） | — | — | `vfx/vfx_fire_burst`（FireBurstVfx） |
| 巨像（Colossus） | — | — | — |
| 焚烧（Conflagration） | `vfx/vfx_attack_blunt`（1 纹理） | — | `vfx/fires/vfx_ground_fire`（GroundFireVfx）<br>资产预载：NGroundFireVfx.AssetPaths |
| 腐化（Corruption） | — | — | `vfx/vfx_power_up/vfx_power_up`（PowerUpVfx） |
| 绯红披风（CrimsonMantle） | — | — | `vfx/vfx_power_up/vfx_power_up`（PowerUpVfx） |
| 残酷（Cruelty） | — | — | — |
| 黑暗之拥（DarkEmbrace） | — | — | — |
| 防御（DefendIronclad） | — | — | — |
| 恶魔形态（DemonForm） | — | — | — |
| 恶魔护盾（DemonicShield） | — | — | `vfx/vfx_bloody_impact`（1 纹理） |
| 拆卸（Dismantle） | `vfx/vfx_attack_blunt`（1 纹理） | — | — |
| 主宰（Dominate） | — | — | — |
| 战鼓（DrumOfBattle） | — | — | — |
| 邪眼（EvilEye） | — | — | `vfx/vfx_gaze`（0 纹理） |
| 跃跃欲试（ExpectAFight） | — | — | — |
| 狂宴（Feed） | `vfx/vfx_bite`（0 纹理） | — | — |
| 无惧疼痛（FeelNoPain） | — | — | — |
| 恶魔之焰（FiendFire） | — | — | `vfx/fires/vfx_ground_fire`（GroundFireVfx）<br>资产预载：NGroundFireVfx.AssetPaths |
| 与我一战！（FightMe） | `vfx/vfx_attack_blunt`（1 纹理） | — | — |
| 火焰屏障（FlameBarrier） | — | — | `vfx/vfx_fire_burning`（FireBurningVfx） |
| 被遗忘的仪式（ForgottenRitual） | — | — | `vfx/fires/vfx_ground_fire`（GroundFireVfx）<br>资产预载：NGroundFireVfx.AssetPaths |
| 破灭（Havoc） | — | — | — |
| 头槌（Headbutt） | `vfx/vfx_attack_blunt`（1 纹理） | — | — |
| 地狱狂徒（Hellraiser） | — | — | `vfx/cards/vfx_hellraiser/hellraiser_vfx`（HellraiserVfx）<br>资产预载：NHellraiserVfx.AssetPaths |
| 御血术（Hemokinesis） | `vfx/vfx_bloody_impact`（1 纹理） | — | — |
| 彼岸咆哮（HowlFromBeyond） | `vfx/vfx_attack_blunt`（1 纹理） | — | — |
| 岿然不动（Impervious） | — | — | — |
| 地狱之刃（InfernalBlade） | — | — | — |
| 狱火（Inferno） | — | — | — |
| 燃烧（Inflame） | — | — | `vfx/fires/vfx_ground_fire`（GroundFireVfx）<br>`vfx/vfx_power_up/vfx_power_up`（PowerUpVfx）<br>资产预载：NGroundFireVfx.AssetPaths |
| 铁斩波（IronWave） | `vfx/vfx_flying_slash`（0 纹理） | — | — |
| 势不可当（Juggernaut） | — | — | — |
| 杂耍（Juggling） | — | — | — |
| 凌虐（Mangle） | `vfx/vfx_heavy_blunt`（1 纹理） | — | — |
| 午夜（Midnight） | `vfx/vfx_heavy_blunt`（1 纹理） | — | — |
| 熔融之拳（MoltenFist） | `vfx/vfx_molten_fist`（8 纹理） | — | — |
| 时候未到（NotYet） | — | — | — |
| 祭品（Offering） | — | — | — |
| 连环拳（OneTwoPunch） | — | — | — |
| 群情激愤（Outrage） | `vfx/vfx_attack_slash`（4 纹理） | — | — |
| 契约终结（PactsEnd） | `vfx/vfx_heavy_blunt`（1 纹理） | — | — |
| 完美打击（PerfectedStrike） | — | — | `vfx/vfx_big_slash_impact`（BigSlashImpactVfx）<br>`vfx/vfx_big_slash`（BigSlashVfx） |
| 劫掠（Pillage） | `vfx/vfx_attack_slash`（4 纹理） | — | — |
| 剑柄打击（PommelStrike） | `vfx/vfx_attack_blunt`（1 纹理） | — | — |
| 原始力量（PrimalForce） | — | — | — |
| 薪火之源（Pyre） | — | — | `vfx/vfx_fire_burning`（FireBurningVfx） |
| 狂怒（Rage） | — | — | — |
| 暴走（Rampage） | `vfx/vfx_attack_slash`（4 纹理） | — | — |
| 撕裂（Rupture） | — | — | — |
| 重振精神（SecondWind） | — | — | — |
| 预备打击（SetupStrike） | `vfx/vfx_attack_slash`（4 纹理） | — | — |
| 耸肩无视（ShrugItOff） | — | — | — |
| 怨恨（Spite） | `vfx/vfx_attack_slash`（4 纹理） | — | — |
| 惊逃（Stampede） | — | — | — |
| 添柴（Stoke） | — | — | — |
| 踩踏（Stomp） | `vfx/vfx_heavy_blunt`（1 纹理） | — | `vfx/spike_splash_vfx`（SpikeSplashVfx） |
| 岩石铠甲（StoneArmor） | — | — | — |
| 打击（StrikeIronclad） | `vfx/vfx_attack_slash`（4 纹理） | — | — |
| 飞剑回旋镖（SwordBoomerang） | `vfx/vfx_attack_slash`（4 纹理） | — | — |
| 肉盾（Tank） | — | — | — |
| 挑衅（Taunt） | — | — | — |
| 扯碎（TearAsunder） | `vfx/vfx_attack_slash`（4 纹理） | — | — |
| 痛殴（Thrash） | `vfx/vfx_thrash`（4 纹理） | — | — |
| 闪电霹雳（Thunderclap） | `vfx/vfx_attack_slash`（4 纹理） | — | — |
| 战栗（Tremble） | — | — | — |
| 坚毅（TrueGrit） | — | — | — |
| 双重打击（TwinStrike） | `vfx/vfx_attack_slash`（4 纹理） | — | — |
| 坚定不移（Unmovable） | — | — | `vfx/vfx_power_up/vfx_power_up`（PowerUpVfx） |
| 无情猛攻（Unrelenting） | `vfx/vfx_heavy_blunt`（1 纹理） | — | — |
| 上勾拳（Uppercut） | `vfx/vfx_heavy_blunt`（1 纹理） | — | — |
| 凶恶（Vicious） | — | — | — |
| 旋风斩（Whirlwind） | `vfx/vfx_giant_horizontal_slash`（1 纹理） | — | `vfx/whole_screen/horizontal_lines_vfx`（HorizontalLinesVfx）<br>（全屏烟雾，代码直构无场景）（SmokyVignetteVfx） |

## 储君 Regent

| 卡牌 | 命中 VFX | 出手 VFX | 打出/其他 VFX |
|---|---|---|---|
| 星位序列（Alignment） | — | — | — |
| 武器库（Arsenal） | — | — | — |
| 星界脉冲（AstralPulse） | `vfx/vfx_starry_impact`（0 纹理） | — | — |
| 锻打成型（BeatIntoShape） | `vfx/vfx_attack_blunt`（1 纹理） | — | — |
| 下去！（Begone） | — | — | — |
| 大爆炸（BigBang） | — | — | — |
| 黑洞（BlackHole） | — | — | — |
| 轰击（Bombardment） | `vfx/vfx_attack_blunt`（1 纹理） | — | `vfx/vfx_large_magic_missile`（LargeMagicMissileVfx） |
| 铸墙（Bulwark） | — | — | — |
| 新生之喜（BundleOfJoy） | — | — | — |
| 天穹之力（CelestialMight） | `vfx/vfx_starry_impact`（0 纹理） | — | — |
| 冲锋！！（Charge） | — | — | — |
| 群星之子（ChildOfTheStars） | — | — | — |
| 群星斗篷（CloakOfStars） | — | — | — |
| 碰撞轨迹（CollisionCourse） | `vfx/vfx_attack_blunt`（1 纹理） | — | — |
| 彗星（Comet） | — | — | `vfx/vfx_small_magic_missile`（SmallMagicMissileVfx） |
| 征服者（Conqueror） | — | — | — |
| 群星荟萃（Constellation） | — | — | — |
| 汇流（Convergence） | — | — | — |
| 宇宙冷漠（CosmicIndifference） | — | — | — |
| 迫降（CrashLanding） | `vfx/vfx_heavy_blunt`（1 纹理） | — | — |
| 新月长矛（CrescentSpear） | `vfx/vfx_starry_impact`（0 纹理） | — | — |
| 下砸（CrushUnder） | `vfx/vfx_heavy_blunt`（1 纹理） | — | `vfx/spike_splash_vfx`（SpikeSplashVfx） |
| 抉择，抉择（DecisionsDecisions） | — | — | — |
| 防御（DefendRegent） | — | — | — |
| 葬送（Devastate） | `vfx/vfx_attack_blunt`（1 纹理） | — | — |
| 星灭（DyingStar） | `vfx/vfx_starry_impact`（0 纹理） | — | — |
| 陨星（FallingStar） | `vfx/vfx_starry_impact`（0 纹理） | — | — |
| 既定事项（ForegoneConclusion） | — | — | — |
| 熔炉（Furnace） | — | — | — |
| 伽马爆破（GammaBlast） | `vfx/vfx_giant_horizontal_slash`（1 纹理） | — | — |
| 收集光辉（GatherLight） | — | — | — |
| 创世纪（Genesis） | — | — | — |
| 微光（Glimmer） | — | — | — |
| 流光溢彩（Glitterstream） | — | — | — |
| 辉光（Glow） | — | — | — |
| 护驾！！！（Guards） | — | — | — |
| 引导之星（GuidingStar） | — | — | `vfx/vfx_small_magic_missile`（SmallMagicMissileVfx） |
| 锤子时间（HammerTime） | — | — | — |
| 天际钻头（HeavenlyDrill） | `vfx/vfx_giant_horizontal_slash`（1 纹理） | — | — |
| 霸权（Hegemony） | `vfx/vfx_attack_slash`（4 纹理） | — | — |
| 传承之锤（HeirloomHammer） | `vfx/vfx_attack_blunt`（1 纹理） | — | — |
| 隐秘藏品（HiddenCache） | — | — | — |
| 所向无敌（IAmInvincible） | — | — | — |
| 王者之踢（KinglyKick） | `vfx/vfx_attack_slash`（4 纹理） | — | — |
| 王者之拳（KinglyPunch） | `vfx/vfx_attack_slash`（4 纹理） | — | — |
| 决胜一击（KnockoutBlow） | `vfx/vfx_attack_blunt`（1 纹理） | — | — |
| 何人僭越（KnowThyPlace） | — | — | — |
| 慷慨捐助（Largesse） | — | — | — |
| 月面射击（LunarBlast） | `vfx/vfx_attack_blunt`（1 纹理） | — | — |
| 如此甚好（MakeItSo） | `vfx/vfx_starry_impact`（0 纹理） | — | — |
| 君权自授（ManifestAuthority） | — | — | — |
| 流星雨（MeteorShower） | `vfx/vfx_attack_blunt`（1 纹理） | — | — |
| 王之凝视（MonarchsGaze） | — | — | — |
| 独白（Monologue） | — | — | — |
| 中子护盾（NeutronAegis） | — | — | — |
| 环绕轨道（Orbit） | — | — | — |
| 暗淡蓝点（PaleBlueDot） | — | — | — |
| 招架（Parry） | — | — | — |
| 粒子墙（ParticleWall） | — | — | — |
| 星星点点（Patter） | — | — | — |
| 光子切割（PhotonCut） | `vfx/vfx_attack_slash`（4 纹理） | — | — |
| 筹划（Plot） | — | — | — |
| 创世之柱（PillarOfCreation） | — | — | — |
| 预言（Prophesize） | — | — | — |
| 类星体（Quasar） | — | — | — |
| 辐射（Radiate） | `vfx/vfx_starry_impact`（0 纹理） | — | — |
| 淬炼刀刃（RefineBlade） | — | — | — |
| 倒映（Reflect） | — | — | — |
| 共鸣（Resonance） | — | — | — |
| 胜券在王（RoyalGamble） | — | — | — |
| 王国资产（Royalties） | — | — | — |
| 追踪之刃（SeekingEdge） | — | — | — |
| 七星（SevenStars） | `vfx/vfx_starry_impact`（0 纹理） | — | — |
| 明耀打击（ShiningStrike） | `vfx/vfx_starry_impact`（0 纹理） | — | — |
| 太阳打击（SolarStrike） | `vfx/vfx_attack_slash`（4 纹理） | — | — |
| 光谱偏移（SpectrumShift） | — | — | — |
| 战利品（SpoilsOfBattle） | — | — | — |
| 星尘（Stardust） | `vfx/vfx_starry_impact`（0 纹理） | — | — |
| 打击（StrikeRegent） | `vfx/vfx_attack_slash`（4 纹理） | — | — |
| 征召上前（SummonForth） | — | — | — |
| 超质量体（Supermassive） | `vfx/vfx_attack_slash`（4 纹理） | — | — |
| 剑圣（SwordSage） | — | — | — |
| 地形改造（Terraforming） | — | — | — |
| 封印王座（TheSealedThrone） | — | — | — |
| 铸剑者（TheSmith） | — | — | — |
| 指导（Tutor） | — | — | — |
| 暴政（Tyranny） | — | — | — |
| 崇拜（Venerate） | — | — | — |
| 虚空形态（VoidForm） | — | — | — |
| 战火铸就（WroughtInWar） | `vfx/vfx_attack_blunt`（1 纹理） | — | — |

## 附录：场景 → 纹理文件

### `scenes/vfx/big_slash/vfx_big_slash_impact_core.tscn`
- `vfx/big_slash/big_slash_impact_core_flipbook.png`

### `scenes/vfx/big_slash/vfx_big_slash_impact_side_smoke.tscn`
- `vfx/big_slash/big_slash_impact_smoke_flipbook.png`

### `scenes/vfx/common/vfx_common_blunt_impact_core.tscn`
- `vfx/common/common_blunt_impact_core.png`

### `scenes/vfx/common/vfx_common_glow.tscn`
- `vfx/common/common_glow.png`

### `scenes/vfx/common/vfx_common_hit_flare.tscn`
- `vfx/common/common_impact_flare_flipbook.png`

### `scenes/vfx/common/vfx_common_ring_polar_a.tscn`
- `vfx/common/common_ring_polar_a.png`

### `scenes/vfx/common/vfx_common_ring_polar_b.tscn`
- `vfx/common/common_ring_polar_b.png`

### `scenes/vfx/common/vfx_common_specks.tscn`
- `vfx/common/common_glow_speck.png`

### `scenes/vfx/common/vfx_poof.tscn`
- `vfx/common/common_poof.png`

### `scenes/vfx/common/vfx_smoke_flipbook.tscn`
- `vfx/starry_impact/starry_impact_smoke_flipbook.png`

### `scenes/vfx/distortions/vfx_outward_screen_distortion.tscn`
- `vfx/distortion/distortion_ring.png`

### `scenes/vfx/fire_impact/vfx_fire_burst_center_flipbook.tscn`
- `vfx/fire_impact/fire_burst_center_flipbook.png`

### `scenes/vfx/fire_impact/vfx_fire_burst_left_flipbook.tscn`
- `vfx/fire_impact/fire_burst_left_flipbook.png`

### `scenes/vfx/fire_impact/vfx_fire_burst_right_flipbook.tscn`
- `vfx/fire_impact/fire_burst_right_flipbook.png`

### `scenes/vfx/fire_impact/vfx_fire_flipbook.tscn`
- `vfx/fire_impact/fire_impact_flipbook.png`

### `scenes/vfx/fires/vfx_additive_step_fire.tscn`
- `vfx/fire/basic_fire_noise.png`
- `vfx/fire/cinder_particle.png`
- `vfx/fire/fire_base_pointy.png`
- `vfx/fire/fire_bottom_mask.png`
- `vfx/fire/triangle_noise_tile.png`
- `vfx/fire/zigzag_fire_distortion.png`

### `scenes/vfx/fires/vfx_ground_fire.tscn`
子场景: `vfx/fires/vfx_additive_step_fire.tscn`
- `vfx/environment/fire/fire_flame_sprites.png`
- `vfx/environment/fire/flame_gradient_mask.png`
- `vfx/shared_use/ash_particle.png`

### `scenes/vfx/heavy_blunt/components/vfx_heavy_blunt_anticipation_core.tscn`
- `vfx/vfx_heavy_blunt/heavy_blunt_anticipation_core.png`

### `scenes/vfx/heavy_blunt/components/vfx_heavy_blunt_hit_core.tscn`
- `vfx/vfx_heavy_blunt/heavy_blunt_hit_core_flat.png`

### `scenes/vfx/heavy_blunt/components/vfx_heavy_blunt_hit_core_glow.tscn`
- `vfx/vfx_heavy_blunt/heavy_blunt_hit_core_flat_glow.png`

### `scenes/vfx/heavy_blunt/components/vfx_heavy_blunt_hit_floor_smoke.tscn`
- `vfx/vfx_heavy_blunt/heavy_blunt_hit_floor_smoke.png`

### `scenes/vfx/heavy_blunt/components/vfx_heavy_blunt_hit_spikes.tscn`
- `vfx/vfx_heavy_blunt/heavy_blunt_hit_spikes.png`

### `scenes/vfx/missile/vfx_missile_core.tscn`
- `vfx/missile/missile_core_b.png`

### `scenes/vfx/missile/vfx_missile_impact_core.tscn`
- `vfx/missile/missile_impact_core.png`

### `scenes/vfx/missile/vfx_missile_impact_smoke.tscn`
- `vfx/missile/missile_impact_smoke.png`

### `scenes/vfx/missile/vfx_missile_impact_streak_side.tscn`
- `vfx/missile/missile_impact_streak_a.png`

### `scenes/vfx/missile/vfx_missile_sky_flare.tscn`
- `vfx/missile/missile_sky_flare.png`

### `scenes/vfx/missile/vfx_missile_sky_ray.tscn`
- `vfx/missile/missile_sky_ray.png`

### `scenes/vfx/slash/vfx_slash_core.tscn`
- `vfx/slash/slash_flipbook.png`

### `scenes/vfx/starry_impact/vfx_starry_impact_core.tscn`
- `vfx/starry_impact/starry_impact_core.png`

### `scenes/vfx/starry_impact/vfx_starry_impact_small_stars.tscn`
- `vfx/starry_impact/starry_impact_small_star.png`

### `scenes/vfx/starry_impact/vfx_starry_impact_smoke_flipbook.tscn`
- `vfx/starry_impact/starry_impact_smoke_flipbook.png`

### `scenes/vfx/vfx_attack_blunt.tscn`
子场景: `vfx/common/vfx_common_blunt_impact_core.tscn`、`vfx/common/vfx_common_glow.tscn`、`vfx/common/vfx_common_specks.tscn`
- `vfx/common/common_speck.png`

### `scenes/vfx/vfx_attack_slash.tscn`
- `vfx/vfx_attack_slash/vfx_attack_slash_00.png`
- `vfx/vfx_attack_slash/vfx_attack_slash_01.png`
- `vfx/vfx_attack_slash/vfx_attack_slash_02.png`
- `vfx/vfx_attack_slash/vfx_attack_slash_03.png`

### `scenes/vfx/vfx_big_slash.tscn`
子场景: `vfx/slash/vfx_slash_core.tscn`

### `scenes/vfx/vfx_big_slash_impact.tscn`
子场景: `vfx/big_slash/vfx_big_slash_impact_core.tscn`、`vfx/big_slash/vfx_big_slash_impact_side_smoke.tscn`、`vfx/common/vfx_common_hit_flare.tscn`、`vfx/common/vfx_common_specks.tscn`
- `vfx/common/common_speck.png`

### `scenes/vfx/vfx_blood_wall.tscn`
- `vfx/vfx_blood_wall/blood_wall_asset.png`
- `vfx/vfx_blood_wall/tear_drop.png`
- `vfx/vfx_blood_wall/vfx_blood_wall_flash.png`

### `scenes/vfx/vfx_bloody_impact.tscn`
- `vfx/vfx_bloody_impact/bloodyimpact_anim_all.png`

### `scenes/vfx/vfx_fire_burning.tscn`
子场景: `vfx/common/vfx_common_glow.tscn`、`vfx/common/vfx_common_specks.tscn`、`vfx/fire_impact/vfx_fire_burst_right_flipbook.tscn`、`vfx/fire_impact/vfx_fire_flipbook.tscn`

### `scenes/vfx/vfx_fire_burst.tscn`
子场景: `vfx/common/vfx_common_glow.tscn`、`vfx/common/vfx_common_hit_flare.tscn`、`vfx/common/vfx_common_specks.tscn`、`vfx/common/vfx_poof.tscn`、`vfx/fire_impact/vfx_fire_burst_center_flipbook.tscn`、`vfx/fire_impact/vfx_fire_burst_left_flipbook.tscn`、`vfx/fire_impact/vfx_fire_burst_right_flipbook.tscn`
- `vfx/common/common_speck.png`

### `scenes/vfx/vfx_giant_horizontal_slash.tscn`
- `vfx/vfx_giant_horizontal_slash/giant_horizontal_slash.png`

### `scenes/vfx/vfx_heavy_blunt.tscn`
子场景: `vfx/common/vfx_common_hit_flare.tscn`、`vfx/common/vfx_common_specks.tscn`、`vfx/distortions/vfx_outward_screen_distortion.tscn`、`vfx/heavy_blunt/components/vfx_heavy_blunt_anticipation_core.tscn`、`vfx/heavy_blunt/components/vfx_heavy_blunt_hit_core.tscn`、`vfx/heavy_blunt/components/vfx_heavy_blunt_hit_core_glow.tscn`、`vfx/heavy_blunt/components/vfx_heavy_blunt_hit_floor_smoke.tscn`、`vfx/heavy_blunt/components/vfx_heavy_blunt_hit_spikes.tscn`
- `vfx/common/common_speck.png`

### `scenes/vfx/vfx_large_magic_missile.tscn`
子场景: `vfx/common/vfx_common_glow.tscn`、`vfx/common/vfx_common_ring_polar_a.tscn`、`vfx/common/vfx_common_specks.tscn`、`vfx/common/vfx_poof.tscn`、`vfx/distortions/vfx_outward_screen_distortion.tscn`、`vfx/heavy_blunt/components/vfx_heavy_blunt_hit_floor_smoke.tscn`、`vfx/missile/vfx_missile_core.tscn`、`vfx/missile/vfx_missile_impact_core.tscn`、`vfx/missile/vfx_missile_impact_smoke.tscn`、`vfx/missile/vfx_missile_impact_streak_side.tscn`、`vfx/missile/vfx_missile_sky_flare.tscn`、`vfx/missile/vfx_missile_sky_ray.tscn`

### `scenes/vfx/vfx_molten_fist.tscn`
- `vfx/vfx_heal_osty/falloff_asset.png`
- `vfx/vfx_heal_osty/heal_stretch_asset.png`
- `vfx/vfx_molten_fist/fist.png`
- `vfx/vfx_molten_fist/fist_bg_v2.png`
- `vfx/vfx_molten_fist/ground_spark.png`
- `vfx/vfx_molten_fist/vfx_rock_light_ver.png`
- `vfx/vfx_rock_shatter/vfx_rock_shatter_rock.png`
- `vfx/vfx_rock_shatter/vfx_rock_shatter_shine_01.png`

### `scenes/vfx/vfx_power_up/vfx_power_up.tscn`
子场景: `vfx/vfx_power_up/vfx_power_up_2d_front.tscn`、`vfx/vfx_power_up/vfx_power_up_3d_back.tscn`

### `scenes/vfx/vfx_power_up/vfx_power_up_2d_front.tscn`
- `vfx/vfx_power_up/beam.png`
- `vfx/vfx_power_up/ghostly_beam.png`
- `vfx/vfx_power_up/sparkle.png`

### `scenes/vfx/vfx_power_up/vfx_power_up_3d_back.tscn`
- `vfx/vfx_power_up/aura.png`
- `vfx/vfx_power_up/base_glow.png`
- `vfx/vfx_power_up/base_ring.png`
- `vfx/vfx_power_up/inner_ring.png`

### `scenes/vfx/vfx_small_magic_missile.tscn`
子场景: `vfx/common/vfx_common_glow.tscn`、`vfx/common/vfx_common_hit_flare.tscn`、`vfx/common/vfx_common_ring_polar_a.tscn`、`vfx/common/vfx_common_specks.tscn`、`vfx/common/vfx_smoke_flipbook.tscn`、`vfx/missile/vfx_missile_core.tscn`、`vfx/missile/vfx_missile_sky_ray.tscn`

### `scenes/vfx/vfx_starry_impact.tscn`
子场景: `vfx/common/vfx_common_glow.tscn`、`vfx/common/vfx_common_ring_polar_b.tscn`、`vfx/common/vfx_common_specks.tscn`、`vfx/distortions/vfx_outward_screen_distortion.tscn`、`vfx/starry_impact/vfx_starry_impact_core.tscn`、`vfx/starry_impact/vfx_starry_impact_small_stars.tscn`、`vfx/starry_impact/vfx_starry_impact_smoke_flipbook.tscn`

### `scenes/vfx/vfx_thrash.tscn`
- `vfx/vfx_heal/heal_asset.png`
- `vfx/vfx_thrash/flash_v2.png`
- `vfx/vfx_thrash/pummel_fist.png`
- `vfx/vfx_thrash/sparkle.png`

### `scenes/vfx/whole_screen/horizontal_lines_vfx.tscn`
- `vfx/shared_use/motion_line.png`
