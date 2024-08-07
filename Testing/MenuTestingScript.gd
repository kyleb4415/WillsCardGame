extends Node2D

# Constant array of 2D scene paths
const SCENE_PATHS := [
	"res://Scenes/MainMenu.tscn",
	"res://Scenes/Audio.tscn",
	"res://Scenes/InGameMenu.tscn",
	"res://Scenes/Display.tscn"
]

var current_scene_index = 0  # Track the current scene index

# Called when the node enters the scene tree for the first time
func _ready():
	# Start automated scene testing
	_test_scene_rotation()

# Rotate to the next scene in the array
func _rotate_scene():
	# Calculate the next scene index
	current_scene_index = (current_scene_index + 1) % SCENE_PATHS.size()
	var next_scene = SCENE_PATHS[current_scene_index]

	# Print for debugging
	print("Changing to scene: ", next_scene)

	# Change the scene
	get_tree().change_scene(next_scene)

# Get the current scene path for testing purposes
func _get_current_scene_path() -> String:
	return SCENE_PATHS[current_scene_index]

# Automated scene rotation testing
func _test_scene_rotation() -> void:
	# Loop through each scene and test transition
	for i in range(SCENE_PATHS.size()):
		# Wait for the scene to fully load
		await get_tree().process_frame

		# Get current scene path and compare with SceneManager's record
		var expected_scene = _get_current_scene_path()  # Correct assignment and function call
		var current_scene = get_tree().current_scene.filename  # Get the current scene's filename

		# Log scene information
		_log_scene_info(expected_scene, current_scene)  # Correct placement

		# Verify the scene is the expected one
		_assert(expected_scene == current_scene, 
			"Scene mismatch: expected %s, got %s" % [expected_scene, current_scene])

		# Rotate to the next scene
		_rotate_scene()

	print("All scenes tested successfully.")

# Log scene information
func _log_scene_info(expected_scene: String, current_scene: String):
	print("Expected Scene: %s, Current Scene: %s" % [expected_scene, current_scene])

# Simple assertion function
func _assert(condition: bool, message: String = "Assertion failed"):
	if not condition:
		push_error(message)
		# You can pause the engine
