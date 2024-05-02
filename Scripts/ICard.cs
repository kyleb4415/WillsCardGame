using Godot;
using System;

public interface ICard
{
    string CardName { get; set; }
    string Type { get; set; }
    int ManaCost { get; set; }

}
