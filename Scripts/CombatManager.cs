using Godot;
using System;

public partial class CombatManager : Node2D
{
    [Signal] public delegate void TurnCompletedEventHandler();
    [Signal] public delegate void UnpausedEventHandler();

    public Godot.Collections.Array<Unit> Units;
    public TileMap Tilemap;
    public GameManager GameManager;
    public bool InCombat;
    public bool IsPaused = false;
    private Texture2D BlueHex;
    private Texture2D RedHex;
    private Godot.Collections.Array<Vector2I> EnemySpawnTiles;



    public Godot.Collections.Array<Unit> TurnOrder;
    public Godot.Collections.Array<TurnObject> TurnQueue;
    public int TurnOrderPos = 0;
    public float TurnTime = 0.5f;

    public RoundOverScreen RoundOverScreen;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Tilemap = GetNode<TileMap>("../Map/TileMap");
        GameManager = GetNode<GameManager>("..");
        BlueHex = (Texture2D)GD.Load("res://Hexagons/BlueHexagon.png");
        RedHex = (Texture2D)GD.Load("res://Hexagons/RedHexagon.png");
        Units = new Godot.Collections.Array<Unit>();
        TurnQueue = new Godot.Collections.Array<TurnObject>();
        TurnOrder = new Godot.Collections.Array<Unit>();
        RoundOverScreen = GetNode<RoundOverScreen>("../RoundOverScreen");
        EnemySpawnTiles = new Godot.Collections.Array<Vector2I>();
        SetEnemySpawnTiles();
    }

    public async void StartCombat()
    {
        GenerateEnemyTeam();
        SetCombatUnits(Tilemap.Units);
        InCombat = true;
        TurnOrderPos = 0;
        SetTurnOrder();
        int TurnsTaken = 0;
        await ToSignal(GameManager.CameraController, "ReachedLocation");
        while (InCombat)
        {
            if (IsPaused)
            {
                await ToSignal(this, "Unpaused");
            }
            if (IsWinner())
            {
                GD.Print("We Have A Winner! The " + (Units[0].PlayerOwned ? "Player" : "Computer") + " Won!");
                InCombat = false;
                EndCombat(Units[0].PlayerOwned);
                break;
            }
            // GD.Print("Taking Turn");
            if (TurnQueue.Count > 0)
            {
                TurnObject TurnObject = TurnQueue[0];
                // Commented Block useful for debugging Turn Order
                // GD.Print("Turn Order:");
                // foreach (Unit Unit in TurnOrder)
                // {
                //     if (Unit != null)
                //     {
                //         GD.Print(Unit.UnitName + (Unit == TurnObject.Unit ? "   <-------" : ""));
                //     }
                //     else { GD.Print("Dead Unit"); }
                // }
                // GD.Print("----------------");
                TurnQueue.RemoveAt(0);
                TakeTurn(TurnObject);
                await ToSignal(this, "TurnCompleted");
                TurnsTaken++;
            }
            else
            {
                Unit Unit = GetNextUnit();
                Unit.MovesThisTurn = 0;
                Unit.GetTarget(Units);
                TurnObject TurnObject = new TurnObject();
                if (Unit.CanAttack())
                {
                    if (Unit.DoesBuffs)
                    {
                        TurnObject = new TurnObject("Buff", Unit, Unit.GetAffectedTiles());
                    }
                    else
                    {
                        TurnObject = new TurnObject("Attack", Unit, Unit.GetAffectedTiles());
                    }
                }
                else
                {
                    Vector2I NextMove = TurnObject.Unit.GetNextMove();
                    Godot.Collections.Array<Vector2I> NextMoveArray = new Godot.Collections.Array<Vector2I>();
                    NextMoveArray.Add(NextMove);
                    TurnObject = new TurnObject("Move", Unit, NextMoveArray);
                }
                TurnQueue.Add(TurnObject);
                TurnOrderPos++;
                if (TurnOrderPos >= TurnOrder.Count)
                {
                    TurnOrderPos = 0;
                    // Call SetTurnOrder() here to shuffle the Unit turn order (higher damage units will still go first, but equal damage units may have their order shuffled)
                    // SetTurnOrder();
                    // it just made me confused, maybe it can come back with a turn order display
                }
            }

        }
        // once we exit the loop, end combat by e.g. removing units

    }

    public void SetCombatUnits(Godot.Collections.Array<Unit> IncUnits)
    {
        Units = new Godot.Collections.Array<Unit>();
        foreach (Unit IncUnit in IncUnits)
        {
            if (IncUnit.CurrentCell.X > 3)
            {
                Units.Add(Tilemap.CloneUnit(IncUnit));
                IncUnit.Visible = false;
            }
        }
        foreach (Unit Unit in Units)
        {
            Unit.CombatHealth = Unit.Health;
        }
        Tilemap.CombatUnits = Units;
    }

    public void SetTurnOrder()
    {
        TurnOrder = Units.Duplicate();
        TurnOrder.Shuffle();
        bool Swapped = true;

        // First sort to order by Damage
        while (Swapped)
        {
            Swapped = false;
            for (int i = 1; i < TurnOrder.Count; i++)
            {
                if (TurnOrder[i - 1].Damage < TurnOrder[i].Damage)
                {
                    Unit Temp = TurnOrder[i - 1];
                    TurnOrder[i - 1] = TurnOrder[i];
                    TurnOrder[i] = Temp;
                    Swapped = true;
                }
            }
        }

        // Second sort to put Haste Units first
        Swapped = true;
        while (Swapped)
        {
            Swapped = false;
            for (int i = 1; i < TurnOrder.Count; i++)
            {
                if (!TurnOrder[i - 1].Haste && TurnOrder[i].Haste)
                {
                    Unit Temp = TurnOrder[i - 1];
                    TurnOrder[i - 1] = TurnOrder[i];
                    TurnOrder[i] = Temp;
                    Swapped = true;
                }
            }
        }
    }

    private Unit GetNextUnit()
    {
        Unit NextUnit = TurnOrder[TurnOrderPos];
        while (NextUnit == null)
        {
            TurnOrderPos++;
            if (TurnOrderPos >= TurnOrder.Count)
            {
                TurnOrderPos = 0;
                // Call SetTurnOrder() here to shuffle the Unit turn order (higher damage units will still go first, but equal damage units may have their order shuffled)
                SetTurnOrder();
            }
            NextUnit = TurnOrder[TurnOrderPos];
        }
        return NextUnit;
    }

    private async void TakeTurn(TurnObject TurnObject)
    {
        if (IsPaused)
        {
            await ToSignal(this, "Unpaused");
        }
        Tilemap.ResetAllCellColoursExcept(TurnObject.Unit.CurrentCell);
        // TurnObject = {Type: string e.g.'Move', Unit: Unit, TargetTiles: Godot.Collections.Array<Vector2>}
        if (!TurnObject.Unit.CurrentCellIsHighlighted())
        {
            ColorCell(TurnObject.Unit.CurrentCell, TurnObject.Unit.PlayerOwned ? 3 : 4);
            // wait 1 second
            await ToSignal(GetTree().CreateTimer(TurnTime), "timeout");
        }
        if (TurnObject.Type == "Move")
        {
            ColorCell(TurnObject.TargetTiles[0], TurnObject.Unit.PlayerOwned ? 3 : 4);
            await ToSignal(GetTree().CreateTimer(TurnTime), "timeout");
            if (IsPaused)
            {
                await ToSignal(this, "Unpaused");
            }
            ColorCell(TurnObject.Unit.CurrentCell, 0);
            TurnObject.Unit.Move(TurnObject.TargetTiles[0]);
            TurnObject.Unit.MovesThisTurn++;
            await ToSignal(GetTree().CreateTimer(TurnTime), "timeout");
            if (IsPaused)
            {
                await ToSignal(this, "Unpaused");
            }
            // if unit can attack, queue an attack TurnObject
            if (TurnObject.Unit.CanAttack())
            {
                if (TurnObject.Unit.DoesBuffs)
                {
                    TurnObject NextTurnObject = new TurnObject("Buff", TurnObject.Unit, TurnObject.Unit.GetAffectedTiles());
                    TurnQueue.Add(NextTurnObject);
                }
                else
                {
                    TurnObject NextTurnObject = new TurnObject("Attack", TurnObject.Unit, TurnObject.Unit.GetAffectedTiles());
                    TurnQueue.Add(NextTurnObject);
                }
            }
            else if (TurnObject.Unit.CanMove())
            {
                // had to separately make the array and add the move after updating to godot 4
                Vector2I NextMove = TurnObject.Unit.GetNextMove();
                Godot.Collections.Array<Vector2I> NextMoveArray = new Godot.Collections.Array<Vector2I>();
                NextMoveArray.Add(NextMove);
                TurnObject NextTurnObject = new TurnObject("Move", TurnObject.Unit, NextMoveArray);
                TurnQueue.Add(NextTurnObject);
            }

        }
        if (TurnObject.Type == "Attack")
        {
            foreach (Vector2I Tile in TurnObject.TargetTiles)
            {
                ColorCell(Tile, TurnObject.Unit.PlayerOwned ? 3 : 4);
            }
            await ToSignal(GetTree().CreateTimer(TurnTime), "timeout");
            if (IsPaused)
            {
                await ToSignal(this, "Unpaused");
            }
            // damage units on target cells
            foreach (Vector2 Tile in TurnObject.TargetTiles)
            {
                for (int i = Units.Count - 1; i >= 0; i--)
                {
                    Unit Unit = Units[i];
                    if (Unit.CurrentCell == Tile)
                    {
                        TurnObject.Unit.DealDamage(Unit);
                        if (Unit.CombatHealth <= 0)
                        {
                            DestroyUnit(Unit);
                        }
                    }
                }
            }
            await ToSignal(GetTree().CreateTimer(TurnTime), "timeout");
            if (IsPaused)
            {
                await ToSignal(this, "Unpaused");
            }
            foreach (Vector2I Tile in TurnObject.TargetTiles)
            {
                ColorCell(Tile, 0);
            }
            await ToSignal(GetTree().CreateTimer(TurnTime), "timeout");
            if (IsPaused)
            {
                await ToSignal(this, "Unpaused");
            }
        }
        if (TurnObject.Type == "Buff")
        {
            foreach (Vector2I Tile in TurnObject.TargetTiles)
            {
                ColorCell(Tile, TurnObject.Unit.PlayerOwned ? 3 : 4);
            }
            await ToSignal(GetTree().CreateTimer(TurnTime), "timeout");
            if (IsPaused)
            {
                await ToSignal(this, "Unpaused");
            }
            // damage units on target cells
            foreach (Vector2 Tile in TurnObject.TargetTiles)
            {
                for (int i = Units.Count - 1; i >= 0; i--)
                {
                    Unit Unit = Units[i];
                    if (Unit.CurrentCell == Tile)
                    {
                        TurnObject.Unit.DoBuff(Unit);
                        Unit.UpdateText();
                    }
                }
            }
            await ToSignal(GetTree().CreateTimer(TurnTime), "timeout");
            if (IsPaused)
            {
                await ToSignal(this, "Unpaused");
            }
            foreach (Vector2I Tile in TurnObject.TargetTiles)
            {
                ColorCell(Tile, 0);
            }
            await ToSignal(GetTree().CreateTimer(TurnTime), "timeout");
            if (IsPaused)
            {
                await ToSignal(this, "Unpaused");
            }
        }
        // ColorCell(TurnObject.Unit.CurrentCell, 0);
        EmitSignal("TurnCompleted");
    }
    private void ColorCell(Vector2I Cell, int Colour)
    {
        if (Tilemap.GetCellSourceId(0, Cell) != -1)
        {
            Tilemap.SetCell(0, Cell, Colour);
        }
    }

    private bool IsWinner()
    {
        if (Units.Count <= 1)
        {
            return true;
        }
        for (int i = 1; i < Units.Count; i++)
        {
            if (Units[i].PlayerOwned != Units[i - 1].PlayerOwned)
            {
                return false;
            }
        }
        return true;
    }

    private void EndCombat(bool PlayerWon)
    {
        RoundOverScreen.Visible = true;
        RoundOverScreen.SetTitle(PlayerWon);

        // Destroy all the remaining Combat Units
        while (Units.Count > 0)
        {
            DestroyUnit(Units[0]);
        }

        // Make the Non-Combat Player Units Visible
        // MAKE SURE TO DESTROY THE OLD ENEMY UNITS AT SOME POINT
        foreach (Unit Unit in Tilemap.Units)
        {
            if (Unit.PlayerOwned)
            {
                Unit.Visible = true;
            }
        }

        Tilemap.ClearEnemyUnits();

        Tilemap.ResetAllCellColours();
    }

    public void Unpause()
    {
        EmitSignal("Unpaused");
        IsPaused = false;
    }

    private void GenerateEnemyTeam()
    {
        int PlayerUnitCount = 0;
        foreach (Unit Unit in Tilemap.Units)
        {
            if (Unit.PlayerOwned)
            {
                PlayerUnitCount++;
            }
        }
        // at this points Units should only contain Player Units
        for (int i = 0; i < PlayerUnitCount - 1; i++)
        {
            SpawnRandomEnemyUnit();
        }
    }

    private void SpawnRandomEnemyUnit()
    {
        EnemySpawnTiles.Shuffle();
        int TileIndex = 0;
        Vector2I Tile = EnemySpawnTiles[0];
        while (GameManager.GetUnitOnTile(Tile) != null)
        {
            TileIndex++;
            if (TileIndex == EnemySpawnTiles.Count)
            {
                GD.Print("All Tiles Occupied");
            }
            Tile = EnemySpawnTiles[TileIndex];
        }

        //  POSSIBLE FIX NEEDED
        int Index = (int)GD.Randi() % (Tilemap.UnitScenes.Count - 1);
        Tilemap.SpawnUnit(Tile, false, Index);
    }

    private void SetEnemySpawnTiles()
    {
        for (int i = 10; i < 15; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (Tilemap.GetCellSourceId(0, new Vector2I(i, j)) != -1)
                {
                    EnemySpawnTiles.Add(new Vector2I(i, j));
                }
            }
        }
    }

    private void DestroyUnit(Unit Unit)
    {
        Units.Remove(Unit);
        Unit.QueueFree();
    }
}
