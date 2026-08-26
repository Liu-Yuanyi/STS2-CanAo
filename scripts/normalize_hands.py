# normalize_hands.py — 手势部件预处理：
# 1) 四张 AI 风格化手势统一到 422×1200 画布（等高缩放、水平居中、底对齐）；
# 2) 把铁甲绿色皮肤程序改为残傲冷白肤色（保留明暗，只改色相/饱和度）。
# 输出到 art/hands/parts/。
from pathlib import Path

from PIL import Image

ROOT = Path(__file__).resolve().parent.parent
RAW = ROOT / "art" / "hands" / "raw"
OUT = ROOT / "art" / "hands" / "parts"
CANVAS = (422, 1200)

def normalize(img: Image.Image) -> Image.Image:
    """裁透明边 → 等比缩放到高 1200 → 底对齐居中放 422×1200。"""
    bbox = img.getchannel("A").getbbox()
    if bbox:
        img = img.crop(bbox)
    scale = CANVAS[1] / img.height
    nw = int(img.width * scale)
    img = img.resize((nw, CANVAS[1]), Image.LANCZOS)
    canvas = Image.new("RGBA", CANVAS, (0, 0, 0, 0))
    canvas.paste(img, ((CANVAS[0] - nw) // 2, 0), img)
    return canvas

def fix_skin(img: Image.Image) -> Image.Image:
    """绿色皮肤 → 冷白（带极淡青灰）。只处理偏绿且带饱和度的像素。"""
    r, g, b, a = img.split()
    hsv = Image.merge("RGB", (r, g, b)).convert("HSV")
    h, s, v = hsv.split()

    hp, sp, vp = h.load(), s.load(), v.load()
    w, hh = img.size
    for y in range(hh):
        for x in range(w):
            hue = hp[x, y]
            sat = sp[x, y]
            # 绿色区间（PIL HSV：绿≈85 附近，取 60~120）且有一定饱和度
            if 55 <= hue <= 125 and sat > 25:
                hp[x, y] = 148           # 冷青灰（≈208°）
                sp[x, y] = min(60, sat // 4)  # 大幅降饱和→冷白
                vp[x, y] = min(255, int(vp[x, y] * 1.15))  # 略提亮
    out = Image.merge("HSV", (h, s, v)).convert("RGBA")
    out.putalpha(a)
    return out

def main() -> int:
    OUT.mkdir(parents=True, exist_ok=True)
    for g in ("rock", "scissors", "paper", "point"):
        src = RAW / f"hand_{g}_A_v01.png"
        img = Image.open(src).convert("RGBA")
        out = fix_skin(normalize(img))
        out.save(OUT / f"hand_{g}_norm.png")
        print("OK", g, out.size)
    print("normalize_hands 完成 ->", OUT)
    return 0

if __name__ == "__main__":
    raise SystemExit(main())
