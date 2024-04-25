using Godot;
using System;


public partial class Card : RigidBody3D
{

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.InputRayPickable = true;
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
}
