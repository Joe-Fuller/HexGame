using Godot;
using System;

public class GameManager : Node
{
    public TileMap Tilemap;
    public CombatManager CombatManager;
    public Player Player;
    public Shop Shop;
    public UnitInfoScreen UnitInfoScreen;
    public CameraController CameraController;
    public Unit SelectedUnit;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Tilemap = GetNode<TileMap>("./Map/TileMap");
        CombatManager = GetNode<CombatManager>("./CombatManager");
        Player = GetNode<Player>("./Player");
        Shop = GetNode<Shop>("./Shop");
        UnitInfoScreen = GetNode<UnitInfoScreen>("./UnitInfoScreen");
        CameraController = GetNode<CameraController>("./Camera2D");
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("left_click"))
        {
            Vector2 Mousepos = Tilemap.GetGlobalMousePosition();
            Vector2 ClickedTile = Tilemap.WorldToMap(Mousepos);

            if (GetUnitOnTile(ClickedTile) != null)
            {
                SelectedUnit = GetUnitOnTile(ClickedTile);
                UnitInfoScreen.SetText(SelectedUnit);
                GD.Print("Selected Unit: ", SelectedUnit.Name);
            }

            // SHOP
            if (Shop.InShopMode)
            {
                // BUY SELECTED UNIT
                if (SelectedUnit != null && SelectedUnit.CurrentCell.x < 3 && ClickedTile.x > 3 && ClickedTile.x < 7)
                {
                    Shop.BuyUnit(SelectedUnit, ClickedTile);
                    SelectedUnit = null;
                }

                // MOVE OWNED UNIT
                if (SelectedUnit != null && SelectedUnit.CurrentCell.x > 0 && ClickedTile.x > 0 && ClickedTile.x < 7 && SelectedUnit.PlayerOwned)
                {
                    Vector2 PrevTile = SelectedUnit.CurrentCell;
                    SelectedUnit.Move(ClickedTile);
                    if (SelectedUnit.CurrentCell != PrevTile)
                    {
                        SelectedUnit = null;
                    }
                }

            }

            // COMBAT
            else if (CombatManager.InCombat)
            {

            }

            // FIRST CLICK ON GAME TO ENTER SHOP MODE
            if (!CombatManager.InCombat && !Shop.InShopMode)
            {
                Shop.EnterShopMode();
            }
        }

        // right click to deselect
        if (Input.IsActionJustPressed("right_click"))
        {
            SelectedUnit = null;
            GD.Print("Selected Unit: None");
        }

        if (Input.IsActionJustPressed("left_ctrl"))
        {
            UnitInfoScreen.Visible = true;
        }

        if (Input.IsActionJustReleased("left_ctrl"))
        {
            UnitInfoScreen.Visible = false;
        }
    }

    public Unit GetUnitOnTile(Vector2 Tile)
    {
        // returns only VISIBLE units
        foreach (Unit Unit in Tilemap.Units + Shop.ShopUnits + CombatManager.Units)
        {
            if (Unit.Visible && Unit.CurrentCell == Tile)
            {
                return Unit;
            }
        }

        return null;
    }
}
