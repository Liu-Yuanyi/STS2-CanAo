# finalize_vfx.py — VFX 母版 → 三帧 alpha 渐变（100%/45%/15%）→ godot/images/vfx/can_ao/。
# 归档 A/B 原始母版到 art/vfx/raw/。当前实装 A 版（用户 2026-08-16 指示）。
# 用法: python scripts/finalize_vfx.py [batch81_results.jsonl]
import json
import shutil
from pathlib import Path

from PIL import Image

ROOT = Path(__file__).resolve().parent.parent
BOT = ROOT.parent / "aizex-image-bot"
RESULTS = BOT / "batch81_jobs_results.jsonl"
RAW = ROOT / "art" / "vfx" / "raw"
DST = ROOT / "godot" / "images" / "vfx" / "can_ao"

# 名称 → (目标尺寸, 方形?)
VFX = {
    "slash": 512, "heavy": 512, "aoe": 512,
    "luanflame": 512, "phoenixblaze": 512, "starburst": 512,
    "moonslash": (1024, 512), "starmoon": 512, "edictseal": 512,
    "buffup": 256, "debuff": 256, "exhaust": 256, "gain": 256,
    "stanceswap": 256, "summon": 256,
}

def load_master(src: Path, wide: bool) -> Image.Image:
    img = Image.open(src).convert("RGBA")
    bbox = img.getchannel("A").getbbox()
    if bbox:
        img = img.crop(bbox)
    w, h = img.size
    if wide:
        tw, th = 2048, 1024
        scale = min(tw / w, th / h)
        canvas = Image.new("RGBA", (tw, th), (0, 0, 0, 0))
        img = img.resize((int(w * scale), int(h * scale)), Image.LANCZOS)
        canvas.paste(img, ((tw - img.width) // 2, (th - img.height) // 2), img)
        return canvas
    side = max(w, h)
    canvas = Image.new("RGBA", (side, side), (0, 0, 0, 0))
    canvas.paste(img, ((side - w) // 2, (side - h) // 2), img)
    return canvas

def alpha_ramp(img: Image.Image, factor: float) -> Image.Image:
    out = img.copy()
    a = out.getchannel("A").point(lambda v: int(v * factor))
    out.putalpha(a)
    return out

def main() -> int:
    records = {}
    for line in RESULTS.read_text(encoding="utf-8").splitlines():
        if line.strip():
            r = json.loads(line)
            records[r["tag"]] = r

    RAW.mkdir(parents=True, exist_ok=True)
    DST.mkdir(parents=True, exist_ok=True)
    missing = []
    for name, size in VFX.items():
        wide = isinstance(size, tuple)
        for ab in "AB":
            tag = f"vfx_{name}_{ab}"
            r = records.get(tag)
            if r and r.get("ok") and r.get("outFile") and Path(r["outFile"]).exists():
                dst = RAW / f"{tag}.png"
                if not dst.exists():
                    shutil.copy2(r["outFile"], dst)
            else:
                missing.append(tag)

        # 实装 A 版
        src = RAW / f"vfx_{name}_A.png"
        if not src.exists():
            missing.append(f"vfx_{name}_A(归档缺失)")
            continue
        master = load_master(src, wide)
        if wide:
            master = master.resize(size, Image.LANCZOS)
        else:
            master = master.resize((size, size), Image.LANCZOS)
        for i, fct in enumerate((1.0, 0.45, 0.15)):
            alpha_ramp(master, fct).save(DST / f"{name}_f{i}.png")
        print("OK", name)

    if missing:
        print("缺失:", ", ".join(missing))
        return 1
    print("finalize_vfx 完成（A 版实装）")
    return 0

if __name__ == "__main__":
    raise SystemExit(main())
