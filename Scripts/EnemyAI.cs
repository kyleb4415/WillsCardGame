using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyAI : Node
{
	public List<ICard> EnemyDeck = new List<ICard>();
	public List<Card> EnemyHand = new List<Card>();
	public List<Node3D> EnemySpaces = new List<Node3D>();
	private bool _firstTurn = true;
	private BoardController _boardController;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		EnemyDeck = CardManager.LoadCardsFromDB();
		_boardController = this.GetParent<BoardController>();
		_boardController.EnemyTurnStarted += PlayTurn;
		foreach(var space in GetNode("EnemyBoardPositions").GetChildren())
		{
			EnemySpaces.Add((Node3D)space);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void PlayTurn()
	{
		Random rnd = new Random();
		int selection = rnd.Next(0, 101);
		if(_firstTurn == true)
		{
            PlayFirstTurn(rnd);
			_firstTurn = false;
        }
		else
		{
            switch (selection)
            {
                case int n when n >= 0 && n < 50:
                    break;
            }
        }
	}

	//reparent card to other node
	public void PlayFirstTurn(Random rnd)
	{
		GD.Print("playfristrun method");
		int cardSelection = rnd.Next(0, EnemyHand.Count);
		int spaceSelection = rnd.Next(0, EnemySpaces.Count);

		Card card = EnemyHand[cardSelection];
		card.PlacedPos = EnemySpaces[spaceSelection].Position;

		EnemyHand.Remove(card);
		card.CanPickUp = false;

        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(card, "position", card.PlacedPos, 2f).SetTrans(Tween.TransitionType.Quad);
		RotationHelper.ResetRotation(card, this.GetTree());
    }

	public void PlaceCard()
	{

	}
}
