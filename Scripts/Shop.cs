using Godot;
using System;

public class Shop : Node
{
    private Player Player;
    private TileMap Tilemap;
    private Godot.Collections.Array<Unit> ShopUnits;
    private Godot.Collections.Array<Vector2> ShopTiles;
    public bool InShopMode = false;
    private Unit SelectedUnit;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Player = GetNode<Player>("../Player");
        Tilemap = GetNode<TileMap>("../Map/TileMap");
        ShopTiles = GetShopTiles();
        // EnterShopMode();
        // BuyUnit(Tilemap.GetUnitOnTile(new Vector2(1, 1)), new Vector2(5, 3));
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("left_click") && InShopMode)
        {
            Vector2 Mousepos = Tilemap.GetGlobalMousePosition();
            Vector2 ClickedTile = Tilemap.WorldToMap(Mousepos);

            if (ShopTiles.Contains(ClickedTile))
            {
                SelectedUnit = GetUnitOnTile(ClickedTile);
                if (SelectedUnit != null)
                {
                    GD.Print("Selected Unit: ", SelectedUnit.Name);
                }
            }
            else if (Tilemap.Tiles.Contains(ClickedTile) && SelectedUnit != null)
            {
                BuyUnit(SelectedUnit, ClickedTile);
                GD.Print("Bought Unit: ", SelectedUnit, " Remaining Money: ", Player.Money);
                SelectedUnit = null;
            }
        }
    }

    public void EnterShopMode()
    {
        InShopMode = true;
        Player.Money = 10;
        PopulateShop();
    }

    private void ExitShopMode()
    {
        InShopMode = false;
    }

    public void PopulateShop()
    {
        int NumberOfUnits = Tilemap.UnitScenes.Count;

        foreach (Vector2 ShopTile in ShopTiles)
        {
            int Index = Math.Abs((int)GD.Randi() % NumberOfUnits);

            Tilemap.SpawnShopUnit(ShopTile, Index);
        }

        ShopUnits = Tilemap.ShopUnits;
    }

    public void BuyUnit(Unit Unit, Vector2 TileToPlaceUnitOn)
    {
        if (Player.Money >= 3)
        {
            Player.Money -= 3;
            // TODO more about money
            Unit.PlayerOwned = true;
            Unit.Move(TileToPlaceUnitOn);
            Unit.Texture = (Texture)GD.Load("res://Hexagons/BlueHexagon.png");
            Tilemap.Units.Add(Unit);
            Tilemap.ShopUnits.Remove(Unit);
        }
        else
        {
            GD.Print("Can't Afford Unit");
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

    private Unit GetUnitOnTile(Vector2 Tile)
    {
        foreach (Unit Unit in ShopUnits)
        {
            if (Unit.CurrentCell == Tile)
            {
                return Unit;
            }
        }
        return null;
    }
}
