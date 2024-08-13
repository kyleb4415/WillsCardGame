using Godot;
using System;
using System.Collections.Generic;
using System.Data.SQLite;


public partial class Card : RigidBody3D, ICard
{
	public bool IsPickedUp { get; set; }
	public bool CanPickUp { get; set; }
	public bool Released { get; set; }
	public bool Selected { get; set; } = false;
	public bool MouseOver { get; set; } = false;
	public Vector3 PlacedPos { get; set; }
	//storing rotation and position information for card in fan so it can go back if released outside of a space
	public Vector3 OriginPos { get; set; } = new Vector3();
	public Vector3 OriginRot { get; set; } = new Vector3();
	public SQLiteBlob CardImage { get; set; } = null;
	public SQLiteBlob TypeImage { get; set; }
	public string CardName { get; set; }
	public string Description { get; set; }
	public string Type { get; set; }
	public int ManaCost { get; set; }
	public int UnlockedFlag { get; set; }
	public Vector2 MousePos { get; set; }
	public Panel ContextMenu { get; set; }
	public Timer ContextMenuTimer { get; set; }
	public CardState State { get; set; }
	public CardAlignment CardAlignmentType { get; set; }
	public List<StatusEffect> Effects { get; set; } = new List<StatusEffect>();

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
		this.PlacedPos = new Vector3(0, 0, 0);
		this.PlaceCard += Place;
		this.MouseExited += Card_MouseExited;
		this.MouseEntered += Card_MouseEntered;
		this.InputRayPickable = true;
		this.CanPickUp = true;
		this.ContextMenu = this.GetNode("ContextMenuControl/ContextMenu") as Panel;
		if(CardName != null)
		{
			this.Name = CardName;
		}
		
		var contextMenuText = ContextMenu.GetChild(0) as RichTextLabel;
		contextMenuText.Text = Description;
		//change to ability desc later
		ContextMenu.Visible = false;
		this.ContextMenuTimer = this.GetNode("ContextMenuControl/Timer") as Timer;
		ContextMenuTimer.Timeout += ContextMenuTimer_Timeout;
		if (Name is not null)
		{
			GetNode("Name").Set("text", CardName);
		}
		if(Description is not null)
		{
            GetNode("Description").Set("text", Description);
        }
        GetNode("ManaCost").Set("text", ManaCost.ToString());

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
				GetNode<BoardController>("/root/GameBoard").PlayerCardsOnBoard.Add(c);
				GetNode<BoardController>("/root/GameBoard").Hand.Remove(c);
				CardManager.CalculatePlayerCardAlignmentAnimation(GetNode<BoardController>("/root/GameBoard"), new Vector3(0, 0, 0), this.GetTree());
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
		c.Selected = !Selected;
		GD.Print($"{c.Name} is now selected: {c.Selected}");
		if(Selected == true)
		{
			c.GravityScale = 0;
		}
		else if(c.Selected == false && !c.CanPickUp && MouseOver == false)
		{
			Tween t = GetTree().CreateTween();
			t.TweenProperty(c, "position", this.PlacedPos, 0.25f).SetTrans(Tween.TransitionType.Quad);

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
				Vector3 collisionPoint = (Vector3)RaycastHelper.GetCollisionPoint((Camera3D)instance, instance.mouse, 9.0f)["position"];
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
			Tween tween = this.GetTree().CreateTween();
			tween.TweenProperty(this, "position", this.PlacedPos, 0.2f).SetTrans(Tween.TransitionType.Quad);
		}
		this.MouseOver = false;
		ContextMenuTimer.Stop();
		if(ContextMenu is not null)
		{
			ContextMenu.Hide();
		}

		
	}
	
	public void ApplyStatusEffect(StatusEffect effect, Card c)
	{
		c.Effects.Add(effect);
	}

	public enum CardAlignment
	{
		Player,
		Neutral,
		Enemy
	}

}
