# make_sheet_batch62.py — 批62（按剑/星月轮转 各A/B）25:19 裁切预览拼版。
# 用法: python scripts/make_sheet_batch62.py
import json
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont

ROOT = Path(__file__).resolve().parent.parent
BOT = ROOT.parent / "aizex-image-bot"
OUT = BOT / "debug" / "batch62_sheet.png"
CELL_W, CELL_H = 500, 380  # 25:19
LABEL_H = 40
PAD = 12
FONT = "C:/Windows/Fonts/msyh.ttc"

CARDS = [
    ("按剑 an_jian", ["anjian_v01_A", "anjian_v01_B"]),
    ("星月轮转 xing_yue_lun_zhuan", ["xingyuelunzhuan_v01_A", "xingyuelunzhuan_v01_B"]),
]


def load_records():
    records = {}
    for line in (BOT / "batch62_jobs_results.jsonl").read_text(encoding="utf-8").splitlines():
        if line.strip():
            r = json.loads(line)
            records[r["tag"]] = r
    return records


def cell(path: Path) -> Image.Image:
    im = Image.open(path).convert("RGB")
    w, h = im.size
    tw = int(h * 25 / 19)
    x0 = max(0, (w - tw) // 2)
    im = im.crop((x0, 0, x0 + min(tw, w), h)).resize((CELL_W, CELL_H), Image.LANCZOS)
    return im


def main():
    records = load_records()
    rows, cols = len(CARDS), 2
    W = PAD + cols * (CELL_W + PAD)
    H = PAD + rows * (CELL_H + LABEL_H + PAD)
    sheet = Image.new("RGB", (W, H), (30, 30, 34))
    d = ImageDraw.Draw(sheet)
    font = ImageFont.truetype(FONT, 24)
    missing = []
    for r, (card_name, tags) in enumerate(CARDS):
        for c, tag in enumerate(tags):
            x = PAD + c * (CELL_W + PAD)
            y = PAD + r * (CELL_H + LABEL_H + PAD)
            rec = records.get(tag)
            if rec and rec.get("ok") and Path(rec["outFile"]).exists():
                sheet.paste(cell(Path(rec["outFile"])), (x, y))
            else:
                missing.append(tag)
                d.rectangle([x, y, x + CELL_W, y + CELL_H], outline=(200, 60, 60), width=3)
            d.text((x + 6, y + CELL_H + 6), f"{card_name}  {'A' if c == 0 else 'B'}", font=font, fill=(240, 240, 240))
    OUT.parent.mkdir(parents=True, exist_ok=True)
    sheet.save(OUT)
    print(f"saved: {OUT}")
    if missing:
        print("MISSING:", ", ".join(missing))


if __name__ == "__main__":
    main()
