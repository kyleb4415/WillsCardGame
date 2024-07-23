extends Control

@onready var SFX_BUS_ID = AudioServer.get_bus_index("SFX")
@onready var MUSIC_BUS_ID = AudioServer.get_bus_index("Music")

func _on_pressed():
	SelectSFX.play()
	get_tree().change_scene_to_file("res://Scenes3D/GameBoard.tscn") # Replace with function body.
	MenuMusic.stop()

func _on_options_pressed():
	SelectSFX.play()
	get_tree().change_scene_to_file("res://Scenes/Main Menu/Options.tscn")

func _on_quit_pressed():
	SelectSFX.play()
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
	


func dmode_on_item_selected(index):
	match index: 
		0: 
			DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_FULLSCREEN)
		1: 
			DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_WINDOWED)


func _on_value_changed(value):
	AudioServer.set_bus_volume_db(MUSIC_BUS_ID, linear_to_db(value))
	AudioServer.set_bus_mute(MUSIC_BUS_ID, value < .05)

func sfx_on_value_changed(value):
	AudioServer.set_bus_volume_db(SFX_BUS_ID, linear_to_db(value))
	AudioServer.set_bus_mute(SFX_BUS_ID, value < .05)


func audio_on_pressed():
	SelectSFX.play()
	get_tree().change_scene_to_file("res://Scenes/Main Menu/Audio.tscn")
