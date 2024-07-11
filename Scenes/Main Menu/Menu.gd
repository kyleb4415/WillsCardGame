extends Control

func _on_pressed():
	get_tree().change_scene_to_file("res://Scenes3D/GameBoard.tscn") # Replace with function body.
	

func _on_options_pressed():
	get_tree().change_scene_to_file("res://Scenes/Main Menu/Options.tscn")


func _on_quit_pressed():
	get_tree().quit()
	

func _on_item_selected(index):
	match index: 
		0: 
			DisplayServer.window_set_size(Vector2i(1920,1080))
		1: 
			DisplayServer.window_set_size(Vector2i(1600,900))	
		2: 
			DisplayServer.window_set_size(Vector2i(1280,720))	


func back_on_pressed():
	get_tree().change_scene_to_file("res://Scenes/Main Menu/MainMenu.tscn")
	# AudioStreamPlaybackPolyphonic.play_stream("")


func audioback_on_pressed():
	get_tree().change_scene_to_file("res://Scenes/Main Menu/Options.tscn")


func _on_audio_pressed():
	get_tree().change_scene_to_file("res://Scenes/Main Menu/Audio.tscn")
