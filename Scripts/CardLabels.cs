using Godot;
using System;

public partial class UnitCard2D : Node2D
{
	private UnitCard unitCard;
	private Label damageLabel;
	private Label hpLabel;
	private Label name;
	private Label mana;
	private Label type;
	private Label description;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Get the UnitCard 
		unitCard = GetNode<UnitCard>("res://Scenes3D/CardBase3D/CardBody");

		// Get the labels from the Control node
		damageLabel = GetNode<Label>("Damage");
		hpLabel = GetNode<Label>("HP");
		name = GetNode<Label>("Name");
		mana = GetNode<Label>("ManaCost");
		description = GetNode<Label>("Description");

		UpdateLabels();
	}

	private void UpdateLabels()
	{
		// Update the labels with the values from the UnitCard instance
		damageLabel.Text = unitCard.Damage.ToString();
		hpLabel.Text = unitCard.HP.ToString();
		name.Text = unitCard.CardName.ToString();
		mana.Text = unitCard.ManaCost.ToString();
		description.Text = unitCard.Description.ToString();

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		UpdateLabels();
	}
}
