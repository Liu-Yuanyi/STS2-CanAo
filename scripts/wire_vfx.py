# wire_vfx.py — 一次性：给卡牌/观星问月挂 VFX（OnEnqueuePlayVfx / 选择分支）。
import re
from pathlib import Path

SRC = Path(__file__).resolve().parent.parent / "src" / "CanAoNative"

CARD_MAP = {
    "Cards/EdictCard.cs": "edictseal",
    "Cards/TianFengXingTaiCard.cs": "phoenixblaze",
    "Cards/FengYanBuXiCard.cs": "phoenixblaze",
    "Cards/FuBiCard.cs": "stanceswap",
    "Cards/JiHuoCard.cs": "exhaust",
    "Cards/QuShiBaiZhongCard.cs": "debuff",
    "Cards/ZheGuanCard.cs": "debuff",
    "Cards/ZanBiFengMangCard.cs": "debuff",
    "Cards/YuanJunCard.cs": "summon",
    "Cards/QingLuanGouFaCard.cs": "starburst",
    "Cards/XingHuiHuZhenCard.cs": "starburst",
    "Cards/GuiLunCard.cs": "gain",
    "Cards/WanBangLaiChaoCard.cs": "buffup",
    "Cards/ShiWeiCard.cs": "buffup",
    "Cards/FengLinJiuTianCard.cs": "buffup",
}

METHOD_TMPL = """
    public override Task OnEnqueuePlayVfx(Creature? target)
    {
        if (Owner?.Creature != null)
            VfxCmd.PlayOnCreatureCenter(
                Owner.Creature,
                "vfx/can_ao/vfx_%s");

        return Task.CompletedTask;
    }

"""

def main() -> int:
    for rel, vfx in CARD_MAP.items():
        p = SRC / rel
        t = p.read_text(encoding="utf-8")
        if "OnEnqueuePlayVfx" in t:
            print("已有，跳过", rel)
            continue
        if "using MegaCrit.Sts2.Core.Entities.Creatures;" not in t:
            t = t.replace(
                "using MegaCrit.Sts2.Core.Entities.Cards;",
                "using MegaCrit.Sts2.Core.Entities.Cards;\n"
                "using MegaCrit.Sts2.Core.Entities.Creatures;",
                1)
        anchor = "    protected override void OnUpgrade()"
        if anchor not in t:
            print("!! 锚点缺失", rel)
            return 1
        t = t.replace(anchor, METHOD_TMPL % vfx + anchor, 1)
        p.write_text(t, encoding="utf-8", newline="\n")
        print("OK", rel, "->", vfx)

    # 观星问月：按选择在 OnPlay 内播 starburst/gain
    p = SRC / "Cards" / "GuanXingWenYueCard.cs"
    t = p.read_text(encoding="utf-8")
    old_star = ("        if (choice is GuanXingOptionCard)\n"
                "        {\n"
                "            await PowerCmd.Apply<StarPower>(")
    new_star = ("        if (choice is GuanXingOptionCard)\n"
                "        {\n"
                "            VfxCmd.PlayOnCreatureCenter(\n"
                "                owner.Creature,\n"
                "                \"vfx/can_ao/vfx_starburst\");\n\n"
                "            await PowerCmd.Apply<StarPower>(")
    old_moon = ("        else if (choice is WenYueOptionCard)\n"
                "        {\n"
                "            await PowerCmd.Apply<MoonPower>(")
    new_moon = ("        else if (choice is WenYueOptionCard)\n"
                "        {\n"
                "            VfxCmd.PlayOnCreatureCenter(\n"
                "                owner.Creature,\n"
                "                \"vfx/can_ao/vfx_gain\");\n\n"
                "            await PowerCmd.Apply<MoonPower>(")
    for old, new in ((old_star, new_star), (old_moon, new_moon)):
        if old not in t:
            print("!! 观星问月锚点缺失")
            return 1
        t = t.replace(old, new, 1)
    p.write_text(t, encoding="utf-8", newline="\n")
    print("OK 观星问月 选择分支特效")
    return 0

if __name__ == "__main__":
    raise SystemExit(main())
