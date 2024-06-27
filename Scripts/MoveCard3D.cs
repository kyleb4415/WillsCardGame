using Godot;
using Godot.Collections;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Threading.Tasks;

public partial class MoveCard3D : Camera3D
{
	//reconfig later to use signals to avoid setting colliders to null
	public readonly Script gameSpaceScript = ResourceLoader.Load<Script>("res://Scripts/MoveCard3D.cs");
	public Script cardScript = ResourceLoader.Load<Script>("res://Scripts/UnitCard.cs");
	public readonly PackedScene cardSpace = ResourceLoader.Load<PackedScene>("res://Scenes3D/CardSpaceBase.tscn");
	public readonly PackedScene cardBase = ResourceLoader.Load<PackedScene>("res://Scenes3D/CardBase3D.tscn");

    public Dictionary colliders;
	public Card LastCardSelected { get; set; }
	public List<UnitCard> SelectedCards { get; set; } = new List<UnitCard>();
	public Node3D cardSpaceInstanceParent;
	public Area3D cardSpaceInstanceChild;
	public List<Node3D> cardSpaceInstances;
    List<Node> cardGameObjects = new List<Node>();
    public Vector2 mouse;
	public Vector2 screenSize;
	public bool queueFree = false;
	public bool MouseOverCard { get; set; }

	[Signal]
	public delegate void HoverCardEventHandler(Card c);

    public override void _Ready()
	{
        var gameSpace = this.GetParentNode3D();
        cardSpaceInstances = new List<Node3D>();
        mouse = new Vector2();

        //loading card spaces onto the points defined on the board
        //-------------------------------------------------------------------------------
        foreach (var space in GetNode("/root/GameBoard/BoardPositions").GetChildren())
		{
            cardSpaceInstanceParent = cardSpace.Instantiate() as Node3D;
            cardSpaceInstanceChild = cardSpaceInstanceParent.GetChild(0) as Area3D;
            cardSpaceInstances.Add(cardSpaceInstanceParent);
			space.CallDeferred("add_child", cardSpaceInstanceParent);
        }
        //-------------------------------------------------------------------------------

        //attaching events for card TODO: DELETE LATER
        //-------------------------------------------------------------------------------
        Card card = (Card)GetNode("/root/GameBoard/CardBody");

        card.MouseEntered += Card_MouseEntered;
        card.MouseExited += Card_MouseExited;
        //TODO: Fix this
        card.CardReleased += card.Release;
        card.CardSelected += card.Select;
        //-------------------------------------------------------------------------------

        //attaching events for card spaces
        //-------------------------------------------------------------------------------
        if (cardSpaceInstances is not null)
		{
			foreach(var s in cardSpaceInstances)
			{
				//attaches event for each cardspace

				var sChild = s.GetChild(0) as Area3D;
                sChild.BodyEntered += Area_OnBodyEntered;
				sChild.BodyExited += Area_OnBodyExited;
            }
        }
        //-------------------------------------------------------------------------------


        //instancing cards from db [move after testing]
        //-------------------------------------------------------------------------------
        List<UnitCard> cards = CardManager.LoadCardsFromDB();
		foreach(var c in cards)
		{
			//modify before instantiation
			var cardBaseInstance = cardBase.Instantiate();
			cardGameObjects.Add(cardBaseInstance);
			CardFactory.CreateUnitCard(c, cardBaseInstance);
			UnitCard unitCard = cardBaseInstance.GetChild(0) as UnitCard;
            unitCard.MouseEntered += Card_MouseEntered;
            unitCard.MouseExited += Card_MouseExited;
            unitCard.CardReleased += unitCard.Release;
            unitCard.CardSelected += unitCard.Select;
            unitCard.CardHit += unitCard.TakeDamage;
            //cardBaseInstance.SetScript(cardScript);
			this.GetParent().CallDeferred("add_child", cardBaseInstance);
        }
        //-------------------------------------------------------------------------------
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.

    public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
	}
	

    public override void _Process(double delta)
	{
		//screenSize = GetViewport().GetVisibleRect().Size;

        if (colliders != null)
		{
            MoveColliders(colliders, delta);
        }

		if(MouseOverCard == true && LastCardSelected.CanPickUp == false)
		{
			Vector3 collisionPoint = (Vector3)RaycastHelper.GetCollisionPoint(this, mouse, 3.0f)["position"];
            RotationHelper.RotateCard(LastCardSelected, collisionPoint, this.GetTree());
        }
    }

    public override async void _Input(InputEvent @event)
    {
		//TODO: below
		//refactor all card casts to see if they implement ICard instead (both inherit from Card so they should inherit parameters)

		if (@event is InputEventMouseMotion)
		{
			mouse = (Vector2)@event.Get("position");
		}

		//checks to see if click & drag, collider will be selected
		else if (@event is InputEventMouseButton && @event.IsActionPressed("leftclick"))
		{
			colliders = RaycastHelper.GetCollisionPoint(this, mouse, 3.0f);
			if(colliders["collider"].AsGodotObject().GetType() != typeof(StaticBody3D))
			{
				Card c = (Card)colliders["collider"];
				//!!!!TODO: Separate this out into its own method!!!!
				if(c.CanPickUp == false)
				{
					c.EmitSignal(Card.SignalName.CardSelected, c);
					GD.Print(c.Selected);
					if(!SelectedCards.Contains((UnitCard)c) && c.Selected == true)
					{
                        SelectedCards.Add((UnitCard)c);
						GD.Print("Added");
                    }
                    else
                    {
						c.Selected = false;
                        SelectedCards.Remove((UnitCard)c);
                    }
                    if (SelectedCards.Count > 1 && SelectedCards.Count < 3)
					{
						SelectedCards[1].HP = SelectedCards[1].HP - SelectedCards[0].Damage;
						SelectedCards[1].GetNode("HP").Set("text", SelectedCards[1].HP);
						GD.Print(SelectedCards[0].Name + " did " + SelectedCards[0].Damage + " damage to " + SelectedCards[1].Name + "!");
						SelectedCards[1].EmitSignal(UnitCard.SignalName.CardHit, SelectedCards[1]);
						foreach(var card in SelectedCards)
						{
							card.Selected = false;
							if(card.HP > 0)
							{
                                RotationHelper.ResetRotation(card, GetTree());
                            }
						}
						SelectedCards.Clear();
                    }
				}
			}
		}
		else if (@event is InputEventMouseButton && @event.IsActionReleased("leftclick") && colliders is not null && colliders["collider"].AsGodotObject().GetType() != typeof(StaticBody3D))
		{
			//Are we able to move cards around before we end the turn or are they placed when they're placed? !!US IDEA!!
			Card card = (Card)colliders["collider"];
			if(card.CanPickUp)
			{
				if (card.PlacedPos != default)
				{
					card.GravityScale = 1;
					Tween tween = CreateTween();
					tween.TweenProperty(card, "position", card.PlacedPos, 0.5f).SetTrans(Tween.TransitionType.Quad);
					tween.Finished += () =>
					{
						card.EmitSignal(Card.SignalName.PlaceCard, card, this.GetParent().GetNode("ManaBar"));
					};
				}
				else
				{
					card.GravityScale = 1;
				}
            }
            colliders = null;
        }
		base._Input(@event);
    }

    private void MoveColliders(Dictionary colliders, double delta)
	{
		try
		{
			if (colliders.Count > 0)
			{
				if (colliders["collider"].AsGodotObject().GetType() != typeof(StaticBody3D))
				{
					dynamic colliderToMove;
					if (colliders["collider"].AsGodotObject().GetType() == typeof(UnitCard))
					{
						colliderToMove = (UnitCard)colliders["collider"];
					}
					else if (colliders["collider"].AsGodotObject().GetType() == typeof(SkillCard))
					{
						colliderToMove = (SkillCard)colliders["collider"];
					}
					else if (colliders["collider"].AsGodotObject().GetType() == typeof(Card))
					{
						colliderToMove = (Card)colliders["collider"];
					}
                    else
                    {
						return;
                    }

					if(colliderToMove.CanPickUp == true)
					{
                        colliderToMove.Set("gravity_scale", 0);
                        colliderToMove.IsPickedUp = true;
                        colliderToMove.Position = colliderToMove.Position.Lerp(ProjectPosition(mouse, 2.5f), (float)delta * 10);
                    }
                }
			}
		}
		catch (Exception ex)
		{
			GD.Print("Invalid cast!");
			GD.Print(ex.Message);
		}
	}

    //this is the event for the area3d colliders, should lerp card to space
    private void Area_OnBodyEntered(Node3D body)
    {
		//reconfigure to use signals so it can fix card automatically going to spac
		foreach(var s in cardSpaceInstances)
		{
			Area3D area = s.GetChild(0) as Area3D;

			if(area.GetOverlappingBodies().Count > 1 && area.GetOverlappingBodies().Count < 3)
			{
				Card bodyAsCard = area.GetOverlappingBodies()[1] as Card;
				if (bodyAsCard.CanPickUp == true)
				{
					bodyAsCard.PlacedPos = new Vector3(area.GetParentNode3D().GetParentNode3D().Position.X, -1.5f, area.GetParentNode3D().GetParentNode3D().Position.Z);
				}
			}
		}
    }

	
    private void Area_OnBodyExited(Node3D body)
    {
		Card cardBody = (Card)body;
		cardBody.PlacedPos = new Vector3(0, 0, 0);
		cardBody.CanPickUp = true;
    }
	

    private void Card_MouseEntered()
    {
        LastCardSelected = (Card)RaycastHelper.GetCollisionPoint(this, mouse, 3.0f)["collider"];
        if (LastCardSelected != null && LastCardSelected.CanPickUp == false)
        {
            MouseOverCard = true;
        }
    }

    private void Card_MouseExited()
    {
        if (LastCardSelected != null && LastCardSelected.CanPickUp == false)
        {
            MouseOverCard = false;
            RotationHelper.ResetRotation(LastCardSelected, this.GetTree());
        }
    }

    public override void _ExitTree()
    {
		queueFree = true;
		cardSpaceInstanceChild.QueueFree();
		this.QueueFree();
		colliders.Clear();
		foreach(var c in cardSpaceInstances)
		{
			c.QueueFree();
		}
		foreach(var c in cardGameObjects)
		{
			c.QueueFree();
		}
        base._ExitTree();
    }
}
