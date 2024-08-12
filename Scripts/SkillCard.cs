using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using static System.Data.Entity.Infrastructure.Design.Executor;
using static System.Net.Mime.MediaTypeNames;

public partial class SkillCard : Card, ICard
{
#nullable enable
    public int ID { get; set; }
    public string AbilityName { get; set; } = string.Empty;
    public string Race { get; set; } = string.Empty;
    public int Damage { get; set; }
    public BoardController b { get; set; } = new BoardController();
    public Dictionary CardUnder { get; set; } = new Dictionary();

    //image no damage
    public SkillCard(int id, string name, SQLiteBlob? cardImage, string ability, string desc, int manaCost, int unlockedFlag, string race, string type)
    {
        this.ID = id;
        this.CardName = name;
        this.CardImage = cardImage;
        if (ability != null)
        {
            this.AbilityName = ability;
        }
        this.Description = desc;
        this.Type = type;
        this.UnlockedFlag = unlockedFlag;
        this.ManaCost = manaCost;
        this.Race = race;
    }

    public SkillCard(int id, string name, SQLiteBlob? cardImage, string ability, string desc, int damage, int manaCost, int unlockedFlag, string race, string type)
    {
        this.ID = id;
        this.CardName = name;
        this.CardImage = cardImage;
        if (ability != null)
        {
            this.AbilityName = ability;
        }
        this.Description = desc;
        this.Damage = damage;
        this.Type = type;
        this.UnlockedFlag = unlockedFlag;
        this.ManaCost = manaCost;
        this.Race = race;
    }

    public SkillCard(int id, string name, string ability, string desc, int manaCost, int unlockedFlag, string race, string type)
    {
        this.ID = id;
        this.CardName = name;
        if (ability != null)
        {
            this.AbilityName = ability;
        }
        this.Description = desc;
        this.Type = type;
        this.UnlockedFlag = unlockedFlag;
        this.ManaCost = manaCost;
        this.Race = race;
    }

    //no image and damage
    public SkillCard(int id, string name, string ability, string desc, int damage, int manaCost, int unlockedFlag, string race, string type)
    {
        this.ID = id;
        this.CardName = name;
        if(ability != null)
        {
            this.AbilityName = ability;
        }

        this.Description = desc;
        this.Type = type;
        this.Damage = damage;
        this.UnlockedFlag = unlockedFlag;
        this.ManaCost = manaCost;
        this.Race = race;
    }

    public SkillCard(int id, string name, string ability, string desc, int damage, int manaCost, int unlockedFlag, string race, string type, bool healthOrDamage)
    {
        this.ID = id;
        this.CardName = name;
        if (ability != null)
        {
            this.AbilityName = ability;
        }

        this.Description = desc;
        this.Type = type;
        if (healthOrDamage = true)
        {
        }
        else
        {
            this.Damage = damage;
        }
        this.UnlockedFlag = unlockedFlag;
        this.ManaCost = manaCost;
        this.Race = race;
    }

    public SkillCard()
    {
        this.ID = 999;
        this.CardName = string.Empty;
        this.AbilityName = string.Empty;
        this.Description = string.Empty;
        this.Type = string.Empty;
        this.Damage = 0;
        this.ManaCost = 0;
        this.UnlockedFlag = 0;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        if (Damage != 0)
        {
            GetNode("Damage").Set("text", Damage);
        }
        //assigning properties
        GD.Print(this.IsInsideTree());
        b = GetNode("/root/GameBoard") as BoardController;
        base._Ready();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        GD.Print("processing");
        CardUnder = ReturnCardUnderSkillCard();
        GD.Print(CardUnder["collider"].AsStringName());
    }

    public override void _PhysicsProcess(double delta)
    {

        base._PhysicsProcess(delta);    
    }

    
    public Dictionary ReturnCardUnderSkillCard()
    {
        var space = GetWorld3D().DirectSpaceState;
        var query = PhysicsRayQueryParameters3D.Create(this.Position, new Vector3(this.Position.X, this.Position.Y - 9f, this.Position.Z));
        query.Exclude.Add(this.GetRid());
        GD.Print("calculating");
        var result = space.IntersectRay(query);
        return result;
    }
    
}
