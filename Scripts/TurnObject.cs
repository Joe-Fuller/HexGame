using Godot;
using System;

public partial class TurnObject : Node
{
    public String Type;
    public Unit Unit;
    public Godot.Collections.Array<Vector2I> TargetTiles;

    public TurnObject(String Type = "NoType", Unit Unit = null, Godot.Collections.Array<Vector2I> TargetTiles = null)
    {
        this.Type = Type;
        this.Unit = Unit;
        this.TargetTiles = TargetTiles;
    }
}
