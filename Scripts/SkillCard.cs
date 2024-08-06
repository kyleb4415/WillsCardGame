using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;

public partial class SkillCard : Card, ICard
{
#nullable enable
    public int ID { get; set; }
    public string AbilityName { get; set; }
    public string Race { get; set; } = string.Empty;
    public BoardController b { get; set; } = new BoardController();
    public SkillCard(int id, string name, SQLiteBlob? cardImage, string ability, string desc, string type, SQLiteBlob? typeImage, int unlockedFlag, int manaCost, string race)
    {
        this.ID = id;
        this.CardName = name;
        this.CardImage = cardImage;
        this.AbilityName = ability;
        this.Description = desc;
        this.Type = type;
        this.TypeImage = typeImage;
        this.UnlockedFlag = unlockedFlag;
        this.ManaCost = manaCost;
        this.Race = race;
    }

    public SkillCard(int id, string name, SQLiteBlob? cardImage, string ability, string desc, string type, int unlockedFlag, int manaCost, string race)
    {
        this.ID = id;
        this.CardName = name;
        this.CardImage = cardImage;
        this.AbilityName = ability;
        this.Description = desc;
        this.Type = type;
        this.UnlockedFlag = unlockedFlag;
        this.ManaCost = manaCost;
        this.Race = race;
    }

    public SkillCard(int id, string name, string ability, string desc, string type, int unlockedFlag, int manaCost, string race)
    {
        this.ID = id;
        this.CardName = name;
        this.AbilityName = ability;
        this.Description = desc;
        this.Type = type;
        this.UnlockedFlag = unlockedFlag;
        this.ManaCost = manaCost;
        this.Race = race;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        b = GetNode("/root/GameBoard") as BoardController;
        base._Ready();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
