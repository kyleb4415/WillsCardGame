using Godot;
using Godot.Collections;
using System;

public partial class CardSpace : Area3D
{
	public bool IsOccupied { get; set; }


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		this.InputRayPickable = false;
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
