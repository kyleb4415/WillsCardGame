using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyAI : Node
{
	public List<ICard> EnemyDeck = new List<ICard>();
	public List<Card> EnemyHand = new List<Card>();
	private bool _firstTurn = true;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		EnemyDeck = CardManager.LoadCardsFromDB();
		foreach(var c in EnemyDeck)
		{
			Card card = (Card)c;
			card.CardAlignmentType = Card.CardAlignment.Enemy;
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
            PlayFirstTurn();
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

	public void PlayFirstTurn()
	{

	}

	public void PlaceCard()
	{

	}
}
