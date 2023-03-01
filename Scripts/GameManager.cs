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
            Vector2 ClickedTile = Tilemap.WorldToMap(Mousepos * 2);

            if (GetUnitOnTile(ClickedTile) != null)
            {
                SelectUnit(GetUnitOnTile(ClickedTile));
            }

            // SHOP
            if (Shop.InShopMode)
            {
                // // BUY SELECTED UNIT (replaced by SELECT UNIT PACK)
                // if (SelectedUnit != null && SelectedUnit.CurrentCell.x < 3 && ClickedTile.x > 3 && ClickedTile.x < 7)
                // {
                //     Shop.BuyUnit(SelectedUnit, ClickedTile);
                //     SelectedUnit = null;
                // }

                // SELECT UNIT PACK
                if (SelectedUnit != null && SelectedUnit.CurrentCell.x < 1 && Shop.CanSelectUnitPack)
                {
                    Shop.BuyUnitPackFromUnit(SelectedUnit);
                    DeselectUnit();
                }

                // MOVE OWNED UNIT
                if (SelectedUnit != null && ClickedTile.x > 0 && ClickedTile.x < 7 && SelectedUnit.PlayerOwned)
                {
                    Vector2 PrevTile = SelectedUnit.CurrentCell;
                    SelectedUnit.Move(ClickedTile);
                    if (SelectedUnit.CurrentCell != PrevTile)
                    {
                        Tilemap.SetCellv(PrevTile, 0);
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
            DeselectUnit();
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

    private void SelectUnit(Unit Unit)
    {
        // Decolours current selected tile, sets SelectedUnit, colours new selected tile
        if (SelectedUnit != null)
        {
            Tilemap.SetCellv(SelectedUnit.CurrentCell, 0);
        }
        SelectedUnit = Unit;
        // UnitInfoScreen.SetText(SelectedUnit); idk why this line was here
        Tilemap.SetCellv(Unit.CurrentCell, 2);
    }

    private void DeselectUnit()
    {
        if (SelectedUnit != null)
        {
            Tilemap.SetCellv(SelectedUnit.CurrentCell, 0);
        }
        SelectedUnit = null;
    }

}
