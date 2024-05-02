extends Node2D


# Called when the node enters the scene tree for the first time.

const card_base = preload("res://Assets/Cards/CardBase.tscn")
const player_hand = preload("res://Scenes/Boards/BoardTest.gd")

func _ready():
	pass # Replace with function body.

func _input(event):
	if Input.is_action_just_released("leftclick"):
		var new_card = card_base.instantiate()
		new_card.card_name = "Burner"
		new_card.position = get_global_mouse_position()
		$Cards.add_child(new_card)
