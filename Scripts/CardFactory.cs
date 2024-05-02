using Godot;
using System;

public abstract class CardFactory
{
    protected abstract Card MakeCard();
    protected abstract Card MakeCard(Card card);

}
