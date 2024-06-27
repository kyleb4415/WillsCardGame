using Godot;
using System;
using System.Data.SQLite;


public partial class Card : RigidBody3D, ICard
{
    public bool IsPickedUp { get; set; }
    public bool CanPickUp { get; set; }
    public bool Released { get; set; }
    public bool Selected { get; set; } = false;
    public Vector3 OriginPos { get; set; }
    public Vector3 PlacedPos { get; set; }
    public SQLiteBlob CardImage { get; set; } = null;
    public SQLiteBlob TypeImage { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public int ManaCost { get; set; }
    public int UnlockedFlag { get; set; }

    public Vector2 MousePos { get; set; }

    [Signal]
    public delegate void PlaceCardEventHandler(Card c, TextureProgressBar t);

    [Signal]
    public delegate void CardReleasedEventHandler(Card c);

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
        if (Name is not null && Description is not null)
        {
            GetNode("Name").Set("text", Name);
            GetNode("Description").Set("text", Description);
            GetNode("ManaCost").Set("text", ManaCost.ToString());
        }
        if(CardImage is not null)
        {
            PngImageLoader.LoadPngFromDatabase(this, 200, 200);
        }

        (GetNode("SelectedLight") as OmniLight3D).SetLayerMaskValue(1, true);
    }

    //implement method for dragging here (can change isRayPickable and such)
    public void PickUp(Card card)
    {

    }

    //implement method for dropped card here
    public void Place(Card c, TextureProgressBar t)
    {
        this.CanPickUp = false;
        t.Value -= c.ManaCost * 100;
    }

    public void Release(Card c)
    {
        this.Released = true;
    }

    public void Select(ICard c)
    {
        this.Selected = !Selected;
    }
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
