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
    public Dictionary playerColliders;

    public dynamic selectedCard;
    public dynamic targetCard;

    //use this in case all actions happen at the end of the turn
    public List<CardAction> cardActions { get; set; } = new List<CardAction>();

    public Vector2 mouse;
    public Vector2 screenSize;
    public bool queueFree = false;
    public GameState currentGameState;
    public BoardController boardController;
    public float MouseCastLength = 9.0f;
    public float CardFollowDistance = 3.5f;

    [Signal]
    public delegate void HoverCardEventHandler(Card c);

    public override void _Ready()
    {
        mouse = new Vector2();
        boardController = this.GetParent<BoardController>();
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
                    InitialAction(@event);
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


    private void InitialAction(InputEvent @event)
    {
        if (RaycastHelper.GetCollisionPoint(this, mouse, MouseCastLength)["collider"].AsGodotObject().GetType() != typeof(StaticBody3D))
        {
            colliders = RaycastHelper.GetCollisionPoint(this, mouse, MouseCastLength);
            Card c = (Card)colliders["collider"];

            if (c != null && !c.CanPickUp && !boardController.Hand.Contains(c) && c.CardAlignmentType != Card.CardAlignment.Enemy)
            {
                currentGameState = GameState.SelectingCard;
                CardInteractNoContextMenu(@event);
            }
            else if (selectedCard != null && !boardController.Enemy.EnemyHand.Contains(c))
            {
                currentGameState = GameState.ExecutingAction;
            }
            else if (!c.CanPickUp)
            {
                currentGameState = GameState.SelectingCard;
                CardInteractNoContextMenu(@event);
            }
        }
        else
        {
            playerColliders = RaycastHelper.GetCollisionPoint(this, mouse, MouseCastLength);
            //try logic here for player collider stuff like get node name and such
        }
    }


    /// <summary>
    /// Handles card placement on first placing onto the board
    /// Path 1 - Card is the first to be placed onto the board and there is no selectedCard
    /// Path 2 - Card has been placed and is now being clicked again, leading the event to be relayed to the selectcard stage
    /// Path 3 - Card has been selected so gameState has to jump back to the AwaitingTarget phase
    /// </summary>

    private void PlaceCard(InputEvent @event)
    {
        if (colliders != null)
        {
            Card card = (Card)colliders["collider"];
            if (card.CanPickUp && card.CardAlignmentType != Card.CardAlignment.Enemy)
            {
                //card.EmitSignal(Card.SignalName.PlaceCard, card);
                if(card.PlacedPos != new Vector3(0, 0, 0) && this.GetParent().GetNode<TextureProgressBar>("ManaBar").Value >= card.ManaCost * 100)
                {
                    Tween tween = CreateTween();
                    tween.TweenProperty(card, "position", card.PlacedPos, 0.5f).SetTrans(Tween.TransitionType.Quad);
                    tween.Finished += () =>
                    {
                        card.EmitSignal(Card.SignalName.PlaceCard, card, this.GetParent().GetNode("ManaBar"));
                    };
                    boardController.Hand.Remove(card);
                    currentGameState = GameState.SelectingCard;
                    GD.Print("Placing");

                    CardManager.DealPlayerCardAnimation(boardController.PlayerDeck[boardController.PlayerDeck.Count - 1], boardController, new Vector3(0,0,0), this.GetTree());

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
            if(selectedCard != null)
            {
                GD.Print("awaitingtarget");
                currentGameState = GameState.AwaitingTarget;
            }
        }
    }

    /// <summary>
    /// Handles card interaction WITH context menu, allowing a regular attack or use of an ability.
    /// Path 1 - Card is not already selected in the selectedCard variable, thus adding it to the selectedCard variable and progressing the state machine.
    /// Path 2 - Card is already the selectedCard, thus selecting it again will deselect it and will cause the state machine to stay in the same state.
    /// </summary>
    private void CardInteractContextMenu(InputEvent @event)
    {
        colliders = RaycastHelper.GetCollisionPoint(this, mouse, MouseCastLength);
        if (colliders["collider"].AsGodotObject().GetType() == typeof(UnitCard))
        {
            UnitCard cardToBeSelected = (UnitCard)colliders["collider"];
            if (selectedCard != cardToBeSelected)
            {
                selectedCard = (UnitCard)colliders["collider"];
                if (CardManager.GetCardMethod(selectedCard) != null)
                {
                    if (!selectedCard.CanPickUp && !selectedCard.Selected)
                    {
                        selectedCard.Selected = true;
                        GD.Print("card selected context menu " + selectedCard.Selected);
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
            else
            {
                selectedCard.EmitSignal(Card.SignalName.CardSelected, selectedCard);
                selectedCard = null;
            }

        }
    }

    /// <summary>
    /// Handles card interaction without context menu, just clicking on the cards makes them attack and not use special ability (unless passive)
    /// Path 1 - Card is not already selected in the selectedCard variable, thus adding it to the selectedCard variable and progressing the state machine.
    /// Path 2 - Card is already the selectedCard, thus selecting it again will deselect it and will cause the state machine to stay in the same state.
    /// </summary>
    private void CardInteractNoContextMenu(InputEvent @event)
    {
        colliders = RaycastHelper.GetCollisionPoint(this, mouse, MouseCastLength);
        if (colliders["collider"].AsGodotObject().GetType() == typeof(UnitCard))
        {
            UnitCard cardToBeSelected = (UnitCard)colliders["collider"];
            if(boardController.Hand.Contains(cardToBeSelected))
            {
                currentGameState = GameState.PlacingCard;
            }
            else if(selectedCard != cardToBeSelected)
            {
                selectedCard = (UnitCard)colliders["collider"];
                if (!selectedCard.CanPickUp && !selectedCard.Selected)
                {
                    selectedCard.EmitSignal(Card.SignalName.CardSelected, selectedCard);
                    currentGameState = GameState.AwaitingTarget;
                    selectedCard.State = CardState.Attacking;
                }
            }
            else
            {
                selectedCard.EmitSignal(Card.SignalName.CardSelected, selectedCard);
                selectedCard = null;
                currentGameState = GameState.PlacingCard;
            }
        }
    }

    /// <summary>
    /// Selects target card that selectedCard will attack
    /// Path 1 - Selected card is NOT in the hand and it will be selected as the target card for the action to be performed against.
    /// Path 2 - Selected target card IS in the hand it will NOT be selected as the target, but will instead undergo placement.
    /// </summary>
    
    private void SelectTarget(InputEvent @event)
    {
        colliders = RaycastHelper.GetCollisionPoint(this, mouse, MouseCastLength);
        if (colliders["collider"].AsGodotObject().GetType() == typeof(UnitCard))
        {
            targetCard = (UnitCard)colliders["collider"];
            UnitCard c = (UnitCard)targetCard; 
            if(!boardController.Hand.Contains(c))
            {
                if (targetCard != selectedCard && targetCard.CardAlignmentType == Card.CardAlignment.Enemy && targetCard.CanPickUp == false && !boardController.Enemy.EnemyHand.Contains(targetCard))
                {
                    currentGameState = GameState.ExecutingAction;
                    colliders = null;
                }
                else if (targetCard == selectedCard)
                {
                    CardInteractNoContextMenu(@event);
                    currentGameState = GameState.SelectingCard;
                }
                else if (targetCard.CardAlignmentType == Card.CardAlignment.Player)
                {
                    //deselect previous selected card
                    selectedCard.EmitSignal(Card.SignalName.CardSelected, selectedCard);
                    selectedCard = targetCard;

                    //select new selectedcard
                    selectedCard.EmitSignal(Card.SignalName.CardSelected, selectedCard);
                    targetCard = null;
                    currentGameState = GameState.PlacingCard;
                }
            }
            else
            {
                currentGameState = GameState.PlacingCard;
                colliders = RaycastHelper.GetCollisionPoint(this, mouse, MouseCastLength);
            }

        }
        else if(colliders["collider"].AsGodotObject().GetType() == typeof(StaticBody3D))
        {
            StaticBody3D collider = (StaticBody3D)colliders["collider"];
            if(collider == GetNode<StaticBody3D>("/root/GameBoard/EnemyBody/EnemyBodyObject"))
            {
                boardController.Enemy.Health -= selectedCard.Damage;
                GD.Print(boardController.Enemy.Health);
                selectedCard.EmitSignal(Card.SignalName.CardSelected, selectedCard);
                currentGameState = GameState.PlacingCard;
                selectedCard = null;
                collider = null;
            }
            else
            {
                collider = null;
            }
        }
    }

    /// <summary>
    /// Executes action, whether it be attacking or using an ability
    /// </summary>
    private void ExecuteAction()
    {
        //this if statement will be unnecessary once enemy is implemented
        if (!targetCard.CanPickUp)
        {
            CardAction action = new CardAction(selectedCard, targetCard);
            action.ExecuteAction();
            currentGameState = GameState.PlacingCard;
            selectedCard = null;
            targetCard = null;
        }
    }

    /// <summary>
    /// Continues game after ability phase if needed
    /// </summary>
    private void ContinueGameAfterAbility()
    {
        currentGameState = GameState.PlacingCard;
    }

    /// <summary>
    /// Moves card 
    /// </summary>
    /// <param name="colliders">Represents the card object that's being moved</param>
    /// <param name="delta">Represents the physicsprocess time</param>
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
                        //TODO: 
                        //emit remove from hand signal so awaitingtarget logic will work
                        //will be replaced into hand if it's not put in a spot
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

                    if (colliderToMove.CanPickUp == true && colliderToMove.CardAlignmentType != Card.CardAlignment.Enemy)  
                    {
                        RotationHelper.ResetRotation(colliderToMove, this.GetTree());
                        colliderToMove.Set("gravity_scale", 0);
                        colliderToMove.IsPickedUp = true;
                        colliderToMove.Position = colliderToMove.Position.Lerp(ProjectPosition(mouse, CardFollowDistance), (float)delta * 10);
                        boardController.Hand.Remove(colliderToMove);
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

    /// <summary>
    /// Lerps card into the area if it's released while into the area
    /// </summary>
    /// <param name="body">Body entering the area</param>
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

    /// <summary>
    /// Removes card lerping if it exits the area
    /// </summary>
    /// <param name="body">Represents card body exiting area</param>
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
