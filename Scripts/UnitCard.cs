using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;

public partial class UnitCard : Card, ICard
{
	public int ID { get; set; }
	public int HP { get; set; } = 0;
	public int Damage { get; set; } = 0;
	public string Race { get; set; }
	public string AbilityName { get; set; }

	[Signal]
	public delegate void CardHitEventHandler(int dmg);

	[Signal]
	public delegate void AbilityEventHandler(UnitCard c);

	[Signal]
	public delegate void OnDamagedEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BoardController b = GetNode("/root/GameBoard") as BoardController;
		if (Damage != 0 && HP != 0)
		{
			GetNode("Damage").Set("text", Damage);
			GetNode("HP").Set("text", HP);
			this.Ability += InvokeAbility;
			this.OnDamaged += UpdateHP;
        }
		b.PlayerTurnStarted += ProcessStatusEffects;
		this.CardHit += UnitCard_CardHit;
		this.CardReleased += Release;
		this.CardSelected += Select;
		base._Ready();
	}

	public UnitCard()
	{

	}

#nullable enable
	public UnitCard(int id, string name, SQLiteBlob? cardImage, string abilityName, string desc, string type, SQLiteBlob? typeImage, int damage, int hp, int manaCost, int unlockedFlag, string race)
	{
		this.ID = id;
		this.CardName = name;
		this.CardImage = cardImage;
		this.AbilityName = abilityName;
		this.Description = desc;
		this.Type = type;
		this.TypeImage = typeImage;
		this.Damage = damage;
		this.HP = hp;
		this.ManaCost = manaCost;
        this.UnlockedFlag = unlockedFlag;
		this.Race = race;

    }
    public UnitCard(int id, string name, SQLiteBlob? cardImage, string abilityName, string desc, string type, int damage, int hp, int manaCost, int unlockedFlag, string race)
    {
        this.ID = id;
        this.CardName = name;
        this.CardImage = cardImage;
        this.AbilityName = abilityName;
        this.Description = desc;
        this.Type = type;
        this.Damage = damage;
        this.HP = hp;
        this.ManaCost = manaCost;
        this.UnlockedFlag = unlockedFlag;
        this.Race = race;

    }

    public UnitCard(int id, string name, string abilityName, string desc, string type, int damage, int hp, int manaCost, int unlockedFlag, string race)
	{
		this.ID = id;
		this.CardName = name;
        this.AbilityName = abilityName;
        this.Description = desc;
		this.Type = type;
		this.Damage = damage;
		this.HP = hp;
        this.ManaCost = manaCost;
        this.UnlockedFlag = unlockedFlag;
		this.Race = race;
	}
#nullable disable
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	//TODO: Death animation
	public void UnitCard_CardHit(int damage)
	{
		this.HP -= damage;
		this.UpdateHP();
		if(this.HP <= 0)
		{
			this.Visible = false;
			//set this to play a death animation and then shuffle it back in the deck or something
			this.Position = new Vector3(100, 100, 100);
		}
	}

	public void UpdateHP()
	{
		this.GetNode("HP").Set("text", this.HP.ToString());
	}

	public void UpdateDamage()
	{
		this.GetNode("Damage").Set("text", this.Damage.ToString());
	}

    public void InvokeAbility(UnitCard c)
    {
        var method = CardManager.GetCardMethod(this);
        method.DynamicInvoke( new object[] { GetNode("/root/GameBoard"), c });
		/*
		BoardController b = GetNode("/root/GameBoard") as BoardController;
		c.UpdateHP();
		*/
    }

	//Harmful status effects are represented by a StatusEffect name key and a int[] consisting of two integers - the first being the 
	//damage from the status effect and the second being the amount of turns it is applied for.
	//Different status effects can be handled differently within this code or within a separate class but I haven't decided yet.
	public void ProcessStatusEffects()
	{
		if(Effects.Count > 0)
		{
			foreach(var s in Effects)
			{
				if (s.Duration > 0)
				{
					if(s.Boost != true)
					{
						GD.Print($"{this.CardName} inflicted with {s.Effect} for {s.Damage} damage!");
                        this.EmitSignal(SignalName.CardHit, s.Damage);
                        s.Duration--;
                    }
					else
					{
						UpdateDamage();
						this.Damage = Damage + s.Damage;
					}
				}
				if (s.Duration == 0)
				{
					Effects.Remove(s);
					return;
				}
			}
		}
		//maybe print status effects here for testing later
	}
}
