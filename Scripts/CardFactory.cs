using Godot;
using System;

public static class CardFactory
{
	public static void CreateCard(Card card, Node instance)
	{
		Card cardInstance = instance.GetChild(0) as Card;
		cardInstance.CardName = card.CardName;
		cardInstance.Description = card.Description;
		cardInstance.Type = card.Type;
		cardInstance.ManaCost = card.ManaCost;
		cardInstance.CardImage = card.CardImage;
		
		// Get the reference to the GDScript node
		var cardNode = instance.GetNode("CardImages");
		// Call the GDScript function to set the card name and load the image
		cardNode.Call("set_card_name", card.CardName);
	}

	public static void CreateUnitCard(UnitCard card, Node instance)
	{
		UnitCard cardInstance = instance.GetChild(0) as UnitCard;
		cardInstance.CardName = card.CardName;
		cardInstance.Description = card.Description;
		cardInstance.Type = card.Type;
		cardInstance.ManaCost = card.ManaCost;
		cardInstance.CardImage = card.CardImage;
		cardInstance.Damage = card.Damage;
		cardInstance.HP = card.HP;
		
		// Get the reference to the GDScript node
		var cardNode = instance.GetNode("CardImages");
		// Call the GDScript function to set the card name and load the image
		cardNode.Call("set_card_name", card.CardName);
	}

}
