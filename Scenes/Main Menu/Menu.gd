extends Control

# anything with _on_pressed is related to button functionality
func _on_pressed():
	SelectSFX.play() #plays the global sound effect for button pressing
	get_tree().change_scene_to_file("res://Scenes3D/GameBoard.tscn") # chanhges scene
	MenuMusic.stop() #when Play is hit - this will stop main menu music

func _on_options_pressed():
	SelectSFX.play()
	get_tree().change_scene_to_file("res://Scenes/Main Menu/Options.tscn")

func _on_quit_pressed():
	SelectSFX.play()
	get_tree().quit() # quits game

func _on_item_selected(index):
	match index: # drop-down selection for resolution modes
		0: 
			DisplayServer.window_set_size(Vector2i(1920,1080))
		1: 
			DisplayServer.window_set_size(Vector2i(1600,900))	
		2: 
			DisplayServer.window_set_size(Vector2i(1280,720))	


func back_on_pressed():
	SelectSFX.play()
	get_tree().change_scene_to_file("res://Scenes/Main Menu/MainMenu.tscn")
	
	# AudioStreamPlaybackPolyphonic.play_stream("")

func audioback_on_pressed():
	SelectSFX.play()
	get_tree().change_scene_to_file("res://Scenes/Main Menu/Options.tscn")
	


func _on_audio_pressed():
	SelectSFX.play()
	get_tree().change_scene_to_file("res://Scenes/Main Menu/Audio.tscn")
	


func display_on_pressed():
	SelectSFX.play()
	get_tree().change_scene_to_file("res://Scenes/Main Menu/Display.tscn")
	


func dmode_on_item_selected(index): #dropdown for display mode
	match index: 
		0: 
			DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_FULLSCREEN)
		1: 
			DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_WINDOWED)

func audio_on_pressed():
	SelectSFX.play()
	get_tree().change_scene_to_file("res://Scenes/Main Menu/Audio.tscn")
