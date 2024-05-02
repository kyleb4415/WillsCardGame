using Godot;
using Godot.Collections;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

public partial class MoveCard3D : Camera3D
{
	//reconfig later to use signals to avoid setting colliders to null
	public readonly Script gameSpaceScript = ResourceLoader.Load<Script>("res://Scripts/MoveCard3D.cs");
	public readonly PackedScene cardSpace = ResourceLoader.Load<PackedScene>("res://Scenes3D/CardSpaceBase.tscn");

    public Dictionary colliders; 
	public Node3D cardSpaceInstanceParent;
	public Area3D cardSpaceInstanceChild;
	public List<Node3D> cardSpaceInstances;
	public Vector2 mouse;
	public Vector2 screenSize;

	//TODO: separate out instancing board spaces
    public override void _Ready()
	{
        var gameSpace = this.GetParentNode3D();
        cardSpaceInstances = new List<Node3D>();
		foreach (var space in GetNode("/root/GameBoard/BoardPositions").GetChildren())
		{
			GD.Print("Adding space");
            cardSpaceInstanceParent = cardSpace.Instantiate() as Node3D;
            cardSpaceInstanceChild = cardSpaceInstanceParent.GetChild(0) as Area3D;
            cardSpaceInstances.Add(cardSpaceInstanceParent);
			space.CallDeferred("add_child", cardSpaceInstanceParent);
        }

		if(cardSpaceInstances is not null)
		{
			foreach(var s in cardSpaceInstances)
			{
				//attaches event for each cardspace

				var sChild = s.GetChild(0) as Area3D;
                sChild.BodyEntered += _OnBodyEntered;
				sChild.BodyExited += _OnBodyExited;
            }
        }
		mouse = new Vector2();
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		//screenSize = GetViewport().GetVisibleRect().Size;

        if (colliders != null)
		{
            MoveColliders(colliders, delta);
        }
	}

    public override async void _Input(InputEvent @event)
    {
		//checking to see if mouse is moving

		if (@event is InputEventMouseMotion)
		{
			mouse = (Vector2)@event.Get("position");
		}
		//checks to see if click & drag, collider will be selected
		else if (@event is InputEventMouseButton && @event.IsActionPressed("leftclick"))
		{
			colliders = RaycastHelper.GetCollisionPoint(this, mouse, 3.0f);
		}
		else if (@event is InputEventMouseButton && @event.IsActionReleased("leftclick") && colliders is not null && colliders["collider"].AsGodotObject().GetType() != typeof(StaticBody3D))
		{
			Card colliderToMove = (Card)colliders["collider"];
            colliderToMove.IsPickedUp = false;
			//colliderToMove.Position = colliderToMove.Position.Lerp(ProjectPosition(mouse, 1.7f), (float)GetPhysicsProcessDeltaTime() * 10);
			Tween tween = CreateTween();
			GD.Print($"Card moving to {colliderToMove.PlacedPos}");
            tween.TweenProperty(colliderToMove, "position", colliderToMove.PlacedPos, 0.5f).SetTrans(Tween.TransitionType.Quad);
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
                    Card colliderToMove = (Card)colliders["collider"];

                    if (colliderToMove.GetType() == typeof(Card))
                    {
                        //translating coordinates to screen space - the problem now is it's not taking into account angles so idk
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
    private void _OnBodyEntered(Node3D body)
    {
		//reconfigure to use signals
		foreach(var s in cardSpaceInstances)
		{
			Area3D area = s.GetChild(0) as Area3D;

			if(area.GetOverlappingBodies().Count > 1)
			{
				Card bodyAsCard = body as Card;
				bodyAsCard.PlacedPos = new Vector3(area.GetParentNode3D().GetParentNode3D().Position.X, -1.5f, area.GetParentNode3D().GetParentNode3D().Position.Z);
			}
		}
    }

    private void _OnBodyExited(Node3D body)
    {
		Card cardBody = (Card)body;
		cardBody.PlacedPos = new Vector3(0, 0, 0);
    }

    public override void _ExitTree()
    {
		cardSpaceInstanceChild.QueueFree();
		colliders.Clear();
		cardSpaceInstances.Clear();
		base._ExitTree();
    }
}
