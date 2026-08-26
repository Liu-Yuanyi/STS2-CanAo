# update_verify_hashes.py — 一次性：v12 改动后重算冻结哈希并写回 Verify-R11.ps1
import hashlib
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
SRC = ROOT / "src" / "CanAoNative"

TOUCHED = [
    "Cards\\FeatherRanksCard.cs",
    "Cards\\ShiWeiCard.cs",
    "Cards\\EdictCard.cs",
    "Cards\\StarMoonStrike.cs",
    "Cards\\ZanBiFengMangCard.cs",
    "Cards\\TianFengXingTaiCard.cs",
    "Rules\\YuHuo\\YuHuoResolver.cs",
    "Powers\\FengHuoJunXiePower.cs",
]

def norm_hash(rel: str) -> str:
    text = (SRC / rel.replace("\\", "/")).read_text(encoding="utf-8")
    text = text.replace("\r\n", "\n").replace("\r", "\n")
    return hashlib.sha256(text.encode("utf-8")).hexdigest()

def main() -> int:
    ps1 = ROOT / "scripts" / "Verify-R11.ps1"
    t = ps1.read_text(encoding="utf-8")
    n = 0
    for rel in TOUCHED:
        new = norm_hash(rel)
        pat = re.compile(r'    "' + re.escape(rel) + r'" = "[0-9a-f]{64}"\n')
        t, cnt = pat.subn(lambda m: f'    "{rel}" = "{new}"\n', t)
        if cnt == 0:
            print(f"-- 不在冻结表，跳过: {rel}")
            continue
        n += 1
        print(f"{rel} -> {new[:12]}…")

    note = (
        "    # 2026-08-15 intentional change (v12 修改.md): 数值/关键词/重做涉及冻结类——\n"
        "    # FeatherRanksCard(9/12+保留)、SacrificialPreparationCard(删力量)、\n"
        "    # ZanBiFengMangCard(永久-1凤威)、DiGuoYuWeiPower(第一次+改名帝国威严)、\n"
        "    # TianFengJunYinRelic(3)、QingLuanYuYiRelic(阈值3)、ZhanBeiRelic(3)、\n"
        "    # GuWangYuZuoRelic(抽牌替代能量)、DiGuoShuiQiRelic(重做:战斗结束结算金币)；\n"
        "    # 机制均为 v12 用户逐项拍板，冻结哈希同步更新。\n"
    )
    anchor = "    # ChengTianShouMingCard removed — v12 删除（弃稿）\n"
    if anchor not in t:
        print("!! 注释锚点未找到")
        return 1
    if "数值/关键词/重做涉及冻结类" not in t:
        t = t.replace(anchor, anchor + note, 1)
    ps1.write_text(t, encoding="utf-8", newline="\n")
    print(f"更新哈希 {n} 条 + 注释完成")
    return 0

if __name__ == "__main__":
    raise SystemExit(main())
