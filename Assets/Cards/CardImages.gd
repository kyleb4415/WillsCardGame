extends Node2D

var db : SQLite
var db_name = "res://DataStore/CardData.db"
var card_name

# Called when the node enters the scene tree for the first time.
func _ready():
	db = SQLite.new()
	db.path = db_name
	db.open_db()

func set_card_name(name):
	card_name = name
	load_images()

func load_images():
	var character_image_query = "SELECT Images FROM Card WHERE Name = " + card_name
	var character_image = db.select_row(character_image_query)
	
	var border_query = "SELECT Race.Border FROM Card JOIN Race ON Card.Race_ID = Race.ID WHERE Card.Name = " + card_name
	var border_image = db.select_row(character_image_query)
	
	var type_query = "SELECT Type.Image FROM Card JOIN Type ON Card.Type_ID = Type.ID WHERE Card.Name = " + card_name
	var type_image = db.select_row(type_query)
	
	$CharacterImage.texture = load(character_image)
	$CardImage.texture = load(character_image)
	$CardBorder.texture = load(border_image)
	$TypesContainer/Type.texture = load(type_image)
	



# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
