extends Label

var min_font_size = 10
var max_font_size = 27

func _ready():
	self.connect("text_changed", self, "_adjust_font_size")


func _adjust_font_size():
	var rect = get_rect()
	var font = get("theme_override_fonts/font")

	for size in range(max_font_size, min_font_size - 1, -1):
		font.size = size
		if font.get_string_size(text).width <= rect.size.width:
			break

