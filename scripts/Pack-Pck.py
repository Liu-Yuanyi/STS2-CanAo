#!/usr/bin/env python3
"""Packs the mod's Godot resources into a Godot 4.x PCK (format version 3),
byte-compatible with godotpcktool's output. Replacement for the external
godotpcktool.exe binary.

Layout (mirrors the tool-generated pack):
  header:   magic u32, packver u32=3, major u32=4, minor u32=5, patch u32=0,
            flags u32=2, file_base u64, dir_offset u64   (40 bytes)
  reserved: 64 zero bytes
  directory at dir_offset:
            u32 count, then per file:
            u32 padded_path_len, padded path,
            u64 data_offset (relative to file_base), u64 size,
            16-byte content md5, u32 flags=0
  padding to file_base (16-byte aligned)
  file data concatenated at file_base
"""

import hashlib
import os
import struct
import sys

MAGIC = 0x43504447
ROOTS = ("godot/CanAoNative", "godot/scenes", "godot/images", "godot/materials", "godot/.godot/imported")
REMOVE_PREFIX = "godot/"


def pad4(n: int) -> int:
    return (n + 3) & ~3


def pad16(n: int) -> int:
    return (n + 15) & ~15


def collect_files() -> list[tuple[str, bytes]]:
    collected: list[tuple[str, bytes]] = []
    for root in ROOTS:
        if not os.path.isdir(root):
            continue
        for dirpath, _, names in os.walk(root):
            for name in names:
                full = os.path.join(dirpath, name)
                rel = os.path.relpath(full, "godot").replace(os.sep, "/")
                res_path = "res://" + rel
                with open(full, "rb") as handle:
                    collected.append((res_path, handle.read()))
    collected.sort(key=lambda item: item[0])
    return collected


def build_pck(out_path: str) -> None:
    files = collect_files()

    directory = bytearray()
    directory += struct.pack("<I", len(files))

    offsets: list[int] = []
    data_size = 0
    for _, content in files:
        offsets.append(data_size)
        data_size += len(content)

    for (res_path, content), offset in zip(files, offsets):
        raw_path = res_path.encode("utf-8")
        padded_len = pad4(len(raw_path))
        directory += struct.pack("<I", padded_len)
        directory += raw_path.ljust(padded_len, b"\x00")
        directory += struct.pack("<2Q", offset, len(content))
        directory += hashlib.md5(content).digest()
        directory += struct.pack("<I", 0)

    dir_offset = 40 + 64
    file_base = pad16(dir_offset + len(directory))

    header = struct.pack(
        "<6I2Q",
        MAGIC,
        3,      # pack format version
        4,      # godot major
        5,      # godot minor
        0,      # godot patch
        2,      # flags
        file_base,
        dir_offset,
    )

    with open(out_path, "wb") as out:
        out.write(header)
        out.write(b"\x00" * 64)
        out.write(directory)
        out.write(b"\x00" * (file_base - dir_offset - len(directory)))
        for _, content in files:
            out.write(content)


if __name__ == "__main__":
    if len(sys.argv) != 2:
        raise SystemExit("usage: Pack-Pck.py <output.pck>")
    os.makedirs(os.path.dirname(sys.argv[1]), exist_ok=True)
    build_pck(sys.argv[1])
    print(f"Packed {len(collect_files())} files into {sys.argv[1]}")
