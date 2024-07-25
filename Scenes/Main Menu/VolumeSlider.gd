extends HSlider

@export
var bus_name: String #these are exported to our inspector where we can label our sliders
var bus_index: int 

func _ready() -> void: 
	bus_index = AudioServer.get_bus_index(bus_name)
	value_changed.connect(_on_value_changed)
	
	value = db_to_linear( # this allows for the changes to the slider to remain
		AudioServer.get_bus_volume_db(bus_index)
	)
	

func _on_value_changed(value: float) -> void:
	AudioServer.set_bus_volume_db(
		bus_index,
		linear_to_db(value)		
	)
