# finalize_icons.py — 把已定稿的遗物/Power 母版裁方、派生尺寸，写入 godot/images/。
# 遗物三件套：85×85 主图(small) / 85×85 高对比轮廓(outline) / 256×256 大图(根目录)。
# Power 两件套：64×64(small) / 256×256(根目录)。
# 另生成 4 张程序派生图（降饱和/金相）与 derived_sheet.png 供用户过目。
# 用法: python scripts/finalize_icons.py
from pathlib import Path

from PIL import Image, ImageEnhance, ImageFilter

ROOT = Path(__file__).resolve().parent.parent  # CanAoNative/
ART = ROOT / "art"
GODOT_IMG = ROOT / "godot" / "images"

# ---- 遗物：id → 源母版（用户 2026-08-10 拍板） ----
RELICS = {
    "di_guo_nian_biao_relic": ART / "relics" / "raw" / "relic_帝国史册_A_v01.png",  # 互换：年表用史册A图
    "di_guo_shi_ce_relic": ART / "relics" / "raw" / "relic_帝国年表_A_v01.png",      # 互换：史册用年表A图
    "tian_feng_jun_yin_relic": ART / "relics" / "raw" / "relic_天凤军印_A_v01.png",
    "qing_luan_yu_yi_relic": ART / "relics" / "raw" / "relic_青鸾羽衣_B_v01.png",
    "he_ji_wu_dian_relic": ART / "relics" / "raw" / "relic_合击武典_B_v01.png",
    "nie_pan_huo_zhong_relic": ART / "relics" / "raw" / "relic_涅槃火种_A_v01.png",
    "zhan_bei_relic": ART / "manual" / "战碑.png",
    "gu_wang_yu_zuo_relic": ART / "manual" / "孤王玉座.png",
    "di_guo_shui_qi_relic": ART / "relics" / "raw" / "relic_帝国税契_B_v01.png",
}

# ---- Power：id → 源母版（同上拍板） ----
POWERS = {
    "star_power": ART / "manual" / "星.png",
    "moon_power": ART / "manual" / "月.png",
    "next_turn_star_power": ART / "manual" / "下回合星.png",
    "feng_wei_power": ART / "powers" / "raw" / "power_fengwei_v03_A.png",
    "feng_yan_bu_xi_power": ART / "powers" / "raw" / "power_fengyanbuxi_v02.png",
    "yu_huo_banner_power": ART / "powers" / "raw" / "power_yuhuobanner_B_v01.png",
    "bu_duo_power": ART / "powers" / "raw" / "power_buduo_A_v01.png",
    "jiao_hui_power": ART / "powers" / "raw" / "power_jiaohui_A_v01.png",
    "deng_ji_power": ART / "powers" / "raw" / "power_dengji_v03_D.png",
    "bu_si_feng_qu_power": ART / "powers" / "raw" / "power_busifengqu_B_v01.png",
    "xing_yue_wang_guan_power": ART / "powers" / "raw" / "power_xingyuewangguan_A_v01.png",
    "tian_feng_xing_tai_power": ART / "powers" / "raw" / "power_tianfengxingtai_B_v01.png",
    "gui_yin_yun_shan_power": ART / "powers" / "raw" / "power_guiyinyunshan_A_v01.png",
    "feng_hun_power": ART / "powers" / "raw" / "power_fenghun_B_v01.png",
    "shou_que_power": ART / "powers" / "raw" / "power_shouque_A_v01.png",
    "fu_bi_power": ART / "powers" / "raw" / "power_fubi_A_v01.png",
    "zhong_zhang_power": ART / "powers" / "raw" / "power_zhongzhang_A_v01.png",
    "wang_zuo_gu_ming_power": ART / "powers" / "raw" / "power_wangzuoguming_A_v01.png",
    "bu_mie_wang_chao_power": ART / "powers" / "raw" / "power_bumiewangchao_A_v01.png",
    "wan_bang_lai_chao_power": ART / "powers" / "raw" / "power_wanbanglaichao_A_v01.png",
    "pan_xuan_power": ART / "powers" / "raw" / "power_panxuan_A_v01.png",
    "tian_feng_jun_zhen_power": ART / "powers" / "raw" / "power_tianfengjunzhen_A_v01.png",
    "di_guo_yu_wei_power": ART / "powers" / "raw" / "power_diguoyuwei_A_v01.png",
    "zui_hou_yi_wu_power": ART / "powers" / "raw" / "power_zuihouyiwu_A_v01.png",
    "feng_huo_jun_xie_power": ART / "powers" / "raw" / "power_fenghuojunxie_A_v01.png",
    "wa_mo_yuan_zheng_power": ART / "powers" / "raw" / "power_wamoyuanzheng_A_v01.png",
    "cui_huo_temporary_strength_power": ART / "powers" / "raw" / "power_cuihuo_A_v01.png",  # 用户选定 batch77 A
    "xing_yue_lun_zhuan_power": ART / "powers" / "raw" / "power_xingyuelunzhuan_B_v01.png",  # 用户选定 batch80 B
}

# ---- 程序派生（无 AI）：目标 id → (来源 id, 处理) ----
DERIVED = {
    "temporary_feng_wei_power": ("feng_wei_power", "desat"),
    "feng_huo_jun_xie_upgraded_power": ("feng_huo_jun_xie_power", "gold"),
    "yu_huo_banner_temporary_strength_power": ("yu_huo_banner_power", "desat"),
}

DERIVED_LABELS = {
    "temporary_feng_wei_power": "临时凤威 ←凤威降饱和",
    "feng_huo_jun_xie_upgraded_power": "军械升级版 ←军械金相",
    "yu_huo_banner_temporary_strength_power": "军旗临时力量 ←军旗降饱和",
    "cui_huo_temporary_strength_power": "淬火临时力量 ←军械降饱和",
}

def load_square_master(src: Path, margin: float = 0.04) -> Image.Image:
    """裁透明边 → 补成方形 → 1024×1024 母版。"""
    img = Image.open(src).convert("RGBA")
    bbox = img.getchannel("A").getbbox()
    if bbox:
        img = img.crop(bbox)
    w, h = img.size
    side = max(w, h)
    side = int(side * (1 + margin * 2))
    canvas = Image.new("RGBA", (side, side), (0, 0, 0, 0))
    canvas.paste(img, ((side - w) // 2, (side - h) // 2), img)
    return canvas.resize((1024, 1024), Image.LANCZOS)

def desaturate(img: Image.Image, amount: float = 0.2) -> Image.Image:
    r, g, b, a = img.split()
    rgb = Image.merge("RGB", (r, g, b))
    rgb = ImageEnhance.Color(rgb).enhance(amount)
    rgb = ImageEnhance.Brightness(rgb).enhance(0.85)
    out = rgb.convert("RGBA")
    out.putalpha(a)
    return out

def gold_shift(img: Image.Image) -> Image.Image:
    """向星金方向整体偏移，表现'升级版'。"""
    r, g, b, a = img.split()
    rgb = Image.merge("RGB", (r, g, b)).convert("RGBA")
    gold = Image.new("RGBA", img.size, (232, 193, 90, 255))
    out = Image.blend(rgb, gold, 0.30)
    out.putalpha(a)
    return out

def make_outline(small: Image.Image) -> Image.Image:
    """85×85 主图 → 白色高对比轮廓（alpha 膨胀 − 原 alpha 的环形）。"""
    from PIL import ImageChops
    a = small.getchannel("A")
    dilated = a.filter(ImageFilter.MaxFilter(7))
    ring = ImageChops.subtract(dilated, a)
    transparent = Image.new("RGBA", small.size, (255, 255, 255, 0))
    white = Image.new("RGBA", small.size, (255, 255, 255, 255))
    return Image.composite(white, transparent, ring)

def save(img: Image.Image, path: Path, size: int) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    img.resize((size, size), Image.LANCZOS).save(path)

def main() -> int:
    processed_relics = ART / "relics" / "processed"
    processed_powers = ART / "powers" / "processed"
    processed_relics.mkdir(parents=True, exist_ok=True)
    processed_powers.mkdir(parents=True, exist_ok=True)

    masters: dict[str, Image.Image] = {}
    for group, table, proc_dir in (
        ("relic", RELICS, processed_relics),
        ("power", POWERS, processed_powers),
    ):
        for rid, src in table.items():
            if not src.exists():
                print(f"!! 缺源文件: {rid} <- {src}")
                return 1
            master = load_square_master(src)
            master.save(proc_dir / f"{rid}_1024.png")
            masters[rid] = master
            print(f"母版 {rid} <- {src.name}")

    # 遗物三件套
    for rid, master in ((r, masters[r]) for r in RELICS):
        save(master, GODOT_IMG / "relics" / f"{rid}.png", 256)
        small = master.resize((85, 85), Image.LANCZOS)
        small_path = GODOT_IMG / "relics" / "small" / f"{rid}.png"
        small_path.parent.mkdir(parents=True, exist_ok=True)
        small.save(small_path)
        outline_path = GODOT_IMG / "relics" / "outline" / f"{rid}.png"
        outline_path.parent.mkdir(parents=True, exist_ok=True)
        make_outline(small).save(outline_path)

    # Power 两件套（含派生）
    derived_masters: dict[str, Image.Image] = {}
    for rid, (src_id, how) in DERIVED.items():
        base = masters[src_id]
        dm = desaturate(base) if how == "desat" else gold_shift(base)
        dm.save(processed_powers / f"{rid}_1024.png")
        masters[rid] = dm
        derived_masters[rid] = dm
        print(f"派生 {rid} <- {src_id} ({how})")

    for pid in list(POWERS) + list(DERIVED):
        master = masters[pid]
        save(master, GODOT_IMG / "powers" / f"{pid}.png", 256)
        save(master, GODOT_IMG / "powers" / "small" / f"{pid}.png", 64)

    # 派生图拼版（给用户瞥一眼）
    from PIL import ImageDraw, ImageFont
    cell, pad, label_h = 300, 16, 64
    W = 4 * cell + 5 * pad
    H = cell + label_h + 2 * pad
    sheet = Image.new("RGB", (W, H), (45, 45, 52))
    draw = ImageDraw.Draw(sheet)
    font = ImageFont.truetype("C:/Windows/Fonts/msyh.ttc", 18)
    for i, (rid, dm) in enumerate(derived_masters.items()):
        x = pad + i * (cell + pad)
        bg = Image.new("RGBA", (cell, cell), (70, 70, 78, 255))
        im = dm.resize((cell, cell), Image.LANCZOS)
        bg.paste(im, (0, 0), im)
        sheet.paste(bg.convert("RGB"), (x, pad))
        draw.text((x + 4, pad + cell + 6), DERIVED_LABELS[rid],
                  font=font, fill=(235, 235, 235))
        draw.text((x + 4, pad + cell + 32), rid, font=font,
                  fill=(170, 170, 170))
    sheet_path = ART / "powers" / "derived_sheet.png"
    sheet.save(sheet_path)
    print(f"派生拼版: {sheet_path}")
    print("finalize_icons 完成")
    return 0

if __name__ == "__main__":
    raise SystemExit(main())
