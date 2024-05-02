using Godot;
using System;
using System.Collections.Generic;


public partial class Card : RigidBody3D, ICard
{
    public bool IsPickedUp { get; set; }
    public Vector3 OriginPos { get; set; }
    public Vector3 PlacedPos { get; set; }

    //interface properties
    public string CardName { get; set; }
    public string Type { get; set; }
    public int ManaCost { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.InputRayPickable = true;
        this.OriginPos = new Vector3();
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
}
