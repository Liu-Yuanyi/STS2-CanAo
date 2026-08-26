# make_sheet_v03.py — batch75（剩余 7 个 Power × A/B）审查拼版。
# 用法: python scripts/make_sheet_v03.py
import json
import shutil
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont

ROOT = Path(__file__).resolve().parent.parent  # CanAoNative/
BOT = ROOT.parent / "aizex-image-bot"
CELL = 300
LABEL_H = 40
PAD = 12

GROUPS = [
    ("下回合星", "power_nextturnstar"),
    ("盘旋", "power_panxuan"),
    ("天凤军阵", "power_tianfengjunzhen"),
    ("帝国余威", "power_diguoyuwei"),
    ("最后一舞", "power_zuihouyiwu"),
    ("凤火军械", "power_fenghuojunxie"),
    ("瓦魔远征", "power_wamoyuanzheng"),
]

def checker(size: int, sq: int = 20) -> Image.Image:
    img = Image.new("RGB", (size, size), (200, 200, 200))
    d = ImageDraw.Draw(img)
    for y in range(0, size, sq):
        for x in range(0, size, sq):
            if (x // sq + y // sq) % 2 == 0:
                d.rectangle([x, y, x + sq - 1, y + sq - 1], fill=(120, 120, 120))
    return img

def main() -> int:
    records = {}
    for line in (BOT / "batch75_jobs_results.jsonl").read_text(encoding="utf-8").splitlines():
        if line.strip():
            r = json.loads(line)
            records[r["tag"]] = r

    raw_dir = ROOT / "art" / "powers" / "raw"
    raw_dir.mkdir(parents=True, exist_ok=True)
    out = ROOT / "art" / "powers" / "power_sheet_v03.png"
    font = ImageFont.truetype("C:/Windows/Fonts/msyh.ttc", 22)

    W = 2 * CELL + 3 * PAD
    H = len(GROUPS) * (CELL + LABEL_H) + (len(GROUPS) + 1) * PAD
    sheet = Image.new("RGB", (W, H), (45, 45, 52))
    draw = ImageDraw.Draw(sheet)
    missing = []
    for row, (name, prefix) in enumerate(GROUPS):
        y = PAD + row * (CELL + LABEL_H + PAD)
        for col, ab in enumerate(("A", "B")):
            tag = f"{prefix}_{ab}"
            x = PAD + col * (CELL + PAD)
            r = records.get(tag)
            ok = r and r.get("ok") and r.get("outFile") and Path(r["outFile"]).exists()
            dst = None
            if ok:
                src = Path(r["outFile"])
                dst = raw_dir / f"{tag}_v01{src.suffix.lower()}"
                if not dst.exists():
                    shutil.copy2(src, dst)
            if dst:
                img = Image.open(dst).convert("RGBA")
                cell = checker(CELL).convert("RGBA")
                img.thumbnail((CELL, CELL), Image.LANCZOS)
                cell.paste(img, ((CELL - img.width) // 2, (CELL - img.height) // 2), img)
                sheet.paste(cell.convert("RGB"), (x, y))
                label = f"{name} {ab}"
            else:
                draw.rectangle([x, y, x + CELL, y + CELL], fill=(90, 40, 40))
                label = f"{name} {ab} 缺失"
                missing.append(tag)
            draw.text((x + 8, y + CELL + 6), label, font=font, fill=(235, 235, 235))

    sheet.save(out)
    print(f"sheet: {out} ({W}x{H})")
    if missing:
        print("缺失:", ", ".join(missing))
        return 1
    print("14 张全部就位")
    return 0

if __name__ == "__main__":
    raise SystemExit(main())
