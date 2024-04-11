extends Node

const SQLiteCards = preload("res://addons/godot-sqlite/godot-sqlite.gd")
var db #the acutal database object
var db_name = "res://DataStore/CardData"
# Called when the node enters the scene tree for the first time.
func _ready():
	db = SQLite.new()
	db.path = db_name
	readDataFromDB()
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func commitDataToDB():
	db.open_db()
	var tableName = "Card"
	var dict : Dictionary = Dictionary()

func readDataFromDB():
	db.open_db()
	var tableName = "Card"
	db.query("SELECT * FROM " + tableName + ";") 
	for i in range(0, db.query_result.size()):
		print("Query result ", db.query_result[i]["Name"], db.query_result[i]["Description"])

