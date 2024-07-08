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


    public Dictionary colliders;
	//public Card LastCardSelected { get; set; }
	public List<UnitCard> SelectedCards { get; set; } = new List<UnitCard>();
    public Vector2 mouse;
	public Vector2 screenSize;
	public bool queueFree = false;
	//public bool MouseOverCard { get; set; }

	[Signal]
	public delegate void HoverCardEventHandler(Card c);

    public override void _Ready()
	{
        mouse = new Vector2();
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
    }

    public override void _Input(InputEvent @event)
    {
		if (@event is InputEventMouseMotion)
		{
			mouse = (Vector2)@event.Get("position");
		}
		//checks to see if click & drag, card will be selected
		else if (@event is InputEventMouseButton && @event.IsActionPressed("leftclick"))
		{
			colliders = RaycastHelper.GetCollisionPoint(this, mouse, 3.0f);
			if(colliders["collider"].AsGodotObject().GetType() != typeof(StaticBody3D))
			{
				Card c = (Card)colliders["collider"];
				if(c.CanPickUp != false)
				{
                    RotationHelper.ResetRotation(c, this.GetTree());
                }
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
                        foreach (var card in SelectedCards)
						{
                            if (card.HP > 0)
							{
                                card.Selected = false;
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
			try
			{
				if (colliders["collider"].AsGodotObject().GetType() == typeof(Card) || colliders["collider"].AsGodotObject().GetType() == typeof(UnitCard))
				{
                    Card card = (Card)colliders["collider"];
                    if (card.CanPickUp)
                    {
                        if (card.PlacedPos != default)
                        {
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
                }
                colliders = null;
            }
			catch(Exception ex)
			{
				GD.Print(ex.Message);
			}
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
    public void Area_OnBodyEntered(Node3D body)
    {
		//reconfigure to use signals so it can fix card automatically going to space
		foreach(var s in ((BoardController)this.GetParentNode3D()).cardSpaceInstances)
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

    public void Area_OnBodyExited(Node3D body)
    {
		Card cardBody = (Card)body;
		cardBody.PlacedPos = new Vector3(0, 0, 0);
		cardBody.CanPickUp = true;
    }

    public override void _ExitTree()
    {
		//queueFree = true;
		//this.QueueFree();
		colliders.Clear();
        base._ExitTree();
    }
}
