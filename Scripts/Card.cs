using Godot;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Reflection.Metadata;


public partial class Card : RigidBody3D, ICard
{
	public bool IsPickedUp { get; set; }
	public bool CanPickUp { get; set; }
	public Vector3 OriginPos { get; set; }
	public Vector3 PlacedPos { get; set; }
	public SQLiteBlob CardImage { get; set; }
	public SQLiteBlob TypeImage { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public string Type { get; set; }
	public int ManaCost { get; set; }
	public int UnlockedFlag { get; set; }
	public bool Selected { get; set; }

	public Vector2 MousePos { get; set; }

	[Signal]
	public delegate void PlaceCardEventHandler(Card c);

	[Signal]
	public delegate void CardSelectedEventHandler(Card c);

	[Signal]
	public delegate void CardAttackEventHandler(Card attacker, Card defender);


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.PlaceCard += Place;
		this.InputRayPickable = true;
		this.OriginPos = new Vector3();
		this.CanPickUp = true;
		GetNode("Name").Set("text", Name);
		GetNode("Description").Set("text", Description);
		GetNode("ManaCost").Set("text", ManaCost.ToString());
	}

	//implement method for dragging here (can change isRayPickable and such)
	public void PickUp(Card card)
	{
		
	}

	//implement method for dropped card here
	public void Place(Card c)
	{
		this.CanPickUp = false;
	}

	

	/*
	public Card(bool isPickedUp, Vector3 originPos, Vector3 placedPos)
	{
		IsPickedUp = isPickedUp;
		OriginPos = originPos;
		PlacedPos = placedPos;
	}

	public Card(Card card)
	{
		this.IsPickedUp = card.IsPickedUp;
		this.OriginPos = card.OriginPos;
		this.PlacedPos = card.PlacedPos;
	}
	*/

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	/*
	public override void OnBodyEntered(RigidBody3D body)
	{
		
	}
	*/

	public override void _ExitTree()
	{
		base._ExitTree();
	}
}
