# make_sheet_v06.py — batch78（药水 3×4）与 batch79（手势 4 张，含原版对比）拼版。
# 用法: python scripts/make_sheet_v06.py
import json
import shutil
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont

ROOT = Path(__file__).resolve().parent.parent
BOT = ROOT.parent / "aizex-image-bot"
REFS = ROOT.parent / "sts2_assets" / "refs_v12"
CELL = 300
LABEL_H = 40
PAD = 12
FONT = "C:/Windows/Fonts/msyh.ttc"

POTIONS = [
    ("琼浆", "potion_qiongjiang"),
    ("凤威酒", "potion_fengweijiu"),
    ("御令瓶", "potion_yulingping"),
]
HANDS = ["rock", "scissors", "paper", "point"]

def checker(size: int, sq: int = 20) -> Image.Image:
    img = Image.new("RGB", (size, size), (200, 200, 200))
    d = ImageDraw.Draw(img)
    for y in range(0, size, sq):
        for x in range(0, size, sq):
            if (x // sq + y // sq) % 2 == 0:
                d.rectangle([x, y, x + sq - 1, y + sq - 1], fill=(120, 120, 120))
    return img

def load_records(name: str) -> dict:
    records = {}
    for line in (BOT / name).read_text(encoding="utf-8").splitlines():
        if line.strip():
            r = json.loads(line)
            records[r["tag"]] = r
    return records

def paste(sheet, draw, img_path: Path | None, x, y, w, h, label, font):
    if img_path and img_path.exists():
        cell = Image.new("RGBA", (w, h), (0, 0, 0, 0))
        bg = checker(w).resize((w, h))
        cell = bg.convert("RGBA")
        img = Image.open(img_path).convert("RGBA")
        img.thumbnail((w, h), Image.LANCZOS)
        cell.paste(img, ((w - img.width) // 2, (h - img.height) // 2), img)
        sheet.paste(cell.convert("RGB"), (x, y))
    else:
        draw.rectangle([x, y, x + w, y + h], fill=(90, 40, 40))
        label += " 缺失"
    draw.text((x + 8, y + h + 4), label, font=font, fill=(235, 235, 235))

def main() -> int:
    font = ImageFont.truetype(FONT, 22)
    missing = []

    # 药水 3×4
    rec = load_records("batch78_jobs_results.jsonl")
    raw = ROOT / "art" / "potions" / "raw"
    raw.mkdir(parents=True, exist_ok=True)
    W = 4 * CELL + 5 * PAD
    H = 3 * (CELL + LABEL_H) + 4 * PAD
    sheet = Image.new("RGB", (W, H), (45, 45, 52))
    draw = ImageDraw.Draw(sheet)
    for row, (name, prefix) in enumerate(POTIONS):
        y = PAD + row * (CELL + LABEL_H + PAD)
        for col, ab in enumerate("ABCD"):
            tag = f"{prefix}_{ab}"
            r = rec.get(tag)
            dst = None
            if r and r.get("ok") and Path(r["outFile"]).exists():
                src = Path(r["outFile"])
                dst = raw / f"{tag}_v01{src.suffix.lower()}"
                if not dst.exists():
                    shutil.copy2(src, dst)
            else:
                missing.append(tag)
            paste(sheet, draw, dst, PAD + col * (CELL + PAD), y, CELL, CELL, f"{name} {ab}", font)
    out1 = ROOT / "art" / "potions" / "potion_sheet_v01.png"
    sheet.save(out1)
    print("sheet1:", out1)

    # 手势：原版 + AI 风格化，2×4
    rec79 = load_records("batch79_jobs_results.jsonl")
    rawh = ROOT / "art" / "hands" / "raw"
    rawh.mkdir(parents=True, exist_ok=True)
    hw, hh = 140, 400  # 422×1200 缩比
    W = 4 * (hw + PAD) + PAD
    H = 2 * (hh + LABEL_H) + 3 * PAD
    sheet = Image.new("RGB", (W, H), (45, 45, 52))
    draw = ImageDraw.Draw(sheet)
    for col, g in enumerate(HANDS):
        x = PAD + col * (hw + PAD)
        paste(sheet, draw, REFS / f"multiplayer_hand_ironclad_{g}.png", x, PAD, hw, hh, f"{g} 原版", font)
        tag = f"hand_{g}_A"
        r = rec79.get(tag)
        dst = None
        if r and r.get("ok") and Path(r["outFile"]).exists():
            src = Path(r["outFile"])
            dst = rawh / f"{tag}_v01{src.suffix.lower()}"
            if not dst.exists():
                shutil.copy2(src, dst)
        else:
            missing.append(tag)
        paste(sheet, draw, dst, x, PAD + hh + LABEL_H + PAD, hw, hh, f"{g} 风格化", font)
    out2 = ROOT / "art" / "hands" / "hand_sheet_v01.png"
    sheet.save(out2)
    print("sheet2:", out2)

    if missing:
        print("缺失:", ", ".join(missing))
        return 1
    print("全部就位")
    return 0

if __name__ == "__main__":
    raise SystemExit(main())
