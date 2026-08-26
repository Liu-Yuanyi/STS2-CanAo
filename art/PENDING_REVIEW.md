# 待审批 / 待处理总表（2026-08-16 收官更新）

## 已全部完成（本阶段）

- 遗物 9 件三件套、Power 31 个两件套（含派生）、药水 3 瓶（全 A）、能量计数器改蓝
  （五层+充能 VFX+文本/费用图标）、卡框改残傲专属亮浅蓝（h0.58 s0.7 v1.7，
  条目色 #7FB8F5，hsv 着色器机制见下）、v12 全部 39 项 + 后续两项修复
  （万邦来朝逐张发牌视觉、税契 GoldReward 奖励化）。
- 星月轮转图标 = batch80 B（最后一件 Power 图标）。

## 卡框颜色机制备忘（重要）

- 卡框着色器 `hsv.gdshader`：底图偏红中暗（v≈0.41、s≈0.41），h 是**相对旋转**
  （渲染色相 = 底图 170° + (1-h)×360°），s/v 与底图**相乘**；
  v>1 为过驱动提亮（原生亮色系全用 1.2+）。
- 想改色：本地 numpy 仿真脚本可复刻渲染结果（参数记录于
  `art/frame_color_candidates_v3.png` 生成代码），报 RGB 可反解 h/s/v。

## 剩余大项

1. **多人手势 4 张**：用户手绘中（教学：`art/hands/HOWTO.md`，参考图
   `art/hands/native_hands_reference.png` + `sts2_assets/hands_all/`）；
   画好放进 `godot/images/ui/hands/` 后叫我跑 import+Deploy。
2. ~~VFX 贴图 15 张~~：**已决定放弃**（2026-08-16）——A/B 两版母版归档在
   `art/vfx/raw/` 备日后启用；代码侧已全套退回原生特效
   （slash/heavy_blunt/attack_blunt），场景与纹理已从 pck 移除。
3. **宣传物料**（清单 §9）：mod 图标（可复用帝国纹章）、工坊主缩略图、
   横版宣传图、海报、卡组展示图。
4. **实机验收**：`docs/V12_TEST_CHECKLIST.md`（重点：三张删卡的旧存档兼容、
   税契奖励结算、万邦来朝产牌、图标/卡框/能量球视觉回归）。
5. 后续阶段：动画（spine 需 Gemini Key）、先古演出（可选）。
