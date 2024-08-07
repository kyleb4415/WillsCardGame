using Godot;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

public partial class CardNode : Node
{
	private SQLiteConnection db;
	private string dbName = "res://DataStore/CardData.db";
	private string cardName;

	public override void _Ready()
	{
		db = new SQLiteConnection($"Data Source={dbName};Version=3;");
		db.Open();
	}

	public void SetCardName(string name)
	{
		cardName = name;
		LoadImages();
	}

	private void LoadImages()
	{
		string characterImageQuery = $"SELECT Images FROM Card WHERE Name = '{cardName}'";
		string characterImage = ExecuteScalarQuery(characterImageQuery);

		string borderQuery = $"SELECT Race.Border FROM Card JOIN Race ON Card.Race_ID = Race.ID WHERE Card.Name = '{cardName}'";
		string borderImage = ExecuteScalarQuery(borderQuery);

		string typeQuery = $"SELECT Type.Image FROM Card JOIN Type ON Card.Type_ID = Type.ID WHERE Card.Name = '{cardName}'";
		string typeImage = ExecuteScalarQuery(typeQuery);

		if (!string.IsNullOrEmpty(characterImage))
		{
			((TextureRect)GetNode("CardImage")).Texture = (Texture2D)GD.Load(characterImage);
		}

		if (!string.IsNullOrEmpty(borderImage))
		{
			((TextureRect)GetNode("CardBorder")).Texture = (Texture2D)GD.Load(borderImage);
		}

		if (!string.IsNullOrEmpty(typeImage))
		{
			((TextureRect)GetNode("TypesContainer/Type")).Texture = (Texture2D)GD.Load(typeImage);
		}
	}
	
	private string ExecuteScalarQuery(string query)
	{
		using (var command = new SQLiteCommand(query, db))
		{
			var result = command.ExecuteScalar();
			return result != null ? result.ToString() : string.Empty;
		}
	}
}
