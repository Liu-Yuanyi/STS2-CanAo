# make_power_sheet.py — 读取 batch72_jobs_results.jsonl，把 10 个核心 Power × 多版本拼成审查版。
# 星/月/凤威各 4 版（A~D 一行排开），其余 7 个各 2 版（A/B）。
# 用法: python scripts/make_power_sheet.py [results.jsonl路径] [输出png路径]
import json
import shutil
import sys
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont

ROOT = Path(__file__).resolve().parent.parent  # CanAoNative/
BOT = ROOT.parent / "aizex-image-bot"
RESULTS = Path(sys.argv[1]) if len(sys.argv) > 1 else BOT / "batch72_jobs_results.jsonl"
OUT = Path(sys.argv[2]) if len(sys.argv) > 2 else ROOT / "art" / "powers" / "power_sheet_v01.png"
RAW_DIR = ROOT / "art" / "powers" / "raw"

# (显示名, tag 前缀, 版本列表)
GROUPS = [
    ("星 STAR", "power_star", "ABCD"),
    ("月 MOON", "power_moon", "ABCD"),
    ("凤威 FENG_WEI", "power_fengwei", "ABCD"),
    ("临时凤威", "power_tempfengwei", "AB"),
    ("凤焰不息", "power_fengyanbuxi", "AB"),
    ("浴火军旗", "power_yuhuobanner", "AB"),
    ("不堕", "power_buduo", "AB"),
    ("交辉", "power_jiaohui", "AB"),
    ("登基", "power_dengji", "AB"),
    ("不死凤躯", "power_busifengqu", "AB"),
]
CELL = 300
LABEL_H = 40
ROW_LABEL_W = 170
PAD = 12
COLS = 4

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
    for line in RESULTS.read_text(encoding="utf-8").splitlines():
        if not line.strip():
            continue
        r = json.loads(line)
        records[r["tag"]] = r

    RAW_DIR.mkdir(parents=True, exist_ok=True)
    OUT.parent.mkdir(parents=True, exist_ok=True)
    font = ImageFont.truetype("C:/Windows/Fonts/msyh.ttc", 22)

    W = ROW_LABEL_W + COLS * CELL + (COLS + 1) * PAD
    H = len(GROUPS) * (CELL + LABEL_H) + (len(GROUPS) + 1) * PAD
    sheet = Image.new("RGB", (W, H), (45, 45, 52))
    draw = ImageDraw.Draw(sheet)

    missing = []
    for row, (name, prefix, versions) in enumerate(GROUPS):
        y = PAD + row * (CELL + LABEL_H + PAD)
        draw.text((PAD, y + CELL // 2 - 14), name, font=font, fill=(235, 235, 235))
        for col in range(COLS):
            x = ROW_LABEL_W + PAD + col * (CELL + PAD)
            if col >= len(versions):
                draw.rectangle([x, y, x + CELL, y + CELL], fill=(60, 60, 66))
                continue
            ab = versions[col]
            tag = f"{prefix}_{ab}"
            r = records.get(tag)
            ok = r and r.get("ok") and r.get("outFile") and Path(r["outFile"]).exists()
            if ok:
                src = Path(r["outFile"])
                dst = RAW_DIR / f"{tag}_v01{src.suffix.lower()}"
                if not dst.exists():
                    shutil.copy2(src, dst)
                img = Image.open(dst).convert("RGBA")
                cell = checker(CELL).convert("RGBA")
                img.thumbnail((CELL, CELL), Image.LANCZOS)
                cell.paste(img, ((CELL - img.width) // 2, (CELL - img.height) // 2), img)
                sheet.paste(cell.convert("RGB"), (x, y))
                label = ab
            else:
                draw.rectangle([x, y, x + CELL, y + CELL], fill=(90, 40, 40))
                label = f"{ab} 缺失"
                missing.append(tag)
            draw.text((x + 8, y + CELL + 6), label, font=font, fill=(235, 235, 235))

    sheet.save(OUT)
    print(f"sheet: {OUT} ({W}x{H})")
    print(f"raw 归档: {RAW_DIR}")
    if missing:
        print("缺失/失败:", ", ".join(missing))
        return 1
    print("26 张全部就位")
    return 0

if __name__ == "__main__":
    sys.exit(main())
