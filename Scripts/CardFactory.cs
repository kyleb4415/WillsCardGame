using Godot;
using System;

public abstract class CardFactory
{
    protected abstract ICard MakeCard();
    protected abstract ICard MakeCard(Card card);

}
