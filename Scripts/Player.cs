using Godot;
using System;

public partial class Player : Node
{
    public Godot.Collections.Array<Unit> Units;
    public int Money = 10;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    public void AddUnit(Unit Unit)
    {
        Units.Add(Unit);
    }
}
