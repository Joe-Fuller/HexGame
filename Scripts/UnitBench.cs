using Godot;
using System;

public class UnitBench : Node
{
    public TileMap Tilemap;
    public Godot.Collections.Array<Vector2> UnitBenchTiles;

    public override void _Ready()
    {
        base._Ready();
        Tilemap = GetNode<TileMap>("../Map/TileMap");
        UnitBenchTiles = GetUnitBenchTiles();
    }

    private Godot.Collections.Array<Vector2> GetUnitBenchTiles()
    {
        Godot.Collections.Array<Vector2> UnitBenchTiles = new Godot.Collections.Array<Vector2>();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (Tilemap.GetCell(i, j) != -1)
                {
                    UnitBenchTiles.Add(new Vector2(i, j));
                }
            }
        }

        return UnitBenchTiles;
    }
}
