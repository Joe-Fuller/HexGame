using Godot;
using System;

public partial class UnitBench : Node
{
    public TileMap Tilemap;
    public Godot.Collections.Array<Vector2I> UnitBenchTiles;

    public override void _Ready()
    {
        base._Ready();
        Tilemap = GetNode<TileMap>("../Map/TileMap");
        UnitBenchTiles = GetUnitBenchTiles();
    }

    private Godot.Collections.Array<Vector2I> GetUnitBenchTiles()
    {
        Godot.Collections.Array<Vector2I> UnitBenchTiles = new Godot.Collections.Array<Vector2I>();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (Tilemap.GetCellSourceId(0, new Vector2I(i, j)) != -1)
                {
                    UnitBenchTiles.Add(new Vector2I(i, j));
                }
            }
        }

        return UnitBenchTiles;
    }
}
