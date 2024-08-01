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
            c.GravityScale = 0;
			Tween t = s.CreateTween();
			Tween t2 = s.CreateTween();
            t.TweenProperty(c, "position", fanPositionLeft.Lerp(fanPositionRight, alignmentWeight), timeCalculation).SetTrans(Tween.TransitionType.Quad);
			t2.TweenProperty(c, "rotation", fanRotationLeft.Lerp(fanRotationRight, alignmentWeight), 0.25f).SetTrans(Tween.TransitionType.Quad);
			c.OriginPos = fanPositionLeft.Lerp(fanPositionRight, alignmentWeight);
			c.OriginRot = fanRotationLeft.Lerp(fanRotationRight, alignmentWeight);
			idx += 1;
        }
	}

    public static void InitialDealEnemyCardsAnimation(BoardController b, Vector3 dealPosition, SceneTree s)
    {
        Vector3 fanPositionLeft = new Vector3(-0.1f, -1f, 1f);
        Vector3 fanPositionRight = new Vector3(0.1f, -1f, 1f);

        //maybe change positions to accommodate more cards if necessary?
        if (b.Enemy.EnemyHand.Count > 4)
        {

        }

        Vector3 fanRotationLeft = new Vector3(0, 0.25f, 0);
        Vector3 fanRotationRight = new Vector3(0, -0.25f, 0);

        float idx = 0;

        foreach (Card c in b.Enemy.EnemyHand)
        {
            float alignmentWeight = CalculateCardAlignment(b.Hand, idx);
            float timeCalculation = 2f * (c.Position.X - fanPositionLeft.X);
            c.GravityScale = 0;
            Tween t = s.CreateTween();
            Tween t2 = s.CreateTween();
            t.TweenProperty(c, "position", fanPositionLeft.Lerp(fanPositionRight, alignmentWeight), timeCalculation).SetTrans(Tween.TransitionType.Quad);
            t2.TweenProperty(c, "rotation", fanRotationLeft.Lerp(fanRotationRight, alignmentWeight), 0.25f).SetTrans(Tween.TransitionType.Quad);
            c.OriginPos = fanPositionLeft.Lerp(fanPositionRight, alignmentWeight);
            c.OriginRot = fanRotationLeft.Lerp(fanRotationRight, alignmentWeight);
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

	public static List<ICard> LoadCardsFromDB()
	{
		List<ICard> CardList = new();
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
								//2 - CardName
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
	public static Dictionary<string, string> cardAbilityDescriptionDict = new Dictionary<string, string>
	{
		{ "Burner", "Burns enemy for one turn for one damage"},
		{ "Boiler", "Steams one enemy for two turns for one damage" },
		{ "The First Creation", "Infection Spread - Creatures that do any or have any action done from this card will be infected and take 1 damage a round (Can stack)"},
		{ "Taken Commander", "Summon" },
		{ "Infected Soldier", "Infection Spread - Creatures that do any or have any action done from this card will be infected and take 1 damage a round (Can stack)" },
		{ "The First Infected", "Taunt - All creatures must attack this card or any other card with taunt before they can attack anything else"},
		{ "The Taken Dog",  "Double Tap - Card can attack twice" },
		{ "Thrall", "Sacrifice - As long as this card survives a round it can be discarded from the field and have the stats added to another card" },
		{ "Goliath", "No ability" },
		{ "Infected Archer", "Double Tap - Card can attack twice" },
		{ "Evolution", "Stat Buff - Give this card's stats to another card" },
		{ "Armor", "Combine + Excess - Combine this card with another card any excess damage after death is given to the card attacked by it" },
		{ "Greedy Grinner", "(Passive) Graverobber - For every enemy card that dies, gain two coins. When empowered gain four." },
		{ "Goblin King", "(Passive & Active) Dubbing - Spawn a goblin knight every turn. When empowered a card of your choosing will be knighted and gain 3hp and 1 attack that lasts until it dies (?)" },
		{ "Goblin Knight", "(Passive) Taunt - Protect the king! Taunts all cards attacking the Goblin King, making the card attack the knight. When empowered this card gains 1 attack and 2 health." },
		{ "The Black Market", "Buy - Your gold is welcome here. Buy random one time use spells. " },
		{ "Time is Money (bm ability)", "Time is Money - Time slows down for you. Double the time on your next turn" },
		{ "Money is Power (bm ability)", "Money is Power - A goblin of random hp will sacrifice themselves for a card of your choosing. Enemy player does not know which card has been chosen." },
		{ "Goblin Rouge", "(Passive) Sneaky Sneaky - Every turn gain one coin. When empowered \"steal\" one mana." },
		{ "Boulder Goblin", "Rockfall - Roll a boulder down a lane, stunning the first card it hits and dealing two damage to the rest. When empowered stun all cards in the lane." },
		{ "Trade Prince", "Blackmail - When empowered, silence a card of your choosing. This card cannot use their abilities (active or passive) for one turn." },
		{ "Alchemist", "Cure - When empowered, this card will throw a flask that cleanse's a card of your choosing. This will remove all status effects." }

	};
	public static Delegate GetCardMethod(Card c)
	{
		return cardAttackMethodDict[c.CardName];
	}

	private static Dictionary<string, Delegate> cardAttackMethodDict = new Dictionary<string, Delegate>
	{
		{ "Burner", BurnAttack},
		{ "Boiler", BoilAttack},
		{ "Baker", BakeAttack },
		{ "Goblin King", Dubbing },
	};

	//Status effects reference - first index is damage and second index is turns
	public static void BurnAttack(BoardController b, UnitCard c)
	{
		c.StatusEffects.Add("Burn", new int[] { 1, 1 });
		GD.Print("Health reduced");
		GD.Print($"Health is now {c.HP}");
	}

	//burner abilities
	//-----------------------------------------------------------------------------------------

	public static void BoilAttack(BoardController b, UnitCard c)
	{

	}

	public static void BakeAttack(BoardController b, UnitCard c)
	{

	}
    //burner abilities end
    //-----------------------------------------------------------------------------------------

    //infection abilities
    //-----------------------------------------------------------------------------------------
    public static void InfectionSpread(BoardController b, UnitCard c)
	{
		c.StatusEffects.Add("Infection", new int[] { 1, 9999 });
	}

	public static void Taunt(BoardController b, UnitCard c)
	{

	}

	public static void DoubleTap(BoardController b, UnitCard c)
	{

	}

    public static void Sacrifice(BoardController b, UnitCard c)
    {

    }

	public static void CombineExcess(BoardController b, UnitCard c)
	{

	}



    //infection abilities end
    //-----------------------------------------------------------------------------------------


    //goblin abilities
    //-----------------------------------------------------------------------------------------
	//to avoid multiple cards being dubbed could add a status effect to the card "Dubbing" that prevents it from dubbing another card unless the card being dubbed dies?
    public static void Dubbing(BoardController b, UnitCard c)
    {
		RichTextLabel t = new RichTextLabel();
		t.AppendText($"{c.Name} knighted!");
		b.AddChild(t);
		c.Damage += 1;
		c.HP += 1;
		c.UpdateDamage();
		c.UpdateHP();
		c.Selected = false;
    }

	//black market special ability - return random spell card for gold
	public static void Buy(BoardController b, UnitCard c)
	{

	}

	public static void TimeIsMoney(BoardController b, UnitCard c)
	{

	}

	public static void MoneyIsPower(BoardController b, UnitCard c)
	{

	}

	public static void Rockfall(BoardController b, UnitCard c)
	{

	}

	public static void Blackmail(BoardController b, UnitCard c)
	{

	}

	public static void Cure(BoardController b, UnitCard c)
	{

	}
}
