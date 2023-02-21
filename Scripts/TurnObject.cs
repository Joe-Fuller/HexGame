using Godot;
using System;

public class TurnObject : Node
{
    public String Type;
    public Unit Unit;
    public Godot.Collections.Array<Vector2> TargetTiles;

    public TurnObject(String Type = "NoType", Unit Unit = null, Godot.Collections.Array<Vector2> TargetTiles = null)
    {
        this.Type = Type;
        this.Unit = Unit;
        this.TargetTiles = TargetTiles;
    }
}
