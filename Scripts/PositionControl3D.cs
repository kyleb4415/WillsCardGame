using Godot;
using System;

public partial class PositionControl3D : Control
{
	Card CardBody { get; set; } = new Card();
	Vector2 offset = new Vector2(0f, 50f);
	public override void _Ready()
	{
		CardBody = this.GetParent() as Card;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var position3D = CardBody.GlobalPosition;
		var camera = GetViewport().GetCamera3D();
		var position2D = camera.UnprojectPosition(position3D);
		this.GlobalPosition = position2D + offset;
		base._Process(delta);
	}
}
