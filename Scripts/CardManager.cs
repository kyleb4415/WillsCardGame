using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Runtime.CompilerServices;

public static class CardManager
{
    private static void InitialDealCards(List<Card> cards)
    {
        foreach (Card card in cards)
        {

        }
    }

	private static void ShuffleCards(List<Card> cards)
	{
		Random rand = new Random();
		for(int i = 0; i < cards.Count; i++)
		{
			Card temp = cards[i];
			cards[i] = cards[rand.Next(cards.Count)];
			cards[rand.Next(cards.Count)] = temp;
		}
	}

	public static void LoadCardsFromDB()
	{

		List<Card> CardList = new List<Card>();
		using (SQLiteConnection conn = new SQLiteConnection("res://DataStore/CardData.db"))
		{
			using(var command = new SQLiteCommand(conn))
			{
				command.CommandText = @"SELECT * FROM CARDS";
				using (var reader = command.ExecuteReader())
				{
                    while (reader.Read())
					{
						//use factory with reader values as inputs and then put into list that is returned at the end the method
						reader.GetInt32(1); //ID
						reader.GetString(2); //Name
						reader.GetBlob(3, true); //Image
						reader.GetString(4); //Description
						reader.GetString(5); //Type
						reader.GetBlob(6, true); //TypeImage
						reader.GetInt32(7); //Damage
						reader.GetInt32(8); //HP
						reader.GetInt32(9); //Unlocked
						reader.GetInt32(10); //ManaCost
					}
                }
			}
		}
	}
}
