using Godot;
using System;

public class UnitPack : Node
{
    public Godot.Collections.Array<Vector2> Tiles;
    public Godot.Collections.Array<Unit> Units;

    public override void _Ready()
    {
        base._Ready();
        Tiles = new Godot.Collections.Array<Vector2>();
        Units = new Godot.Collections.Array<Unit>();
    }

    public void SetTiles(Godot.Collections.Array<Vector2> IncTiles)
    {
        Tiles = IncTiles;
    }

    public void SetUnits(Godot.Collections.Array<Unit> IncUnits)
    {
        Units = IncUnits;
    }
}
