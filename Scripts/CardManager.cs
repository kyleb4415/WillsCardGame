using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Runtime.CompilerServices;

public static class CardManager
{

    [Signal]
    public delegate void CardInteractionEventHandler(Card c, Card d);
    private static void InitialDealCards(List<Card> cards)
    {
        foreach (Card card in cards)
        {

        }
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
                                CardList.Add(new UnitCard(reader.GetInt32(1), reader.GetString(2), reader.GetBlob(3, true), reader.GetString(4), reader.GetString(5), reader.GetBlob(6, true), reader.GetInt32(7), reader.GetInt32(8), reader.GetInt32(9), reader.GetInt32(10)));
                            }
							catch(Exception e)
							{
								GD.Print(e.Message);
							}
                        }
						else
						{
						
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


}
