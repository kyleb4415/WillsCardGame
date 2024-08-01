extends Node

#written in godot for now because c# was having a fit over SQLite...

@onready var card_size = self.size
var card_name = "Burner"
@onready var card_image_path = str("res://Assets/Cards/", card_name, ".png")
const sqlite_cards = preload("res://addons/godot-sqlite/godot-sqlite.gd")
var db #the acutal database object
var db_name = "res://DataStore/CardData"

# Called when the node enters the scene tree for the first time.
func _ready():
	db = SQLite.new()
	db.path = db_name
	
	#used for dynamic card creation
	var card_info = readDataForCard(card_name)
	var card_name = str(card_info[0]["Name"])
	var card_description = str(card_info[0]["Description"])
	var card_attack = str(card_info[0]["Damage"])
	var card_hp = str(card_info[0]["HP"])
	var card_mana = str(card_info[0]["ManaCost"])
	
	##$Bars/TopBar/Name/CenterContainer/Name.text = card_name
	##$Bars/TopBar/Cost/CenterContainer/Cost.text = card_mana
	##$Bars/CardDesc/DescContainer/CenterContainer/Desc.text = card_description
	##$Bars/BottomBar/AttackContainer/CenterContainer/Attack.text = card_attack
	##$Bars/BottomBar/HealthContainer/CenterContainer/Health.text = card_hp
	
	$ManaNumber.text = card_mana
	$AttackNumber.text = card_attack
	$HealthNumber.text = card_hp
	$Name.text = card_name
	$Description.text = card_description
	
	
	#resizing card to fit container
	$CardImage.texture = load(card_image_path)
	$Border.scale *= card_size/$Border.texture.get_size()
	$Card.scale *= card_size/$Card.texture.get_size()
	
	var card_node = $CardNode  # Replace with the actual path to your card node
	capture_card_texture(card_node)
	pass


func capture_card_texture(card_node):
	var viewport = Viewport.new()
	viewport.size = card_node.rect_size
	viewport.render_target_update_mode = Viewport.UPDATE_ONCE
	viewport.render_target_v_flip = true
	add_child(viewport)
	
	var card_instance = card_node.duplicate()
	viewport.add_child(card_instance)
	
	await viewport.size_changed()
	
	var texture = viewport.get_texture()
	var image = texture.get_data()
	if image.empty():
		print("Failed to capture image. Check the viewport setup.")
		return
	
	var save_path = "res://Assets/Cards/card_texture.png"
	var error = image.save_png(save_path)
	if error != OK:
		print("Failed to save image. Error code: ", error)
		return
	
	print("Texture saved successfully at: ", save_path)
	viewport.queue_free()

# Called every frame. 'delta' is the elapsed time since the previous frame.

func _process(delta):
	pass

func commitDataToDB():
	db.open_db()
	var table_name = "Card"
	var dict : Dictionary = Dictionary()
	#implementation unfinished - may need to for unlock state
	

#test function, reads all rows from card table in db
func readDataFromDB():
	db.open_db()
	var table_name = "Card"
	db.query("SELECT * FROM " + table_name + ";") 
	for i in range(0, db.query_result.size()):
		print("Query result ", db.query_result[i]["Name"], db.query_result[i]["Description"])
		return db.query_result
		
#reads card by name
func readDataForCard(card_name):
	db.open_db()
	db.query("SELECT * FROM CARD WHERE NAME = \"" + card_name + "\";")
	print("Query result from single query ", db.query_result[0]["Name"], " ", db.query_result[0]["Description"])
	return db.query_result

