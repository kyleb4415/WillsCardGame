using Godot;
using System;

public enum GameState
{
    PlacingCard,
    SelectingCard,
    AwaitingTarget,
    ExecutingAction,
    Ability,
    EnemyTurn
}

public enum AbilityPhase
{
    None,
    Taunt
}
