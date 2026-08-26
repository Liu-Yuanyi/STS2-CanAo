# make_relic_sheet.py — 读取 batch71_jobs_results.jsonl，把 9 件遗物 × A/B 两案拼成一张审查版。
# 用法: python scripts/make_relic_sheet.py [results.jsonl路径] [输出png路径]
# 只核文件/尺寸/任务对应，不做内容评审（管线 §5.6 信任通道）。
import json
import shutil
import sys
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont

ROOT = Path(__file__).resolve().parent.parent  # CanAoNative/
BOT = ROOT.parent / "aizex-image-bot"
RESULTS = Path(sys.argv[1]) if len(sys.argv) > 1 else BOT / "batch71_jobs_results.jsonl"
OUT = Path(sys.argv[2]) if len(sys.argv) > 2 else ROOT / "art" / "relics" / "relic_sheet_v01.png"
RAW_DIR = ROOT / "art" / "relics" / "raw"

RELICS = ["帝国年表", "帝国史册", "天凤军印", "青鸾羽衣", "合击武典", "涅槃火种", "战碑", "孤王玉座", "帝国税契"]
CELL = 384
LABEL_H = 44
PAD = 16
COLS = 2

def checker(size: int, sq: int = 24) -> Image.Image:
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
        records[r["tag"]] = r  # 同名取最后一条（重跑覆盖）

    RAW_DIR.mkdir(parents=True, exist_ok=True)
    OUT.parent.mkdir(parents=True, exist_ok=True)
    font = ImageFont.truetype("C:/Windows/Fonts/msyh.ttc", 26)

    rows = len(RELICS)
    W = COLS * CELL + (COLS + 1) * PAD
    H = rows * (CELL + LABEL_H) + (rows + 1) * PAD
    sheet = Image.new("RGB", (W, H), (45, 45, 52))
    draw = ImageDraw.Draw(sheet)

    missing = []
    for row, name in enumerate(RELICS):
        for col, ab in enumerate(("A", "B")):
            tag = f"relic_{name}_{ab}"
            x = PAD + col * (CELL + PAD)
            y = PAD + row * (CELL + LABEL_H + PAD)
            r = records.get(tag)
            ok = r and r.get("ok") and r.get("outFile") and Path(r["outFile"]).exists()
            if ok:
                src = Path(r["outFile"])
                # 归档 raw 母版
                dst = RAW_DIR / f"relic_{name}_{ab}_v01{src.suffix.lower()}"
                if not dst.exists():
                    shutil.copy2(src, dst)
                img = Image.open(dst).convert("RGBA")
                cell = checker(CELL).convert("RGBA")
                img.thumbnail((CELL, CELL), Image.LANCZOS)
                cell.paste(img, ((CELL - img.width) // 2, (CELL - img.height) // 2), img)
                sheet.paste(cell.convert("RGB"), (x, y))
                label = f"{name} {ab}"
            else:
                draw.rectangle([x, y, x + CELL, y + CELL], fill=(90, 40, 40))
                label = f"{name} {ab} — 缺失/失败"
                missing.append(tag)
            draw.text((x + 8, y + CELL + 6), label, font=font, fill=(235, 235, 235))

    sheet.save(OUT)
    print(f"sheet: {OUT} ({W}x{H})")
    print(f"raw 归档: {RAW_DIR}")
    if missing:
        print("缺失/失败:", ", ".join(missing))
        return 1
    print("18 张全部就位")
    return 0

if __name__ == "__main__":
    sys.exit(main())
