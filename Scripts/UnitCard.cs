using Godot;
using System;
using System.Data.SQLite;
using System.Linq;
using System.Reflection.Metadata;

public partial class UnitCard : Card, ICard
{
	public int ID { get; set; }
	public int HP { get; set; } = 0;
	public int Damage { get; set; } = 0;

    [Signal]
    public delegate void CardHitEventHandler(UnitCard c);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
		if (Damage != 0 && HP != 0)
		{
            GetNode("Damage").Set("text", Damage);
            GetNode("HP").Set("text", HP);
        }

        base._Ready();
    }

	public UnitCard()
	{

	}

	public UnitCard(int id, string name, SQLiteBlob? cardImage, string desc, string type, SQLiteBlob? typeImage, int damage, int hp, int unlockedFlag, int manaCost)
	{
		this.ID = id;
		this.Name = name;
		this.CardImage = cardImage;
		this.Description = desc;
		this.Type = type;
		this.TypeImage = typeImage;
		this.Damage = damage;
		this.HP = hp;
		this.UnlockedFlag = unlockedFlag;
		this.ManaCost = manaCost;

	}

    public UnitCard(int id, string name, string desc, string type, int damage, int hp, int unlockedFlag, int manaCost)
	{
		this.ID = id;
		this.Name = name;
		this.Description = desc;
		this.Type = type;
		this.Damage = damage;
		this.HP = hp;
		this.UnlockedFlag = UnlockedFlag;
		this.ManaCost = manaCost;
	}
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{

	}

	//TODO: Death animation
	public void TakeDamage(UnitCard c)
	{
		if(c.HP <= 0)
		{
			this.QueueFree();
		}
	}
}
