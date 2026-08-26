# compose_hands.py — 多人手势合成器。
# 用部件库（art/hands/parts/）按配置拼出四张 422×1200 手势。
#
# 用法:
#   python scripts/compose_hands.py            # 按 compose_config.json 合成四张
# 配置格式（art/hands/compose_config.json）:
#   {
#     "rock": [ {"part": "base_fist"} ],
#     "scissors": [
#       {"part": "base_fist"},
#       {"part": "finger_index", "at": [200, 240], "rot": -12},
#       {"part": "finger_middle", "at": [240, 235], "rot": 8}
#     ]
#   }
# 每层字段: part=部件名(不带.png)；at=把部件枢轴放到画布的 [x,y]（省略=原位）；
#           rot=绕枢轴逆时针角度；scale=缩放倍数；flip=true 水平镜像。
# 手指部件枢轴 = 指根；base 部件枢轴 = 画布中心。
# 输出: art/hands/composed/hand_<gesture>.png + 四宫格预览 preview.png。
import json
from pathlib import Path

from PIL import Image

ROOT = Path(__file__).resolve().parent.parent
PARTS = ROOT / "art" / "hands" / "parts"
OUT = ROOT / "art" / "hands" / "composed"
CANVAS = (422, 1200)
GESTURES = ("rock", "scissors", "paper", "point")

def load_part(name: str) -> tuple[Image.Image, list[int]]:
    img = Image.open(PARTS / f"{name}.png").convert("RGBA")
    meta = json.load(open(PARTS / "parts_meta.json", encoding="utf-8"))
    return img, meta[name]["pivot"]

def compose(layers: list[dict]) -> Image.Image:
    canvas = Image.new("RGBA", CANVAS, (0, 0, 0, 0))
    for layer in layers:
        img, pivot = load_part(layer["part"])
        pivot = list(pivot)
        if layer.get("flip"):
            img = img.transpose(Image.FLIP_LEFT_RIGHT)
            pivot[0] = img.width - pivot[0]
        if "scale" in layer:
            s = layer["scale"]
            img = img.resize((max(1, int(img.width * s)), max(1, int(img.height * s))), Image.LANCZOS)
            pivot = [int(pivot[0] * s), int(pivot[1] * s)]
        if layer.get("rot"):
            # 不 expand，绕枢轴旋转，枢轴位置保持不变
            img = img.rotate(layer["rot"], expand=False, resample=Image.BICUBIC, center=pivot)
        if "at" in layer:
            pos = (int(layer["at"][0] - pivot[0]), int(layer["at"][1] - pivot[1]))
        else:
            pos = ((CANVAS[0] - img.width) // 2, (CANVAS[1] - img.height) // 2)
        canvas.paste(img, pos, img)
    return canvas

def main() -> int:
    cfg = json.load(open(ROOT / "art" / "hands" / "compose_config.json", encoding="utf-8"))
    OUT.mkdir(parents=True, exist_ok=True)
    thumbs = []
    for g in GESTURES:
        if g not in cfg:
            print("跳过（无配置）:", g)
            continue
        img = compose(cfg[g])
        img.save(OUT / f"hand_{g}.png")
        t = img.copy()
        t.thumbnail((211, 600), Image.LANCZOS)
        thumbs.append((g, t))
        print("合成", g)
    # 预览四宫格
    W = 4 * 211 + 5 * 10
    H = 600 + 30
    sheet = Image.new("RGB", (W, H), (45, 45, 52))
    from PIL import ImageDraw, ImageFont
    draw = ImageDraw.Draw(sheet)
    font = ImageFont.truetype("C:/Windows/Fonts/msyh.ttc", 16)
    for i, (g, t) in enumerate(thumbs):
        x = 10 + i * 221
        sheet.paste(t, (x, 5), t)
        draw.text((x + 4, 610), g, font=font, fill=(235, 235, 235))
    sheet.save(OUT / "preview.png")
    print("预览:", OUT / "preview.png")
    return 0

if __name__ == "__main__":
    raise SystemExit(main())
