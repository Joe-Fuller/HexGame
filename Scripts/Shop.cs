using Godot;
using System;

public partial class Shop : Node
{
    private Player Player;
    private TileMap Tilemap;
    private GameManager GameManager;
    public Godot.Collections.Array<Unit> ShopUnits;
    private Godot.Collections.Array<Vector2I> ShopTiles;
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

    public override void _Process(double delta)
    {
        // if (Input.IsActionJustPressed("left_click") && InShopMode)
        // {
        //     Vector2 Mousepos = Tilemap.GetGlobalMousePosition();
        //     Vector2 ClickedTile = Tilemap.LocalToMap(Mousepos);

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
        GameManager.CameraController.SetTargetXPos(-1000);
        InShopMode = true;
        CanSelectUnitPack = true;
        Player.Money = 10;
        ClearShop();
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

        foreach (Vector2I ShopTile in ShopTiles)
        {
            int Index = Math.Abs((int)GD.Randi() % NumberOfUnits);

            Tilemap.SpawnShopUnit(ShopTile, Index);
        }

        ShopUnits = Tilemap.ShopUnits;
    }

    public void BuyUnit(Unit Unit, Vector2I TileToPlaceUnitOn)
    {
        if (Player.Money >= 3)
        {
            Player.Money -= 3;
            // TODO more about money
            Unit.PlayerOwned = true;
            Unit.Move(TileToPlaceUnitOn);
            Unit.Texture = (Texture2D)GD.Load("res://Hexagons/BlueHexagon.png");
            Tilemap.Units.Add(Unit);
            Tilemap.ShopUnits.Remove(Unit);
            GD.Print("Bought Unit: ", Unit.Name, " Remaining Money: ", Player.Money);
        }
        else
        {
            GD.Print("Can't Afford Unit");
        }
    }


    private Godot.Collections.Array<Vector2I> GetShopTiles()
    {
        Godot.Collections.Array<Vector2I> ShopTiles = new Godot.Collections.Array<Vector2I>();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (Tilemap.GetCellSourceId(0, new Vector2I(i, j)) != -1)
                {
                    ShopTiles.Add(new Vector2I(i, j));
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
        Godot.Collections.Array<Vector2I> Pack1Tiles = new Godot.Collections.Array<Vector2I>();
        Pack1Tiles.Add(new Vector2I(-1, 0));
        Pack1Tiles.Add(new Vector2I(-1, 1));
        Pack1Tiles.Add(new Vector2I(-2, 1));
        UnitPack1.SetTiles(Pack1Tiles);
        UnitPacks.Add(UnitPack1);

        // Unit Pack 2
        UnitPack UnitPack2 = GetNode<UnitPack>("./UnitPack2");
        Godot.Collections.Array<Vector2I> Pack2Tiles = new Godot.Collections.Array<Vector2I>();
        Pack2Tiles.Add(new Vector2I(-2, 3));
        Pack2Tiles.Add(new Vector2I(-3, 2));
        Pack2Tiles.Add(new Vector2I(-3, 3));
        UnitPack2.SetTiles(Pack2Tiles);
        UnitPacks.Add(UnitPack2);

        // Unit Pack 3        
        UnitPack UnitPack3 = GetNode<UnitPack>("./UnitPack3");
        Godot.Collections.Array<Vector2I> Pack3Tiles = new Godot.Collections.Array<Vector2I>();
        Pack3Tiles.Add(new Vector2I(-1, 4));
        Pack3Tiles.Add(new Vector2I(-1, 5));
        Pack3Tiles.Add(new Vector2I(-2, 5));
        UnitPack3.SetTiles(Pack3Tiles);
        UnitPacks.Add(UnitPack3);
    }

    private void FillShopPacks()
    {
        int NumberOfUnits = Tilemap.UnitScenes.Count;

        foreach (UnitPack UnitPack in UnitPacks)
        {
            // reset the unit array first
            UnitPack.Units = new Godot.Collections.Array<Unit>();
            foreach (Vector2I Tile in UnitPack.Tiles)
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
                    Unit.Texture = (Texture2D)GD.Load("res://Hexagons/BlueHexagon.png");
                    Tilemap.Units.Add(Unit);
                    Tilemap.ShopUnits.Remove(Unit);
                    ShopUnits.Remove(Unit);
                }
            }
            else
            {
                // Removes the Units and makes them invisible
                foreach (Unit Unit in UnitPack.Units)
                {
                    Tilemap.ShopUnits.Remove(Unit);
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
