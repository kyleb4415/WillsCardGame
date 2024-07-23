using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

public static class CardManager
{
    [Signal]
    public delegate void CardInteractionEventHandler(Card c, Card d);
    public static void InitialDealCards(BoardController b, SceneTree s)
    {
		InitialDealCardsAnimation(b, new Vector3(0,0,0), s);
    }

	//dealing cards from hand
	public static void InitialDealCardsAnimation(BoardController b, Vector3 dealPosition, SceneTree s)
	{
        Vector3 fanPositionLeft = new Vector3(-0.1f, -1f, 1f);
        Vector3 fanPositionRight = new Vector3(0.1f, -1f, 1f);

		//maybe change positions to accommodate more cards if necessary?
		if(b.Hand.Count > 4)
		{

		}

        Vector3 fanRotationLeft = new Vector3(0, 0.25f, 0);
        Vector3 fanRotationRight = new Vector3(0, -0.25f, 0);

		float idx = 0;

        foreach (Card c in b.Hand)
		{
            float alignmentWeight = CalculateCardAlignment(b.Hand, idx);
			float timeCalculation = 2f * (c.Position.X - fanPositionLeft.X);
            GD.Print($"Fan alignment {alignmentWeight}");
            c.GravityScale = 0;
			Tween t = s.CreateTween();
			Tween t2 = s.CreateTween();
            t.TweenProperty(c, "position", fanPositionLeft.Lerp(fanPositionRight, alignmentWeight), timeCalculation).SetTrans(Tween.TransitionType.Quad);
			t2.TweenProperty(c, "rotation", fanRotationLeft.Lerp(fanRotationRight, alignmentWeight), 0.25f).SetTrans(Tween.TransitionType.Quad);
			c.PlacedPos = fanPositionLeft.Lerp(fanPositionRight, alignmentWeight);
			idx += 1;
        }
	}

	//gets weight by setting index over count
	private static float CalculateCardAlignment(List<Card> cardList, float idx)
	{
		if(cardList.Count != 0)
		{
            return idx / (cardList.Count - 1f);
        }
		return 0.5f;
	}


	private static void ShuffleCards(List<Card> cards)
	{
		Random rand = new();
		for(int i = 0; i < cards.Count; i++)
		{
			Card temp = cards[i];
			cards[i] = cards[rand.Next(cards.Count)];
			cards[rand.Next(cards.Count)] = temp;
		}
	}

	public static List<UnitCard> LoadCardsFromDB()
	{
		List<UnitCard> CardList = new();
		using (SQLiteConnection conn = new SQLiteConnection($"Data Source=DataStore/CardData.db"))
		{
			conn.Open();
			using(var command = new SQLiteCommand(conn))
			{
				command.CommandText = @"SELECT rowid, * FROM Card";
				using (SQLiteDataReader reader = command.ExecuteReader(CommandBehavior.KeyInfo))
				{
                    while (reader.Read())
					{
						//use factory with reader values as inputs and then put into list that is returned at the end the method
						//rewrite GetBlobs to call a separate method that creates an appropriate buffer and loads the image
						if(reader.GetValue(3) != null)
						{
							try
							{
								//For UnitCard Constructor
								//1 - ID
								//2 - Name
								//3 - Image
								//4 - Description
								//5 - Type
								//6 - TypeImage
								//7 - Damage
								//8 - HP
								//9 - UnlockedFlag
								//10 - ManaCost
                                CardList.Add(new UnitCard(reader.GetInt32(1), reader.GetString(2), reader.GetBlob(3, true), reader.GetString(4), reader.GetString(5), reader.GetBlob(6, true), reader.GetInt32(7), reader.GetInt32(8), reader.GetInt32(9), reader.GetInt32(10)));
                            }
							catch(Exception e)
							{
								GD.Print(e.Message);
							}
                        }
						else
						{
							//UnitCard Constructor Overload w/o CardImage TypeImage
                            CardList.Add(new UnitCard(reader.GetInt32(0), reader.GetString(2), reader.GetString(4), reader.GetString(5), reader.GetInt32(7), reader.GetInt32(8), reader.GetInt32(9), reader.GetInt32(10)));
                        }
						if(CardList.Count > 0)
						{
							GD.Print("Card added!");
						}
					}
                }
			}
			return CardList;
		}
	}
	
	/*
	 * The following sections represent the solution for dynamically instancing cards with unique abilities
	 * A card ability dictionary has been created that has the CardName as well as the ability as a delegate
	 * This delegate is returned from the GetCardMethod() method that takes in a card and uses the card name to search the dictionary for the method
	 * This GetCardMethod method is employed in the UnitCard class, where (curently) there is a StatusEffect method
	 * This StatusMethod effect will trigger every round (every two turns, this system can be changed)
	 * 
	 * The solution for now is to fire a signal off one of the first card of the hand [in the BoardController] for triggering the status effect
	 * The first card in the hand has no bearing on the function of the signal, but to achieve better coupling/cohesion it may be necessary to move it since
	 * the ability that happens depends on the card that strikes the other card (this may be easier to do once the context menu is finished)
	 */
	public static Delegate GetCardMethod(Card c)
	{
		return cardAttackMethodDict[c.Name];
	}

	private static Dictionary<string, Delegate> cardAttackMethodDict = new Dictionary<string, Delegate>
	{
		{ "Burner", BurnAttack},
		{ "Boiler", BoilAttack},
		{ "Baker", BakeAttack }
	};

	//idk if this will work but we shall see
	public static void BurnAttack(BoardController b, UnitCard c)
	{
		int counter = 0;
		
		GD.Print("Health reduced");
		GD.Print($"Health is now {c.HP}");
	}

	public static void BoilAttack(BoardController b, UnitCard c)
	{

	}

	public static void BakeAttack(UnitCard c)
	{

	}
}
