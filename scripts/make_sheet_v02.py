# make_sheet_v02.py — batch73（重做 7 张）与 batch74（追加 10 Power × A/B）的审查拼版。
# 用法: python scripts/make_sheet_v02.py
import json
import shutil
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont

ROOT = Path(__file__).resolve().parent.parent  # CanAoNative/
BOT = ROOT.parent / "aizex-image-bot"
FONT = "C:/Windows/Fonts/msyh.ttc"
CELL = 300
LABEL_H = 40
PAD = 12

REDO = [  # (显示名, tag, 归档目录)
    ("战碑 v02", "relic_战碑_v02", "relics"),
    ("孤王玉座 v02", "relic_孤王玉座_v02", "relics"),
    ("星 v02", "power_star_v02", "powers"),
    ("月 v02", "power_moon_v02", "powers"),
    ("凤威 v02", "power_fengwei_v02", "powers"),
    ("凤焰不息 v02", "power_fengyanbuxi_v02", "powers"),
    ("登基 v02", "power_dengji_v02", "powers"),
]
NEW10 = [  # (显示名, tag 前缀)
    ("星月王冠", "power_xingyuewangguan"),
    ("天凤形态", "power_tianfengxingtai"),
    ("归隐陨山", "power_guiyinyunshan"),
    ("凤魂", "power_fenghun"),
    ("守缺", "power_shouque"),
    ("复辟", "power_fubi"),
    ("终章", "power_zhongzhang"),
    ("王座孤明", "power_wangzuoguming"),
    ("不灭王朝", "power_bumiewangchao"),
    ("万邦来朝", "power_wanbanglaichao"),
]

def checker(size: int, sq: int = 20) -> Image.Image:
    img = Image.new("RGB", (size, size), (200, 200, 200))
    d = ImageDraw.Draw(img)
    for y in range(0, size, sq):
        for x in range(0, size, sq):
            if (x // sq + y // sq) % 2 == 0:
                d.rectangle([x, y, x + sq - 1, y + sq - 1], fill=(120, 120, 120))
    return img

def load_records(path: Path) -> dict:
    records = {}
    for line in path.read_text(encoding="utf-8").splitlines():
        if line.strip():
            r = json.loads(line)
            records[r["tag"]] = r
    return records

def archive(records: dict, tag: str, raw_dir: Path, name: str) -> Path | None:
    r = records.get(tag)
    if not (r and r.get("ok") and r.get("outFile") and Path(r["outFile"]).exists()):
        return None
    src = Path(r["outFile"])
    dst = raw_dir / f"{name}{src.suffix.lower()}"
    if not dst.exists():
        shutil.copy2(src, dst)
    return dst

def paste_cell(sheet, draw, path: Path | None, x: int, y: int, label: str, font):
    if path:
        img = Image.open(path).convert("RGBA")
        cell = checker(CELL).convert("RGBA")
        img.thumbnail((CELL, CELL), Image.LANCZOS)
        cell.paste(img, ((CELL - img.width) // 2, (CELL - img.height) // 2), img)
        sheet.paste(cell.convert("RGB"), (x, y))
    else:
        draw.rectangle([x, y, x + CELL, y + CELL], fill=(90, 40, 40))
        label += " 缺失"
    draw.text((x + 8, y + CELL + 6), label, font=font, fill=(235, 235, 235))

def main() -> int:
    font = ImageFont.truetype(FONT, 22)
    missing = []

    # 重做 7 张：一列单图
    rec73 = load_records(BOT / "batch73_jobs_results.jsonl")
    W = CELL + 2 * PAD
    H = len(REDO) * (CELL + LABEL_H) + (len(REDO) + 1) * PAD
    sheet = Image.new("RGB", (W, H), (45, 45, 52))
    draw = ImageDraw.Draw(sheet)
    for row, (name, tag, group) in enumerate(REDO):
        raw_dir = ROOT / "art" / group / "raw"
        raw_dir.mkdir(parents=True, exist_ok=True)
        dst = archive(rec73, tag, raw_dir, tag)
        if dst is None:
            missing.append(tag)
        paste_cell(sheet, draw, dst, PAD, PAD + row * (CELL + LABEL_H + PAD), name, font)
    out1 = ROOT / "art" / "redo_sheet_v02.png"
    sheet.save(out1)
    print(f"sheet1: {out1} ({W}x{H})")

    # 追加 10 × A/B
    rec74 = load_records(BOT / "batch74_jobs_results.jsonl")
    raw_dir = ROOT / "art" / "powers" / "raw"
    raw_dir.mkdir(parents=True, exist_ok=True)
    W = 2 * CELL + 3 * PAD
    H = len(NEW10) * (CELL + LABEL_H) + (len(NEW10) + 1) * PAD
    sheet = Image.new("RGB", (W, H), (45, 45, 52))
    draw = ImageDraw.Draw(sheet)
    for row, (name, prefix) in enumerate(NEW10):
        y = PAD + row * (CELL + LABEL_H + PAD)
        for col, ab in enumerate(("A", "B")):
            tag = f"{prefix}_{ab}"
            dst = archive(rec74, tag, raw_dir, f"{tag}_v01")
            if dst is None:
                missing.append(tag)
            paste_cell(sheet, draw, dst, PAD + col * (CELL + PAD), y, f"{name} {ab}", font)
    out2 = ROOT / "art" / "powers" / "power_sheet_v02.png"
    sheet.save(out2)
    print(f"sheet2: {out2} ({W}x{H})")

    if missing:
        print("缺失:", ", ".join(missing))
        return 1
    print("27 张全部就位")
    return 0

if __name__ == "__main__":
    raise SystemExit(main())
