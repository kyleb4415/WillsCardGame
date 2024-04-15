extends Node

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
	var card_info = readDataFromDB()
	
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
	#finish implementation if need be... maybe only for changing unlock state

func readDataFromDB():
	db.open_db()
	var table_name = "Card"
	db.query("SELECT * FROM " + table_name + ";") 
	for i in range(0, db.query_result.size()):
		print("Query result ", db.query_result[i]["Name"], db.query_result[i]["Description"])
		return db.query_result

