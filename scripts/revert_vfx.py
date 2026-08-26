# revert_vfx.py — 一次性：全套退回原生 VFX（用户 2026-08-16 决定）。
# 攻击卡 WithHitFx 换原生路径；卡牌级 OnEnqueuePlayVfx/分支特效全部移除；
# 删除 can_ao VFX 场景与纹理（原始母版保留在 art/vfx/raw/ 归档）。
import re
from pathlib import Path

SRC = Path(__file__).resolve().parent.parent / "src" / "CanAoNative"

VANILLA = {
    "slash": "vfx/vfx_attack_slash",
    "heavy": "vfx/vfx_heavy_blunt",
    "aoe": "vfx/vfx_attack_blunt",
    "moonslash": "vfx/vfx_attack_slash",
    "starmoon": "vfx/vfx_attack_slash",
}

ON_ENQUEUE_FILES = [
    "Cards/EdictCard.cs", "Cards/TianFengXingTaiCard.cs", "Cards/FengYanBuXiCard.cs",
    "Cards/FuBiCard.cs", "Cards/JiHuoCard.cs", "Cards/QuShiBaiZhongCard.cs",
    "Cards/ZheGuanCard.cs", "Cards/ZanBiFengMangCard.cs", "Cards/YuanJunCard.cs",
    "Cards/QingLuanGouFaCard.cs", "Cards/XingHuiHuZhenCard.cs", "Cards/GuiLunCard.cs",
    "Cards/WanBangLaiChaoCard.cs", "Cards/ShiWeiCard.cs", "Cards/FengLinJiuTianCard.cs",
]

def main() -> int:
    # 1. WithHitFx → 原生路径
    for cs in list((SRC / "Cards").glob("*.cs")):
        t = cs.read_text(encoding="utf-8")
        orig = t
        for name, vanilla in VANILLA.items():
            t = t.replace(f'.WithHitFx("vfx/can_ao/vfx_{name}")',
                          f'.WithHitFx("{vanilla}")')
        # 羽列千军浴火分支的 aoe 覆盖行（含前导空行）删除
        t = t.replace('            attack.WithHitFx("vfx/vfx_attack_blunt");\n\n', "")
        if t != orig:
            cs.write_text(t, encoding="utf-8", newline="\n")
            print("WithHitFx 退回:", cs.name)

    # 2. 移除 OnEnqueuePlayVfx 方法与配套 using
    block_re = re.compile(
        r"\n    public override Task OnEnqueuePlayVfx\(Creature\? target\)\n"
        r"    \{\n.*?\n    \}\n\n", re.S)
    for rel in ON_ENQUEUE_FILES:
        p = SRC / rel
        t = p.read_text(encoding="utf-8")
        t2, n = block_re.subn("\n", t)
        if n:
            # 该方法是在 Creatures using 不存在的前提下能编译的，故安全移除
            t2 = t2.replace(
                "using MegaCrit.Sts2.Core.Entities.Creatures;\n", "", 1)
            p.write_text(t2, encoding="utf-8", newline="\n")
            print("移除 OnEnqueuePlayVfx:", rel)
        else:
            print("!! 未找到方法块:", rel)

    # 3. 观星问月分支特效移除
    p = SRC / "Cards" / "GuanXingWenYueCard.cs"
    t = p.read_text(encoding="utf-8")
    for vfx in ("starburst", "gain"):
        t = t.replace(
            "            VfxCmd.PlayOnCreatureCenter(\n"
            "                owner.Creature,\n"
            f"                \"vfx/can_ao/vfx_{vfx}\");\n\n", "")
    p.write_text(t, encoding="utf-8", newline="\n")
    print("观星问月分支特效移除")

    # 4. YuHuoResolver 冷焰块移除
    p = SRC / "Rules" / "YuHuo" / "YuHuoResolver.cs"
    t = p.read_text(encoding="utf-8")
    t = t.replace(
        "                    if (card.Owner?.Creature != null)\n"
        "                    {\n"
        "                        VfxCmd.PlayOnCreatureCenter(\n"
        "                            card.Owner.Creature,\n"
        "                            \"vfx/can_ao/vfx_luanflame\");\n"
        "                    }\n\n", "")
    p.write_text(t, encoding="utf-8", newline="\n")
    print("YuHuoResolver 移除")

    # 5. 凤火军械 summon 移除
    p = SRC / "Powers" / "FengHuoJunXiePower.cs"
    t = p.read_text(encoding="utf-8")
    t = t.replace(
        "        VfxCmd.PlayOnCreatureCenter(Owner, \"vfx/can_ao/vfx_summon\");\n\n", "")
    p.write_text(t, encoding="utf-8", newline="\n")
    print("FengHuoJunXiePower 移除")
    return 0

if __name__ == "__main__":
    raise SystemExit(main())
