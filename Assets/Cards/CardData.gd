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
	
	$Bars/TopBar/Name/CenterContainer/Name.text = card_name
	$Bars/TopBar/Cost/CenterContainer/Cost.text = card_mana
	$Bars/CardDesc/DescContainer/CenterContainer/Desc.text = card_description
	$Bars/BottomBar/AttackContainer/CenterContainer/Attack.text = card_attack
	$Bars/BottomBar/HealthContainer/CenterContainer/Health.text = card_hp
	
	#resizing card to fit container
	$Card.texture = load(card_image_path)
	$Border.scale *= card_size/$Border.texture.get_size()
	$Card.scale *= card_size/$Card.texture.get_size()
	pass


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

