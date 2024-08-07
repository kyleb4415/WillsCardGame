extends Node2D


# Called when the node enters the scene tree for the first time.
func _ready():
	# test_scene_rotation.gd

const SCENE_PATHS = [
	"res://Scenes/MainMenu.tscn",
	"res://Scenes/Audio.tscn",
	"res://Scenes/InGameMenu.tscn",
	"res://Scenes/GameOver.tscn"
]

func before_all():
	# Preload all scenes for faster instancing during tests
	for path in SCENE_PATHS:
		preload(path)

func test_scene_rotation():
	for i in range(SCENE_PATHS.size()):
		var current_scene = SCENE_PATHS[i]
		var next_scene = SCENE_PATHS[(i + 1) % SCENE_PATHS.size()]

		# Instance the current scene
		var scene_instance = preload(current_scene).instance()
		add_child(scene_instance)
		yield(get_tree(), "idle_frame")  # Wait for the scene to load

		# Spy on change_scene
		spy(get_tree(), "change_scene")

		# Call the rotate_scene function
		scene_instance.call("rotate_scene")

		# Wait for the next frame to ensure the scene change takes effect
		yield(get_tree(), "idle_frame")

		# Assert that the next scene is loaded
		assert_spy_called_with(get_tree(), "change_scene", [next_scene],
			"Expected scene transition from %s to %s" % [current_scene, next_scene])

		# Remove the current scene instance after the test
		remove_child(scene_instance)
		scene_instance.queue_free()



# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
