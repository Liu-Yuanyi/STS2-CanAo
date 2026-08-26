# swap_energy_ring_colors.py — 把能量球的"旋转内环"（layer 2/3，纯蓝）与
# "外框"（layer 4/5，浅蓝）的颜色等级对调：外框变纯蓝、旋转环变浅蓝。
# 原理：取两组各自不透明像素的平均 HSV，把每组像素重染到对方均值，
# 明度按本组相对明暗保留笔触层次。不用 AI。
from pathlib import Path

from PIL import Image

ROOT = Path(__file__).resolve().parent.parent
DIR = ROOT / "godot" / "images" / "ui" / "combat" / "energy_counters" / "can_ao"

RING_LAYERS = (2, 3)   # 旋转内环（原纯蓝）
FRAME_LAYERS = (4, 5)  # 外框（原浅蓝）

def mean_hsv(path: Path) -> tuple[float, float, float]:
    img = Image.open(path).convert("RGBA")
    a = img.getchannel("A")
    opaque = a.point(lambda v: 255 if v > 40 else 0)
    hsv = img.convert("HSV")
    h, s, v = hsv.split()
    n = max(1, sum(1 for px in opaque.getdata() if px))
    # 用直方图求均值，避免逐像素
    def band_mean(band):
        hist = band.histogram(opaque)
        total = sum(i * c for i, c in enumerate(hist))
        return total / n
    return band_mean(h), band_mean(s), band_mean(v)

def retint(path: Path, target: tuple[float, float, float], src_mean: tuple[float, float, float]) -> None:
    img = Image.open(path).convert("RGBA")
    r, g, b, a = img.split()
    hsv = Image.merge("RGB", (r, g, b)).convert("HSV")
    h, s, v = hsv.split()
    th, ts, tv = target
    sh, ss, sv = src_mean
    s_ratio = ts / ss if ss > 1 else 1.0
    v_ratio = tv / sv if sv > 1 else 1.0
    h = h.point(lambda x: int(th))
    s = s.point(lambda x: min(255, int(x * s_ratio)))
    v = v.point(lambda x: min(255, int(x * v_ratio)))
    out = Image.merge("HSV", (h, s, v)).convert("RGBA")
    out.putalpha(a)
    out.save(path)

def main() -> int:
    ring_mean = tuple(
        sum(m[i] for m in (mean_hsv(DIR / f"can_ao_orb_layer_{i}.png") for i in RING_LAYERS)) / len(RING_LAYERS)
        for i in range(3))
    frame_mean = tuple(
        sum(m[i] for m in (mean_hsv(DIR / f"can_ao_orb_layer_{i}.png") for i in FRAME_LAYERS)) / len(FRAME_LAYERS)
        for i in range(3))
    print("旋转环均值(纯蓝):", tuple(round(x, 1) for x in ring_mean))
    print("外框均值(浅蓝):", tuple(round(x, 1) for x in frame_mean))
    for i in RING_LAYERS:
        retint(DIR / f"can_ao_orb_layer_{i}.png", frame_mean, ring_mean)
    for i in FRAME_LAYERS:
        retint(DIR / f"can_ao_orb_layer_{i}.png", ring_mean, frame_mean)
    print("对调完成")
    return 0

if __name__ == "__main__":
    raise SystemExit(main())
