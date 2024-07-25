using Godot;
using System;

public partial class CardPopupMenu : PopupMenu
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        this.IdPressed += CardPopupMenu_IdPressed;
        UnitCard u = GetParent().GetParent<UnitCard>();
        u.EmitSignal(Card.SignalName.CardSelected);
	}

    private void CardPopupMenu_IdPressed(long id)
    {
        switch(id)
        {
            case 0:
                UnitCard u = GetParent().GetParent() as UnitCard;
                u.Selected = true;
                u.State = CardState.Attacking;
                u.EmitSignal(Card.SignalName.CardSelected, u);
                GD.Print($"{u.Name} Attack selected");
                break;
            case 1:
                UnitCard u2 = GetParent().GetParent() as UnitCard;
                u2.EmitSignal(Card.SignalName.CardSelected, u2);
                u2.State = CardState.UsingAbility;
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
