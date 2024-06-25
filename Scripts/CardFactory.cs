using Godot;
using System;

public static class CardFactory
{
    public static void CreateCard(Card card, Node instance)
    {
        Card cardInstance = instance.GetChild(0) as Card;
        cardInstance.Name = card.Name;
        cardInstance.Description = card.Description;
        cardInstance.Type = card.Type;
        cardInstance.ManaCost = card.ManaCost;
        cardInstance.CardImage = card.CardImage;
    }

    public static void CreateUnitCard(UnitCard card, Node instance)
    {
        UnitCard cardInstance = instance.GetChild(0) as UnitCard;
        cardInstance.Name = card.Name;
        cardInstance.Description = card.Description;
        cardInstance.Type = card.Type;
        cardInstance.ManaCost = card.ManaCost;
        cardInstance.CardImage = card.CardImage;
        cardInstance.Damage = card.Damage;
        cardInstance.HP = card.HP;
    }

}
