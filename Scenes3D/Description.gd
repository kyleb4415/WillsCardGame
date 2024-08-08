extends Label3D


# Called when the node enters the scene tree for the first time.
func _ready():
	# Get the size of the 2D sprite
	var sprite = get_node("../../CardBase/CardImages/CardViewport/AbilityTextBox")
	var sprite_size = sprite.texture.get_size() * sprite.scale

	# Set the label's width and height to match the sprite's size
	#self.rect_max_size = sprite_size
	
	var scale_x = sprite_size.x / self.scale.x
	var scale_y = sprite_size.y / self.scale.y

	# Apply the scale to the label
	self.scale = Vector3(scale_x, scale_y, 1)
	pass

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
