using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class MoveCard3D : Camera3D
{
	public readonly Script gameSpaceScript = ResourceLoader.Load<Script>("res://Scenes3D/MoveCard3D.cs");
	public readonly PackedScene gameSpace = ResourceLoader.Load<PackedScene>("res://Scenes3D/GameBoard.tscn");

    public Dictionary colliders; public Node gameSpaceInstance;
	public Vector2 mouse;
	public Vector2 screenSize;

	//do we want mouse velocity or not?

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gameSpaceInstance = gameSpace.Instantiate(); 
		//actually adding the instance to the scene... this may have been the mistake I made earlier
		//AddChild(gameSpaceInstance);
		mouse = new Vector2();
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		screenSize = GetViewport().GetVisibleRect().Size;

        if (colliders != null)
		{
            MoveColliders(colliders);
        }
	}

	//this hopefully will be able to intersect with objects using ray casting from camera and mouse position in the world space
    public override void _Input(InputEvent @event)
    {
		if(@event is InputEventMouseMotion)
		{
			mouse = (Vector2)@event.Get("position");
                //GD.Print("Mouse Movement at: ", mouseMotion.Position);
		}
        if (@event is InputEventMouseButton && @event.IsActionPressed("leftclick"))
        {
            GD.Print("Mouse click and drag detected!");
			//GD.Print("Mouse Click/Unclick at: ", mouseButton.Position);
			colliders = _GetSelection();
        }
		if(@event is InputEventMouseButton && @event.IsActionReleased("leftclick") && colliders is not null)
		{
			RigidBody3D colliderToMove = (RigidBody3D)colliders["collider"];
			colliderToMove.Set("gravity_scale", 1);
            colliders = null;
		}
        else if (@event is not InputEventMouseMotion && @event is not InputEventMouseButton)
		{
			GD.Print("Input event not type of mouse!");
		}
		base._Input(@event);
    }

	private Dictionary _GetSelection()
	{
        var worldspace = GetWorld3D().DirectSpaceState;
        var start = ProjectRayOrigin(mouse);
        var end = ProjectPosition(mouse, 3);
        var result = worldspace.IntersectRay(PhysicsRayQueryParameters3D.Create(start, end));
		GD.Print("Collider selected!");

        return result;
    }

	//fix when object is being picked up it's still being affected by gravity, if held for too long then it clips through ground
	private void MoveColliders(Dictionary colliders)
	{
		try
		{
			if (colliders.Count > 0)
			{
				GD.Print($"Colliders found! Dictionary has this first value: {colliders["collider"].VariantType}");
				try
				{
					var newCollider = colliders["collider"];
				}
				catch (Exception ex)
				{
					GD.Print(ex.Message);
				}
				RigidBody3D colliderToMove = (RigidBody3D)colliders["collider"];
				//var colliderToMoveParent = colliderToMoveChild.GetParent();
				GD.Print("Collider's parent is of type: " + colliderToMove.GetType());

				if (colliderToMove.GetType() == typeof(RigidBody3D))
				{
					//translating coordinates to screen space - the problem now is it's not taking into account angles so idk
					colliderToMove.Set("gravity_scale", 0);
                    colliderToMove.Position = ProjectPosition(mouse, 1);


                    GD.Print("Collider moved to " + colliderToMove.Position);
				}
				else
				{
					GD.Print("Parent is not a RigidBody3D!");
				}
			}
			else
			{
				GD.Print("No colliders found!");
			}
		}
		catch (Exception ex)
		{
			GD.Print("Invalid cast!");
			GD.Print(ex.Message);
		}
	}

    public override void _ExitTree()
    {
		gameSpaceInstance.QueueFree();
		colliders.Clear();
		base._ExitTree();
    }
}
