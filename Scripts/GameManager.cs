using Godot;
using System;

public class GameManager : Node
{
    public TileMap Tilemap;
    public CombatManager CombatManager;
    public Player Player;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Tilemap = GetNode<TileMap>("./Map/TileMap");
        CombatManager = GetNode<CombatManager>("./CombatManager");
        Player = GetNode<Player>("./Player");
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("left_click") && !CombatManager.InCombat)
        {
            // Tilemap.SpawnUnit(new Vector2(1, 1), true, 8);
            // Tilemap.SpawnUnit(new Vector2(1, 3), true, 4);
            // Tilemap.SpawnUnit(new Vector2(1, 4), true, 2);
            Tilemap.SpawnUnit(new Vector2(1, 3), true, 0);
            Tilemap.SpawnUnit(new Vector2(1, 4), true, 1);
            Tilemap.SpawnUnit(new Vector2(1, 5), true, 2);
            Tilemap.SpawnUnit(new Vector2(5, 3), false, 3);
            CombatManager.SetUnits(Tilemap.Units);
            CombatManager.StartCombat();
        }
    }
}
