using Godot;
using Godot.Collections;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

public partial class MoveCard3D : Camera3D
{
	public readonly Script gameSpaceScript = ResourceLoader.Load<Script>("res://Scripts/MoveCard3D.cs");
	public readonly PackedScene cardSpace = ResourceLoader.Load<PackedScene>("res://Scenes3D/CardSpaceBase.tscn");

    public Dictionary colliders; 
	public Node3D cardSpaceInstanceParent;
	public Area3D cardSpaceInstanceChild;
	public List<Node3D> cardSpaceInstances;
	public Vector2 mouse;
	public Vector2 screenSize;

	//do we want mouse velocity or not?

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cardSpaceInstances = new List<Node3D>();
		cardSpaceInstanceParent = cardSpace.Instantiate() as Node3D;
		cardSpaceInstanceChild = cardSpaceInstanceParent.GetChild(0) as Area3D;
        cardSpaceInstances.Add(cardSpaceInstanceParent);

		if(cardSpaceInstances is not null)
		{
			var gameSpace = this.GetParentNode3D();
			gameSpace.CallDeferred("add_child", cardSpaceInstances[0]);
			cardSpaceInstances[0].Position = new Vector3(0, -0.5f, 0);
			foreach(var s in  cardSpaceInstances)
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

	//this hopefully will be able to intersect with objects using ray casting from camera and mouse position in the world space
    public override void _Input(InputEvent @event)
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
			colliderToMove.Set("gravity_scale", 1);
			//LerpToSpace();
			colliderToMove.Position = colliderToMove.Position.Lerp(ProjectPosition(mouse, 1.7f), (float)GetPhysicsProcessDeltaTime() * 10);
			colliders = null;
		}
		else if (@event is not InputEventMouseMotion && @event is not InputEventMouseButton)
		{
			GD.Print("Input event not type of mouse!");
		}
		base._Input(@event);
    }

	//fix when object is being picked up it's still being affected by gravity, if held for too long then it clips through ground
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
		GD.Print("Card body entered!");
    }

    private void _OnBodyExited(Node3D body)
    {
		GD.Print("Card body exited!");
    }


    //probably write separate function here for dropping into spots

    public override void _ExitTree()
    {
		cardSpaceInstanceChild.QueueFree();
		colliders.Clear();
		cardSpaceInstances.Clear();
		base._ExitTree();
    }
}
