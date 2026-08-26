extends SceneTree

# 本地复现"NOPE"：在 mod 的 godot 工程里加载能量 tres 与其引用的 PNG。
# 运行: Godot_console --headless --path godot --script ../scripts/debug_energy_tres.gd
func _init():
	var paths = [
		"res://images/ui/card/energy_can_ao.png",
		"res://images/atlases/ui_atlas.sprites/card/energy_can_ao.tres",
	]
	for p in paths:
		print("exists(", p, ") = ", ResourceLoader.exists(p))
		var res = load(p)
		print("load -> ", res)
		if res is Texture2D:
			var img = res.get_image()
			print("  image size: ", img.get_size() if img else "null image")
	quit()
