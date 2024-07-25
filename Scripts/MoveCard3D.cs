using Godot;
using Godot.Collections;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Threading.Tasks;

public partial class MoveCard3D : Camera3D
{
    public Dictionary colliders;

    public dynamic selectedCard;
    public dynamic targetCard;

    //use this in case all actions happen at the end of the turn
    public List<CardAction> cardActions { get; set; } = new List<CardAction>();

    public Vector2 mouse;
    public Vector2 screenSize;
    public bool queueFree = false;
    public GameState currentGameState;
    public BoardController boardController;

    [Signal]
    public delegate void HoverCardEventHandler(Card c);

    public override void _Ready()
    {
        mouse = new Vector2();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        if (colliders != null)
        {
            MoveColliders(colliders, delta);
        }
        base._PhysicsProcess(delta);
    }


    public override void _Process(double delta)
    {
        //screenSize = GetViewport().GetVisibleRect().Size;

        GameState gameState = this.GetParent<BoardController>().gameState;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion)
        {
            mouse = (Vector2)@event.Get("position");
        }
        switch (currentGameState)
        {
            case (GameState.PlacingCard):
                if (@event is InputEventMouseButton && @event.IsActionPressed("leftclick"))
                {
                    colliders = RaycastHelper.GetCollisionPoint(this, mouse, 3.0f);
                }
                else if (@event is InputEventMouseButton && @event.IsActionReleased("leftclick"))
                {
                    GD.Print("placing card");
                    PlaceCard(@event);
                }
                if (@event is InputEventMouseButton && @event.IsActionPressed("rightclick"))
                {
                    GD.Print("picking card");
                    CardInteractContextMenu(@event);
                }
                break;
            case (GameState.SelectingCard):
                if (@event is InputEventMouseButton && @event.IsActionPressed("leftclick"))
                {
                    CardInteractNoContextMenu(@event);
                }
                else if (@event is InputEventMouseButton && @event.IsActionPressed("rightclick"))
                {
                    CardInteractContextMenu(@event);
                }
                break;
            case (GameState.AwaitingTarget):
                if (@event is InputEventMouseButton && @event.IsActionPressed("leftclick"))
                {
                    SelectTarget(@event);
                }
                break;
            case (GameState.ExecutingAction):
                ExecuteAction();
                break;
        }
        base._Input(@event);
    }

    private void PlaceCard(InputEvent @event)
    {
        colliders = RaycastHelper.GetCollisionPoint(this, mouse, 3.0f);
        if (colliders["collider"].AsGodotObject().GetType() != typeof(StaticBody3D))
        {
            Card card = (Card)colliders["collider"];
            if (card.CanPickUp)
            {
                //card.EmitSignal(Card.SignalName.PlaceCard, card);
                if(card.PlacedPos != default)
                {
                    Tween tween = CreateTween();
                    tween.TweenProperty(card, "position", card.PlacedPos, 0.5f).SetTrans(Tween.TransitionType.Quad);
                    tween.Finished += () =>
                    {
                        card.EmitSignal(Card.SignalName.PlaceCard, card, this.GetParent().GetNode("ManaBar"));
                    };
                }
                else
                {
                    Tween tween = CreateTween();
                    Tween tween2 = CreateTween();
                    tween.TweenProperty(card, "position", card.OriginPos, 0.5f).SetTrans(Tween.TransitionType.Quad);
                    tween2.TweenProperty(card, "rotation", card.OriginRot, 0.5f).SetTrans(Tween.TransitionType.Quad);
                    card.GravityScale = 0;
                    colliders = null;
                }
            }
            else
            {
                currentGameState = GameState.SelectingCard;
                CardInteractNoContextMenu(@event);
            }
        }
    }

    private void CardInteractContextMenu(InputEvent @event)
    {
        colliders = RaycastHelper.GetCollisionPoint(this, mouse, 3.0f);
        if (colliders["collider"].AsGodotObject().GetType() == typeof(UnitCard))
        {
            selectedCard = (UnitCard)colliders["collider"];
            if (CardManager.GetCardMethod(selectedCard) != null)
            {
                if (!selectedCard.CanPickUp && !selectedCard.Selected)
                {
                    Node2D contextMenu = ResourceLoader.Load<PackedScene>("res://Scenes/CardContextMenu.tscn").Instantiate<Node2D>();
                    contextMenu.GetChild(0).Set("position", mouse);
                    selectedCard.AddChild(contextMenu);
                    currentGameState = GameState.AwaitingTarget;
                }
                else
                {
                    selectedCard.State = CardState.Idle;
                }
            }
        }
    }

    private void CardInteractNoContextMenu(InputEvent @event)
    {
        colliders = RaycastHelper.GetCollisionPoint(this, mouse, 3.0f);
        if (colliders["collider"].AsGodotObject().GetType() == typeof(UnitCard))
        {
            selectedCard = (UnitCard)colliders["collider"];
            if (!selectedCard.CanPickUp && !selectedCard.Selected)
            {
                selectedCard.EmitSignal(Card.SignalName.CardSelected, selectedCard);
                currentGameState = GameState.AwaitingTarget;
                selectedCard.State = CardState.Attacking;
            }
            else
            {

            }
        }
    }

    private void SelectTarget(InputEvent @event)
    {
        colliders = RaycastHelper.GetCollisionPoint(this, mouse, 3.0f);
        if (colliders["collider"].AsGodotObject().GetType() == typeof(UnitCard))
        {
            targetCard = (UnitCard)colliders["collider"];
            if (targetCard != selectedCard)
            {
                currentGameState = GameState.ExecutingAction;
            }
            else if(targetCard == selectedCard)
            {
                CardInteractNoContextMenu(@event);
                currentGameState = GameState.SelectingCard;
            }
            else if(targetCard.CanPickUp == true)
            {
                //do something here, need to decide
            }
        }
        else
        {
            currentGameState = GameState.SelectingCard;
        }
    }

    private void ExecuteAction()
    {
        //this if statement will be unnecessary once enemy is implemented
        if (!targetCard.CanPickUp)
        {
            CardAction action = new CardAction(selectedCard, targetCard);
            action.ExecuteAction();
            currentGameState = GameState.PlacingCard;
        }

    }

    private void ContinueGameAfterAbility()
    {
        currentGameState = GameState.PlacingCard;
    }

    private void MoveColliders(Dictionary colliders, double delta)
    {
        try
        {
            if (colliders.Count > 0)
            {
                if (colliders["collider"].AsGodotObject().GetType() != typeof(StaticBody3D))
                {
                    dynamic colliderToMove;
                    if (colliders["collider"].AsGodotObject().GetType() == typeof(UnitCard))
                    {
                        colliderToMove = (UnitCard)colliders["collider"];
                    }
                    else if (colliders["collider"].AsGodotObject().GetType() == typeof(SkillCard))
                    {
                        colliderToMove = (SkillCard)colliders["collider"];
                    }
                    else if (colliders["collider"].AsGodotObject().GetType() == typeof(Card))
                    {
                        colliderToMove = (Card)colliders["collider"];
                    }
                    else
                    {
                        return;
                    }

                    if (colliderToMove.CanPickUp == true)
                    {
                        RotationHelper.ResetRotation(colliderToMove, this.GetTree());
                        colliderToMove.Set("gravity_scale", 0);
                        colliderToMove.IsPickedUp = true;
                        colliderToMove.Position = colliderToMove.Position.Lerp(ProjectPosition(mouse, 2.5f), (float)delta * 10);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            GD.Print("Invalid cast!");
            GD.Print(ex.Message);
        }
    }

    //this is the event for the area3d colliders, should lerp card to space
    public void Area_OnBodyEntered(Node3D body)
    {
        //reconfigure to use signals so it can fix card automatically going to space
        foreach (var s in ((BoardController)this.GetParentNode3D()).cardSpaceInstances)
        {
            Area3D area = s.GetChild(0) as Area3D;

            if (area.GetOverlappingBodies().Count > 1 && area.GetOverlappingBodies().Count < 3)
            {
                Card bodyAsCard = area.GetOverlappingBodies()[1] as Card;
                if (bodyAsCard.CanPickUp == true)
                {
                    bodyAsCard.PlacedPos = new Vector3(area.GetParentNode3D().GetParentNode3D().Position.X, -1.5f, area.GetParentNode3D().GetParentNode3D().Position.Z);
                }
            }
        }
    }

    public void Area_OnBodyExited(Node3D body)
    {
        Card cardBody = (Card)body;
        cardBody.PlacedPos = default;
        cardBody.CanPickUp = true;
    }

    public override void _ExitTree()
    {
        //queueFree = true;
        this.QueueFree();
        colliders.Clear();
        base._ExitTree();
    }
}
