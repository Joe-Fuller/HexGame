using Godot;
using System;

public class Shop : Node
{
    private Player Player;
    private TileMap Tilemap;
    private Godot.Collections.Array<Unit> ShopUnits;
    private Godot.Collections.Array<Vector2> ShopTiles;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Player = GetNode<Player>("../Player");
        Tilemap = GetNode<TileMap>("../Map/TileMap");
        ShopTiles = GetShopTiles();
        PopulateShop();
    }

    public void PopulateShop()
    {
        int NumberOfUnits = Tilemap.UnitScenes.Count;

        foreach (Vector2 ShopTile in ShopTiles)
        {
            int Index = Math.Abs((int)GD.Randi() % NumberOfUnits);

            // the true tag here means the units are PlayerOwned, idk if we want that
            Tilemap.SpawnUnit(ShopTile, true, Index);
        }
    }

    private Godot.Collections.Array<Vector2> GetShopTiles()
    {
        Godot.Collections.Array<Vector2> ShopTiles = new Godot.Collections.Array<Vector2>();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (Tilemap.GetCell(i, j) != -1)
                {
                    ShopTiles.Add(new Vector2(i, j));
                }
            }
        }

        return ShopTiles;
    }
}
