using Godot;
using System;

public partial class UnitPack : Node
{
    public Godot.Collections.Array<Vector2I> Tiles;
    public Godot.Collections.Array<Unit> Units;

    public override void _Ready()
    {
        base._Ready();
        Tiles = new Godot.Collections.Array<Vector2I>();
        Units = new Godot.Collections.Array<Unit>();
    }

    public void SetTiles(Godot.Collections.Array<Vector2I> IncTiles)
    {
        Tiles = IncTiles;
    }

    public void SetUnits(Godot.Collections.Array<Unit> IncUnits)
    {
        Units = IncUnits;
    }
}
