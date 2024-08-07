using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyAI : Node
{
	public List<ICard> EnemyDeck = new List<ICard>();
	public List<Card> EnemyHand = new List<Card>();
	public List<Node3D> EnemySpaces = new List<Node3D>();
	public List<Card> EnemyCardsOnBoard = new List<Card>();
	public int Health = 10;
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
					PlaceCard(rnd);
                    break;
				case int n when n >= 50 && n < 100:
					Attack(rnd);
					break;
            }
        }
	}

	//reparent card to other node
	public void PlayFirstTurn(Random rnd)
	{
		int cardSelection = rnd.Next(0, EnemyHand.Count);
		int spaceSelection = rnd.Next(0, EnemySpaces.Count);

		Card card = EnemyHand[cardSelection];
        card.Reparent(EnemySpaces[spaceSelection]);
		card.PlacedPos = new Vector3(0, 0, 0);

        EnemyHand.Remove(card);
		card.CanPickUp = false;
        EnemyCardsOnBoard.Add(card);
        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(card, "position", card.PlacedPos, 2f).SetTrans(Tween.TransitionType.Quad);

		RotationHelper.ResetRotation(card, this.GetTree());

    }

    public void PlaceCard(Random rnd)
    {
        if (EnemyHand.Count > 0 && CheckEmptySpaces() == true)
        {
            int cardSelection = rnd.Next(0, EnemyHand.Count);
            Card card = EnemyHand[cardSelection];
            int spaceSelection = rnd.Next(0, EnemySpaces.Count);
            if (EnemySpaces[spaceSelection].GetChildCount() == 0)
            {
                card.PlacedPos = EnemySpaces[spaceSelection].Position;
                EnemyHand.Remove(card);
                card.CanPickUp = false;

                Tween tween = GetTree().CreateTween();
                tween.TweenProperty(card, "position", card.PlacedPos, 2f).SetTrans(Tween.TransitionType.Quad);
                RotationHelper.ResetRotation(card, this.GetTree());
				EnemyCardsOnBoard.Add(card);
            }
			else
			{
				Attack(rnd);
			}

        }
    }

	public void Attack(Random rnd)
	{
		List<Card> playerCards = GetNode<BoardController>("/root/GameBoard").PlayerCardsOnBoard;
		int playerCardSelection;
		GD.Print(playerCards.Count);

        if (playerCards.Count > 1)
		{
            playerCardSelection = rnd.Next(0, playerCards.Count);
        }
		else if(playerCards.Count == 1)
		{
			playerCardSelection = 0;
		}
		else
		{
			//here can attack player health or something
			return;
		}

		int enemyCardSelection = rnd.Next(0, EnemyCardsOnBoard.Count);

        UnitCard pc = (UnitCard)playerCards[playerCardSelection];
        UnitCard ec = (UnitCard)EnemyCardsOnBoard[enemyCardSelection];
        switch (rnd.Next(0,2))
		{
			case 0:
				ec.State = CardState.Attacking;
                break;
			case 1:
				if(CardManager.cardAbilityDescriptionDict.ContainsKey(EnemyCardsOnBoard[enemyCardSelection].CardName))
				{
					ec.State = CardState.UsingAbility;
                }
				else
				{
					ec.State = CardState.Attacking;
				}
                break;
		}

        CardAction action = new CardAction(ec, pc);
		action.ExecuteAction();
    }


	public bool CheckEmptySpaces()
	{
        foreach (Node3D space in EnemySpaces)
        {
            if (space.GetChildCount() == 0)
            {
				return true;
            }
        }
		return false;
    }
}
