using Godot;
using System;

public class Shop : Node
{
    private Player Player;
    private TileMap Tilemap;
    private GameManager GameManager;
    public Godot.Collections.Array<Unit> ShopUnits;
    private Godot.Collections.Array<Vector2> ShopTiles;
    public Godot.Collections.Array<UnitPack> UnitPacks;
    public bool InShopMode = false;
    public bool CanSelectUnitPack = false;
    private Unit SelectedUnit;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Player = GetNode<Player>("../Player");
        Tilemap = GetNode<TileMap>("../Map/TileMap");
        GameManager = GetNode<GameManager>("..");
        ShopUnits = new Godot.Collections.Array<Unit>();
        ShopTiles = GetShopTiles();
        UnitPacks = new Godot.Collections.Array<UnitPack>();
        SetUnitPacks();
        // EnterShopMode();
        // BuyUnit(Tilemap.GetUnitOnTile(new Vector2(1, 1)), new Vector2(5, 3));
    }

    public override void _Process(float delta)
    {
        // if (Input.IsActionJustPressed("left_click") && InShopMode)
        // {
        //     Vector2 Mousepos = Tilemap.GetGlobalMousePosition();
        //     Vector2 ClickedTile = Tilemap.WorldToMap(Mousepos);

        //     if (ShopTiles.Contains(ClickedTile))
        //     {
        //         SelectedUnit = GetUnitOnTile(ClickedTile);
        //         if (SelectedUnit != null)
        //         {
        //             GD.Print("Selected Unit: ", SelectedUnit.Name);
        //         }
        //     }
        //     else if (Tilemap.Tiles.Contains(ClickedTile) && SelectedUnit != null)
        //     {
        //         BuyUnit(SelectedUnit, ClickedTile);
        //         SelectedUnit = null;
        //     }
        // }
    }

    public void EnterShopMode()
    {
        GameManager.CameraController.SetTargetXPos(-1400);
        InShopMode = true;
        CanSelectUnitPack = true;
        Player.Money = 10;
        // ClearShop();
        // PopulateShop();
        FillShopPacks();
    }

    private void ExitShopMode()
    {
        InShopMode = false;
    }

    private void ClearShop()
    {
        while (ShopUnits.Count > 0)
        {
            ShopUnits[0].QueueFree();
            ShopUnits.RemoveAt(0);
        }
    }

    private void PopulateShop()
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
            GD.Print("Bought Unit: ", Unit.Name, " Remaining Money: ", Player.Money);
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

    private void SetUnitPacks()
    {
        // Unit Pack 1
        UnitPack UnitPack1 = GetNode<UnitPack>("./UnitPack1");
        UnitPack1.SetTiles(new Godot.Collections.Array<Vector2>(
            new Vector2(-1, 0),
            new Vector2(-1, 1),
            new Vector2(-2, 1)
        ));
        UnitPacks.Add(UnitPack1);

        // Unit Pack 2
        UnitPack UnitPack2 = GetNode<UnitPack>("./UnitPack2");
        UnitPack2.SetTiles(new Godot.Collections.Array<Vector2>(
            new Vector2(-2, 3),
            new Vector2(-3, 2),
            new Vector2(-3, 3)
        ));
        UnitPacks.Add(UnitPack2);

        // Unit Pack 3        
        UnitPack UnitPack3 = GetNode<UnitPack>("./UnitPack3");
        UnitPack3.SetTiles(new Godot.Collections.Array<Vector2>(
            new Vector2(-1, 4),
            new Vector2(-1, 5),
            new Vector2(-2, 5)
        ));
        UnitPacks.Add(UnitPack3);
    }

    private void FillShopPacks()
    {
        int NumberOfUnits = Tilemap.UnitScenes.Count;

        foreach (UnitPack UnitPack in UnitPacks)
        {
            // reset the unit array first
            UnitPack.Units = new Godot.Collections.Array<Unit>();
            foreach (Vector2 Tile in UnitPack.Tiles)
            {
                int Index = Math.Abs((int)GD.Randi() % NumberOfUnits);

                Tilemap.SpawnShopUnit(Tile, Index);

                Unit SpawnedUnit = Tilemap.ShopUnits[Tilemap.ShopUnits.Count - 1];

                ShopUnits.Add(SpawnedUnit);
                UnitPack.Units.Add(SpawnedUnit);
            }
        }
    }

    public void BuyUnitPack(UnitPack SelectedUnitPack)
    {
        foreach (UnitPack UnitPack in UnitPacks)
        {
            if (UnitPack == SelectedUnitPack)
            {
                foreach (Unit Unit in UnitPack.Units)
                {
                    Unit.PlayerOwned = true;
                    // Move Unit to Bench automatically?
                    // Unit.Move(TileToPlaceUnitOn);
                    Unit.Texture = (Texture)GD.Load("res://Hexagons/BlueHexagon.png");
                    Tilemap.Units.Add(Unit);
                    Tilemap.ShopUnits.Remove(Unit);
                }
            }
            else
            {
                // Removes the Units and makes them invisible
                foreach (Unit Unit in UnitPack.Units)
                {
                    ShopUnits.Remove(Unit);
                    Unit.Visible = false;
                }
            }
        }
        CanSelectUnitPack = false;
    }

    public void BuyUnitPackFromUnit(Unit SelectedUnit)
    {
        foreach (UnitPack UnitPack in UnitPacks)
        {
            foreach (Unit Unit in UnitPack.Units)
            {
                if (Unit == SelectedUnit)
                {
                    BuyUnitPack(UnitPack);
                    return;
                }
            }
        }
        GD.Print("Unit Not Found - In Shop.BuyUnitPackFromUnit");
    }
}
