using Godot;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

public partial class BoardController : Node3D
{
	// Called when the node enters the scene tree for the first time.

	private TextureProgressBar _textureProgressBar;
    public readonly Script gameSpaceScript = ResourceLoader.Load<Script>("res://Scripts/MoveCard3D.cs");
    public Script cardScript = ResourceLoader.Load<Script>("res://Scripts/UnitCard.cs");
    public readonly PackedScene cardSpace = ResourceLoader.Load<PackedScene>("res://Scenes3D/CardSpaceBase.tscn");
    public readonly PackedScene cardBase = ResourceLoader.Load<PackedScene>("res://Scenes3D/CardBase3D.tscn");
    public List<Node> cardGameObjects = new List<Node>();
    public List<Card> Hand { get; set; } = new List<Card>();
    public int TurnNum;
    private Label _turnNumberLabel;
    private Label _timerLabel;
    private Timer _timer = new Timer();
    public bool PlayerTurn;
    private Button _endTurnButton;

    //finish setting this up
    [Signal]
    public delegate void TurnEndedEventHandler();
    public override void _Ready()
	{
		PrepareBoard();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        _timerLabel.Set("text", Math.Round(_timer.TimeLeft).ToString());
	}

	private void PrepareBoard()
	{
		PrepareUI();
        AddCardSpaces();
        AddCards();
        CardManager.InitialDealCards(this, this.GetTree());
    }

	private void PrepareUI()
	{
        _textureProgressBar = this.GetNode("ManaBar") as TextureProgressBar;
        _textureProgressBar.FillMode = (int)TextureProgressBar.FillModeEnum.LeftToRight;
        _textureProgressBar.MinValue = 0;
        _textureProgressBar.MaxValue = 1000;
        _textureProgressBar.Value = 500;
        _turnNumberLabel = this.GetNode("TurnLabel").GetChild(0) as Label;
        _turnNumberLabel.Set("text", TurnNum.ToString());
        _endTurnButton = this.GetNode("TurnButton") as Button;
        _endTurnButton.Pressed += _endTurnButton_Pressed;
        _timerLabel = this.GetNode("TimerDisplay") as Label;
        _timer = this.GetNode("TimerDisplay").GetChild(0) as Timer;
        _timer.Timeout += _endTurnButton_Pressed;
        _timer.Start(12);
    }

    private void _endTurnButton_Pressed()
    {
        _textureProgressBar.Value += 100D;
        GD.Print("Timed out!");
        TurnNum++;
        _turnNumberLabel.Set("text", TurnNum.ToString());
        if (TurnNum % 2 != 0)
        {
            PlayerTurn = true;
            EmitSignal(SignalName.TurnEnded);
        }
        else
        {
            PlayerTurn = false;
        }
        _timer.Start(12);
    }

    public Node3D cardSpaceInstanceParent;
    public Area3D cardSpaceInstanceChild;
    public List<Node3D> cardSpaceInstances;
    private void AddCardSpaces()
	{
        var gameSpace = this;
        cardSpaceInstances = new List<Node3D>();

        //loading card spaces onto the points defined on the board
        //-------------------------------------------------------------------------------
        foreach (var space in GetNode("/root/GameBoard/BoardPositions").GetChildren())
        {
            cardSpaceInstanceParent = cardSpace.Instantiate() as Node3D;
            cardSpaceInstanceChild = cardSpaceInstanceParent.GetChild(0) as Area3D;
            cardSpaceInstances.Add(cardSpaceInstanceParent);
            space.CallDeferred("add_child", cardSpaceInstanceParent);
        }
        //-------------------------------------------------------------------------------

        //attaching events for card spaces
        //-------------------------------------------------------------------------------
        if (cardSpaceInstances is not null)
        {
            foreach (var s in cardSpaceInstances)
            {
                //attaches event for each cardspace

                var sChild = s.GetChild(0) as Area3D;
                sChild.BodyEntered += ((MoveCard3D)GetNode("Camera3D")).Area_OnBodyEntered;
                sChild.BodyExited += ((MoveCard3D)GetNode("Camera3D")).Area_OnBodyExited;
            }
        }
        //-------------------------------------------------------------------------------
    }

    private void AddCards()
    {
        //attaching events for card TODO: DELETE LATER
        //-------------------------------------------------------------------------------
        //Card card = (Card)GetNode("/root/GameBoard/CardBody");

        //card.MouseEntered += ((MoveCard3D)GetNode("Camera3D")).Card_MouseEntered;
        //card.CardReleased += card.Release;
        //card.CardSelected += card.Select;
        //-------------------------------------------------------------------------------

        //instancing cards from db 
        //-------------------------------------------------------------------------------
        List<UnitCard> cards = CardManager.LoadCardsFromDB();
        foreach (var c in cards)
        {
            //modify before instantiation
            var cardBaseInstance = cardBase.Instantiate();
            cardGameObjects.Add(cardBaseInstance);
            CardFactory.CreateUnitCard(c, cardBaseInstance);
            UnitCard unitCard = cardBaseInstance.GetChild(0) as UnitCard;
            //unitCard.MouseEntered += ((MoveCard3D)GetNode("Camera3D")).Card_MouseEntered;
            unitCard.CardReleased += unitCard.Release;
            unitCard.CardSelected += unitCard.Select;
            unitCard.CardHit += unitCard.TakeDamage;
            unitCard.Position += new Vector3(1, 1, 1);
            this.GetParent().CallDeferred("add_child", cardBaseInstance);
            Hand.Add(unitCard);
        }
        //-------------------------------------------------------------------------------
    }

    //TODO: Create method to instance cards to a certain location (fan? laid out in front?)
    //-------------------------------------------------------------------------------

    public override void _ExitTree()
    {
        cardSpaceInstanceChild.QueueFree();
        this.QueueFree();
        foreach (var c in cardSpaceInstances)
        {
            c.QueueFree();
        }
        foreach (var c in cardGameObjects)
        {
            c.QueueFree();
        }
        base._ExitTree();
    }
}
