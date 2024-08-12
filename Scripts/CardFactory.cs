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
		//cardInstance.CardImage = card.CardImage;
		
		var cardImages = instance.GetNodeOrNull<CardNode>("CardBase/CardImages");
		if(cardImages != null)
		{
            cardImages.SetCardName(card.CardName);
        }
		else
		{
			GD.Print("THAT CARD IMAGE SHIT WAS NULL");
		}

	}

	public static void CreateUnitCard(UnitCard card, Node instance)
	{

        var cardBodyInstancePreScript = instance.GetChild(0) as RigidBody3D;
        ulong objId = instance.GetInstanceId();
        cardBodyInstancePreScript.SetScript(ResourceLoader.Load<Script>("res://Scripts/UnitCard.cs"));

        Node3D cardBodyInstance = GodotObject.InstanceFromId(objId) as Node3D;
        UnitCard cardInstance = cardBodyInstance.GetChild(0) as UnitCard;
        GD.Print(cardInstance.GetType());


        cardInstance.CardName = card.CardName;
		cardInstance.Description = card.Description;
		cardInstance.Type = card.Type;
		cardInstance.ManaCost = card.ManaCost;
		//cardInstance.CardImage = card.CardImage;
		cardInstance.Damage = card.Damage;
		cardInstance.HP = card.HP;

        CardNode cardImages = cardInstance.GetNodeOrNull<CardNode>("CardBase/CardImages");

        if (cardImages != null)
        {
            cardImages.SetCardName(card.CardName);
        }
        
    }

    public static void CreateSkillCard(SkillCard card, Node instance)
    {
        var cardBodyInstancePreScript = instance.GetChild(0) as RigidBody3D;
        ulong objId = instance.GetInstanceId();
        cardBodyInstancePreScript.SetScript(ResourceLoader.Load<Script>("res://Scripts/SkillCard.cs"));

        Node3D cardBodyInstance = GodotObject.InstanceFromId(objId) as Node3D;
        SkillCard cardInstance = cardBodyInstance.GetChild(0) as SkillCard;
        GD.Print(cardInstance.GetType());

        cardInstance.CardName = card.CardName;
        cardInstance.Description = card.Description;
        cardInstance.Type = card.Type;
        cardInstance.ManaCost = card.ManaCost;
        //cardInstance.CardImage = card.CardImage;


        CardNode cardImages = instance.GetNodeOrNull<CardNode>("CardBase/CardImages");
        if (cardImages != null)
        {
            cardImages.SetCardName(card.CardName);
            GD.Print("Setcardname worked!");
        }
        else
        {
            GD.Print("Null cardimages!");
        }
    }

    public static void CreateExampleCard(Card card, Node instance)
    {
        Card cardInstance = instance.GetChild(0) as Card;
        cardInstance.CardName = "placeholder";
        cardInstance.Description = "placeholder";
        cardInstance.Type = "placeholder";
        cardInstance.ManaCost = 1;

        var cardImages = instance.GetNodeOrNull<CardNode>("CardBase/CardImages");
        if (cardImages != null)
        {
            cardImages.SetCardName(card.CardName);
        }
        else
        {
            GD.Print("THAT CARD IMAGE SHIT WAS NULL");
        }

	}

}
