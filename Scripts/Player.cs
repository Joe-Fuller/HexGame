using Godot;
using System;

public class Player : Node
{
	public Godot.Collections.Array<Unit> Units;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	public void AddUnit(Unit Unit)
	{
		Units.Add(Unit);
	}
}
