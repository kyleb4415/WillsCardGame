extends MeshInstance3D


# Called when the node enters the scene tree for the first time.
func _ready():
	var mesh_instance = $MeshInstance3D
	var material = StandardMaterial3D.new()
	var texture = load("res://Assets/Cards/card_texture.png")
	material.albedo_texture = texture
	mesh_instance.material_override = material
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	pass
