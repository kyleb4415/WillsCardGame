using Godot;
using System;

public partial class CardAction : Node
{
	// Called when the node enters the scene tree for the first time.
	public UnitCard card1 { get; set; }
	public UnitCard card2 { get; set; }

	public bool AbilityUsed { get; set; }

	public CardAction(UnitCard c1, UnitCard c2)
	{
		card1 = c1;
		card2 = c2;
	}

	public CardAction()
	{
		card1 = new UnitCard();
		card2 = new UnitCard();
	}
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void ExecuteAction()
	{
		if(card1.State == CardState.Attacking)
		{
			DealDamage();
		}
		else if(card1.State == CardState.UsingAbility)
		{
			UseAbility();
		}
	}

	public void DealDamage()
	{
		card2.EmitSignal(UnitCard.SignalName.CardHit, card1.Damage);
		if(card2.HP <= 0)
		{
			card2.Visible = false;
		}
		card1.EmitSignal(Card.SignalName.CardSelected, card1);
        RotationHelper.ResetRotation(card1, card1.GetTree());
		RotationHelper.ResetRotation(card2, card2.GetTree());
        GD.Print($"{card1.Name} did {card1.Damage} damage to {card2.Name}.");
		card1.State = CardState.Idle;
	}

	//think about how to make an ability last multiple turns.... invoke on certain turn changed signal, increment, and then
	//when it hits certain value stop it?
	public void UseAbility()
	{
		var method = CardManager.GetCardMethod(card1);
        card1.EmitSignal(UnitCard.SignalName.Ability, card2);
        GD.Print($"{card1.Name} did {CardManager.GetCardMethod(card1).Method} on {card2.Name}.");
        card1.State = CardState.Idle;
		card1.EmitSignal(UnitCard.SignalName.CardSelected, card1);
	}
}
