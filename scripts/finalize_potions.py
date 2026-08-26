# finalize_potions.py — 药水定稿母版 → godot/images/potions/{large(256),small(80),outline(80)}。
# 用法: python scripts/finalize_potions.py
from pathlib import Path

from PIL import Image, ImageChops, ImageFilter

ROOT = Path(__file__).resolve().parent.parent
ART = ROOT / "art"
GODOT_IMG = ROOT / "godot" / "images"

# 用户 2026-08-16 拍板：三瓶全选 A
POTIONS = {
    "qiong_jiang_potion": ART / "potions" / "raw" / "potion_qiongjiang_A_v01.png",
    "feng_wei_jiu_potion": ART / "potions" / "raw" / "potion_fengweijiu_A_v01.png",
    "yu_ling_ping_potion": ART / "potions" / "raw" / "potion_yulingping_A_v01.png",
}

def load_square_master(src: Path, margin: float = 0.05) -> Image.Image:
    img = Image.open(src).convert("RGBA")
    bbox = img.getchannel("A").getbbox()
    if bbox:
        img = img.crop(bbox)
    w, h = img.size
    side = int(max(w, h) * (1 + margin * 2))
    canvas = Image.new("RGBA", (side, side), (0, 0, 0, 0))
    canvas.paste(img, ((side - w) // 2, (side - h) // 2), img)
    return canvas.resize((1024, 1024), Image.LANCZOS)

def make_outline(small: Image.Image) -> Image.Image:
    a = small.getchannel("A")
    dilated = a.filter(ImageFilter.MaxFilter(7))
    ring = ImageChops.subtract(dilated, a)
    transparent = Image.new("RGBA", small.size, (255, 255, 255, 0))
    white = Image.new("RGBA", small.size, (255, 255, 255, 255))
    return Image.composite(white, transparent, ring)

def main() -> int:
    proc = ART / "potions" / "processed"
    proc.mkdir(parents=True, exist_ok=True)
    for pid, src in POTIONS.items():
        if not src.exists():
            print("!! 缺源文件:", src)
            return 1
        master = load_square_master(src)
        master.save(proc / f"{pid}_1024.png")
        for sub, size in (("large", 256), ("small", 80), ("outline", 80)):
            out_dir = GODOT_IMG / "potions" / sub
            out_dir.mkdir(parents=True, exist_ok=True)
            if sub == "outline":
                make_outline(master.resize((size, size), Image.LANCZOS)).save(
                    out_dir / f"{pid}.png")
            else:
                master.resize((size, size), Image.LANCZOS).save(
                    out_dir / f"{pid}.png")
        print("OK", pid)
    print("finalize_potions 完成")
    return 0

if __name__ == "__main__":
    raise SystemExit(main())
