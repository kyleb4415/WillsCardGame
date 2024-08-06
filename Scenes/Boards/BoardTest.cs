using Godot;
using System;
using System.Collections.Generic;
using static Godot.OpenXRInterface;

public partial class BoardTest : Node2D
{ 
	//may need to reconfigure and use resourcepreloader node

	public static readonly PackedScene packedScene = GD.Load<PackedScene>(@"res://Assets/Cards/CardBase.tscn");
	public static readonly PackedScene playerHand = GD.Load<PackedScene>(@"res://Scenes/Boards/BoardTest.tscn");
	public Script playerHandScript = GD.Load<Script>(@"res://Scenes/Boards/BoardTest.cs");

	private List<Node> _packedSceneInstances = new List<Node>();
	private Node instantiatedHand = new Node();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		instantiatedHand = playerHand.Instantiate() as Node2D;
		_packedSceneInstances.Add(instantiatedHand);
		//for testing purposes
		/*
		var cardBase = packedScene.Instantiate<Node>();
		Console.WriteLine("Resources in resource list: ");
		if(cardBase != null)
		{
			Console.WriteLine("CardBase loaded!");
		}
		*/

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Input(InputEvent @event)
	{
		//this control is mapped and named accordingly in project -> project settings -> input map
		if(Input.IsActionJustReleased("leftclick"))
		{
			var newCard = packedScene.Instantiate() as MarginContainer;

			newCard.Set("card_name", "Burner");
			Console.WriteLine(newCard.Get("card_name"));

			//newCard.Set("position", GetGlobalMousePosition());
			Console.WriteLine($"Position changed to {GetGlobalMousePosition().X}, {GetGlobalMousePosition().Y}");

			if(newCard is not null)
			{
				newCard.Set("position", GetGlobalMousePosition());
				instantiatedHand.GetNode<Node2D>("Cards").AddChild(newCard);
				_packedSceneInstances.Add(newCard);

				Console.WriteLine(instantiatedHand.GetChildren()[0].GetChildren().Count);
			}
			else
			{
				Console.WriteLine("Null instance!");
			}

			//look into cleanup for this
		}	
		//base._Input(@event);
	}

	//removes instances of cards on scene exit, scene transfer, etc. - may be necessary to change later depending on how we instance cards
	public override void _ExitTree()
	{
		foreach(var item in _packedSceneInstances)
		{
			item.QueueFree();
		}
		base._ExitTree();
	}
}
