using Godot;
using System;

public partial class CardSpace : Area3D
{
    public bool IsOccupied { get; set; }
    // Called when the node enters the scene tree for the first time.
    /*
    private void _OnBodyEntered(Node body)
    {
        if (body is RigidBody3D)
        {
            GD.Print("Rigidbody entered the trigger collider");
            body.Position = new Vector3(this.Position.X, this.Position.Y, this.Position.Z);
        }
    }

    public Area3D.BodyEnteredEventHandler BodyEntered(Node body)
    {

    }
    */
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}
