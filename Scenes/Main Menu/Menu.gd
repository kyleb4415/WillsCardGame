extends Control



func _on_pressed():
	get_tree().change_scene_to_file("res://Scenes3D/GameBoard.tscn") # Replace with function body.
	

func _on_options_pressed():
	pass # Replace with function body.


func _on_quit_pressed():
	get_tree().quit()
