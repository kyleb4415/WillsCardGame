using Godot;
using System;
using System.Collections.Generic;

public partial class BoardController : Node3D
{
	// Called when the node enters the scene tree for the first time.

	private TextureProgressBar _textureProgressBar;
	public override void _Ready()
	{
		PrepareBoard();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_textureProgressBar.Value += 1D;
	}

	private void PrepareBoard()
	{
		PrepareTextureProgressBar();
    }

	private void PrepareTextureProgressBar()
	{
        _textureProgressBar = this.GetNode("ManaBar") as TextureProgressBar;
        _textureProgressBar.FillMode = (int)TextureProgressBar.FillModeEnum.LeftToRight;
        _textureProgressBar.MinValue = 0;
        _textureProgressBar.MaxValue = 1000;
    }

    /*
	private void AddCards()
	{
        var gameSpace = this;
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
    }
    */
}
