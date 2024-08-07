using Godot;
using System;

public partial class CardPopupMenu : PopupMenu
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.IdPressed += CardPopupMenu_IdPressed;
		UnitCard u = GetParent().GetParent<UnitCard>();
	}

    private void CardPopupMenu_IdPressed(long id)
    {
        switch(id)
        {
            case 0:
                UnitCard u = GetParent().GetParent() as UnitCard;
                u.State = CardState.Attacking;
                GD.Print($"{u.Name} Attack selected");
                break;
            case 1:
                UnitCard u2 = GetParent().GetParent() as UnitCard;
                u2.State = CardState.UsingAbility;

                //for unitcard strengthen/taunt/single abilities
                if(CardManager.GetCardMethod(u2).Method.GetParameters().Length == 2)
                {
                    u2.InvokeAbility(u2);
                    u2.EmitSignal(Card.SignalName.CardSelected, u2);
                    GetNode<MoveCard3D>("/root/GameBoard/Camera3D").currentGameState = GameState.SelectingCard;
                    GetNode<MoveCard3D>("/root/GameBoard/Camera3D").selectedCard = null;
                }
                GD.Print($"{u2.Name} ability selected");
                break;
            default:
                break;
        }
            
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


}
