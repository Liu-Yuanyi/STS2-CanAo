# recolor_energy_orb.py — 把铁甲战士能量球 5 层纹理从红橙配色程序改为残傲青蓝配色。
# 不用 AI。色相 +约140°（红→青蓝），饱和度与明度不动，alpha 保留。
# 输入: ../sts2_assets/refs_v12/ironclad_orb_layer_{1..5}.png
# 输出: godot/images/ui/combat/energy_counters/can_ao/can_ao_orb_layer_{1..5}.png
from pathlib import Path

from PIL import Image

ROOT = Path(__file__).resolve().parent.parent
SRC = ROOT.parent / "sts2_assets" / "refs_v12"
DST = ROOT / "godot" / "images" / "ui" / "combat" / "energy_counters" / "can_ao"

HUE_SHIFT = 130  # 0-255 色环位移，约 +183°（红→青蓝，橙→蓝，黄→蓝紫）

def main() -> int:
    DST.mkdir(parents=True, exist_ok=True)
    for i in range(1, 6):
        src = SRC / f"ironclad_orb_layer_{i}.png"
        img = Image.open(src).convert("RGBA")
        r, g, b, a = img.split()
        hsv = Image.merge("RGB", (r, g, b)).convert("HSV")
        h, s, v = hsv.split()
        h = h.point(lambda x: (x + HUE_SHIFT) % 256)
        out = Image.merge("HSV", (h, s, v)).convert("RGBA")
        out.putalpha(a)
        out.save(DST / f"can_ao_orb_layer_{i}.png")
        print(f"can_ao_orb_layer_{i}.png  {out.size}")
    print("recolor 完成 ->", DST)
    return 0

if __name__ == "__main__":
    raise SystemExit(main())
