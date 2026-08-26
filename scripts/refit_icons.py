# refit_icons.py — 统一放大 Power/遗物图标的内容占比，使其与星/月视觉大小一致。
#
# 规则（用户 2026-08-16 拍板的逻辑说明）：
#   0. 内容居中，绝不超出边界（1024 母版最小边距 12px）。
#   1. 以用户手绘的星/月为基准边距 m_star（内容包围盒到画布边的距离）。
#   2. 形状分类用两个客观指标：
#        填充率 f = 不透明像素数 / 包围盒面积（越胖越高）；
#        长短边比 r = min(w,h)/max(w,h)（越细条越低）。
#      圆胖（f≥0.55 且 r≥0.65）：目标边距 = m_star × 0.85 —— 胖形显小，放大一点；
#      细瘦（f≤0.35 或 r≤0.45）：目标边距 = m_star × 1.15 —— 瘦形显大，收一点；
#      其余中等：目标边距 = m_star。
#   3. 星/月/下回合星（用户手绘基准）不参与重排。
# 重排后重新派生 Power 64/256 与遗物 85/85轮廓/256。
# 用法: python scripts/refit_icons.py
from pathlib import Path

from PIL import Image, ImageChops, ImageFilter

ROOT = Path(__file__).resolve().parent.parent
ART = ROOT / "art"
GODOT_IMG = ROOT / "godot" / "images"
SIZE = 1024
MIN_MARGIN = 12

# 用户手绘基准（不参与重排）
REFERENCE = ["star_power", "moon_power", "next_turn_star_power"]

def content_bbox(img: Image.Image):
    return img.getchannel("A").getbbox()

def margin_of(path: Path) -> float:
    img = Image.open(path).convert("RGBA")
    bbox = content_bbox(img)
    if not bbox:
        return 0.0
    w = bbox[2] - bbox[0]
    h = bbox[3] - bbox[1]
    return (SIZE - max(w, h)) / 2

def classify(img: Image.Image) -> str:
    bbox = content_bbox(img)
    if not bbox:
        return "mid"
    w = bbox[2] - bbox[0]
    h = bbox[3] - bbox[1]
    a = img.getchannel("A")
    px = sum(1 for v in a.getdata() if v > 24)
    fill = px / (w * h)
    ratio = min(w, h) / max(w, h)
    if fill >= 0.55 and ratio >= 0.65:
        return "fat"
    if fill <= 0.35 or ratio <= 0.45:
        return "slim"
    return "mid"

def refit(master_path: Path, target_margin: float) -> Image.Image:
    img = Image.open(master_path).convert("RGBA")
    bbox = content_bbox(img)
    if not bbox:
        return img
    content = img.crop(bbox)
    w, h = content.size
    avail = SIZE - 2 * target_margin
    scale = min(avail / w, avail / h)
    nw, nh = max(1, int(w * scale)), max(1, int(h * scale))
    content = content.resize((nw, nh), Image.LANCZOS)
    canvas = Image.new("RGBA", (SIZE, SIZE), (0, 0, 0, 0))
    canvas.paste(content, ((SIZE - nw) // 2, (SIZE - nh) // 2), content)
    return canvas

def make_outline(small: Image.Image) -> Image.Image:
    a = small.getchannel("A")
    dilated = a.filter(ImageFilter.MaxFilter(7))
    ring = ImageChops.subtract(dilated, a)
    transparent = Image.new("RGBA", small.size, (255, 255, 255, 0))
    white = Image.new("RGBA", small.size, (255, 255, 255, 255))
    return Image.composite(white, transparent, ring)

def main() -> int:
    star_margin = margin_of(ART / "powers" / "processed" / "star_power_1024.png")
    moon_margin = margin_of(ART / "powers" / "processed" / "moon_power_1024.png")
    m_star = (star_margin + moon_margin) / 2
    print(f"基准边距: 星 {star_margin:.0f}px / 月 {moon_margin:.0f}px → m_star={m_star:.0f}px")

    factors = {"fat": 0.85, "mid": 1.0, "slim": 1.15}
    stats = {"fat": 0, "mid": 0, "slim": 0}

    # Power：64/256
    for master in sorted((ART / "powers" / "processed").glob("*_1024.png")):
        pid = master.name[:-len("_1024.png")]
        img = Image.open(master).convert("RGBA")
        if pid in REFERENCE:
            out = img
            shape = "基准"
        else:
            shape = classify(img)
            stats[shape] += 1
            margin = max(MIN_MARGIN, m_star * factors[shape])
            out = refit(master, margin)
            out.save(master)
        for sub, size in (("", 256), ("small", 64)):
            d = GODOT_IMG / "powers" / sub if sub else GODOT_IMG / "powers"
            out.resize((size, size), Image.LANCZOS).save(d / f"{pid}.png")
        if shape != "基准":
            print(f"  power {pid}: {shape} 边距→{max(MIN_MARGIN, m_star*factors[shape]):.0f}px")

    # 遗物：85/85轮廓/256
    for master in sorted((ART / "relics" / "processed").glob("*_1024.png")):
        rid = master.name[:-len("_1024.png")]
        img = Image.open(master).convert("RGBA")
        shape = classify(img)
        stats[shape] += 1
        margin = max(MIN_MARGIN, m_star * factors[shape])
        out = refit(master, margin)
        out.save(master)
        out.resize((256, 256), Image.LANCZOS).save(GODOT_IMG / "relics" / f"{rid}.png")
        small = out.resize((85, 85), Image.LANCZOS)
        small.save(GODOT_IMG / "relics" / "small" / f"{rid}.png")
        make_outline(small).save(GODOT_IMG / "relics" / "outline" / f"{rid}.png")
        print(f"  relic {rid}: {shape} 边距→{max(MIN_MARGIN, m_star*factors[shape]):.0f}px")

    print("形状分布:", stats)
    print("refit 完成")
    return 0

if __name__ == "__main__":
    raise SystemExit(main())
