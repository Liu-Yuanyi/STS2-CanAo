# Repository context (AI review copy)

This is a reduced copy of the CanAoNative repository for AI code review.
It was generated with:

```bash
git archive --format=zip HEAD -o repo-for-ai.zip
```

using `.gitattributes` `export-ignore` rules.

## Excluded

- PNG/JPG/WebP/GIF/PSD/TGA/BMP/EXR/HDR images
- WAV/MP3/OGG/FLAC audio
- MP4/MOV video
- FBX/Blend/GLB/GLTF 3D models
- ZIP/7Z/RAR/BIN archives and binaries

The full repository is approximately 1.3 GB on disk, almost entirely
card/relic/power/character art under `art/` and `godot/images/`.

## Included reference lists

- `FILE_TREE.txt` — full Git-tracked file list (all 1392 paths).
- `ASSET_LIST.txt` — the 671 excluded binary asset paths.

Use these to check whether a path referenced in code actually exists.

## Project orientation

- `src/CanAoNative/` — C# mod source for Slay the Spire 2 (STS2),
  zero-BaseLib native mod, character 残傲 (CanAo).
- `godot/CanAoNative/localization/` — zhs/eng text tables for cards,
  powers, relics, potions, characters, ancients.
- `scripts/` — build/deploy/verify PowerShell + asset-processing Python.
- `packaging/CanAoNative.json` — mod manifest.
- `docs/` — design documents, review records, per-release notes.
- `残傲.md` — top-level character design doc (Chinese).
- `CLAUDE.md` — local-AI working context for this repo.

## Regenerating the lists

```bash
git ls-files > FILE_TREE.txt
git ls-files \
  | grep -Ei '\.(png|jpe?g|webp|gif|psd|tga|bmp|exr|hdr|wav|mp3|ogg|flac|mp4|mov|fbx|blend|glb|gltf|zip|7z|rar|bin)$' \
  > ASSET_LIST.txt
```

Then re-run `git archive --format=zip HEAD -o repo-for-ai.zip`.
