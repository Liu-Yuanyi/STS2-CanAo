#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""卡图裁切实装工具：生图原图 → 25:19 居中裁切 → godot/images/card_portraits/canao/<card_id>.png

用法:
    python scripts/crop_card_art.py <src.png> <card_id> [<src.png> <card_id> ...]

例:
    python scripts/crop_card_art.py aizex-output.png yue_zhan

- 输出宽度 = 高度 × 25/19，居中裁切（上下不裁，左右对称裁）。
- 不缩放，保持源高度（aizex 1448×1086 → 1429×1086，与既有 16 张一致）。
- 同时在 art/cards/processed/ 留一份同名副本（交付尺寸归档）。
- 之后仍需: 卡牌类 PortraitPath 指向 res:// 路径 → Godot --import → Deploy-Mod.ps1。
"""
import os
import sys

from PIL import Image

REPO = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
GODOT_DIR = os.path.join(REPO, "godot", "images", "card_portraits", "canao")
ARCHIVE_DIR = os.path.join(REPO, "art", "cards", "processed")


def crop_one(src_path: str, card_id: str) -> None:
    im = Image.open(src_path).convert("RGB")
    w, h = im.size
    target_w = round(h * 25 / 19)
    if target_w > w:
        raise SystemExit(f"{src_path}: 宽度 {w} 不足以按 25:19 裁切（需要 {target_w}）")
    left = (w - target_w) // 2
    cropped = im.crop((left, 0, left + target_w, h))

    os.makedirs(GODOT_DIR, exist_ok=True)
    os.makedirs(ARCHIVE_DIR, exist_ok=True)
    for out_dir in (GODOT_DIR, ARCHIVE_DIR):
        out_path = os.path.join(out_dir, f"{card_id}.png")
        cropped.save(out_path)
        print(f"写入 {out_path} ({cropped.size[0]}x{cropped.size[1]})")


def main() -> None:
    args = sys.argv[1:]
    if len(args) < 2 or len(args) % 2 != 0:
        raise SystemExit(__doc__)
    for i in range(0, len(args), 2):
        crop_one(args[i], args[i + 1])


if __name__ == "__main__":
    main()
