using Godot;
using System;
using System.Data.SQLite;


public partial class Card : RigidBody3D, ICard
{
    public bool IsPickedUp { get; set; }
    public bool CanPickUp { get; set; }
    public bool Released { get; set; }
    public bool Selected { get; set; } = false;
    public bool MouseOver { get; set; } = false;
    public Vector3 PlacedPos { get; set; } = new Vector3();
    public SQLiteBlob CardImage { get; set; } = null;
    public SQLiteBlob TypeImage { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public int ManaCost { get; set; }
    public int UnlockedFlag { get; set; }
    public Vector2 MousePos { get; set; }
    public Panel ContextMenu { get; set; }
    public Timer ContextMenuTimer { get; set; }
    public CardState State { get; set; }

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
        this.GravityScale = 0;
        this.PlaceCard += Place;
        this.MouseExited += Card_MouseExited;
        this.MouseEntered += Card_MouseEntered;
        this.InputRayPickable = true;
        this.CanPickUp = true;
        this.ContextMenu = this.GetNode("ContextMenuControl/ContextMenu") as Panel;

        var contextMenuText = ContextMenu.GetChild(0) as RichTextLabel;
        contextMenuText.Text = Description;
        //change to ability desc later
        ContextMenu.Visible = false;
        this.ContextMenuTimer = this.GetNode("ContextMenuControl/Timer") as Timer;
        ContextMenuTimer.Timeout += ContextMenuTimer_Timeout;
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

    private void ContextMenuTimer_Timeout()
    {
        ContextMenu.Visible = true;
    }


    //implement method for dragging here (can change isRayPickable and such)
    public void PickUp(Card card)
    {

    }

    //implement method for dropped card here
    public void Place(Card c, TextureProgressBar t)
    {
        if(this.CanPickUp == true)
        {
            if (t.Value >= c.ManaCost * 100)
            {
                this.CanPickUp = false;
                t.Value -= c.ManaCost * 100;
            }
            else
            {
                //print something or give some notification that they don't have enough mana
            }
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public void Release(Card c)
    {
        this.Released = true;
    }

    public void Select(Card c)
    {
        this.Selected = !Selected;
        if(Selected == true)
        {
            this.GravityScale = 0;
        }
        else
        {
            this.GravityScale = 1;
        }
        //OpenContextMenu(c);
    }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        if (this.MouseOver == true && this.CanPickUp == false)
        {
            try
            {
                MoveCard3D instance = GetNode("/root/GameBoard/Camera3D") as MoveCard3D;
                Vector3 collisionPoint = (Vector3)RaycastHelper.GetCollisionPoint((Camera3D)instance, instance.mouse, 3.0f)["position"];
                RotationHelper.RotateCard(this, collisionPoint, this.GetTree());
            }
            catch (Exception e)
            {
                GD.Print(e.Message);
            }
        }
    }

    public void OpenContextMenu(Card c)
    {
        Node2D contextMenuParent = ResourceLoader.Load<PackedScene>("res://Scenes/CardContextMenu.tscn").Instantiate<Node2D>();
        PopupMenu contextMenuChild = contextMenuParent.GetChild(0) as PopupMenu;
        contextMenuChild.Set("position", new Vector2(c.GlobalPosition.X, c.GlobalPosition.Y));
        c.AddChild(contextMenuParent);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
    }

    public void Card_MouseEntered()
    {
        this.MouseOver = true;
        this.GravityScale = 0;
        if (!this.CanPickUp)
        {
            ContextMenuTimer.OneShot = true;
            ContextMenuTimer.Start(1);
        }
    }
    

    //handles logic for card animation on hover before & after selection
    public void Card_MouseExited()
    {
        if (this.Selected == true && this.CanPickUp == false)
        {
            RotationHelper.ResetRotation(this, this.GetTree());
        }
        else if(this.Selected == false && this.CanPickUp == false)
        {
            RotationHelper.ResetRotation(this, this.GetTree());
            Tween tween = CreateTween();
            tween.TweenProperty(this, "position", this.PlacedPos, 0.2f).SetTrans(Tween.TransitionType.Quad);
        }
        this.MouseOver = false;
        ContextMenuTimer.Stop();
        if(ContextMenu is not null)
        {
            ContextMenu.Hide();
        }

        
    }

}
