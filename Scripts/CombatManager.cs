using Godot;
using System;

public class CombatManager : Node2D
{
    [Signal] private delegate void TurnCompleted();
    [Signal] private delegate void Unpaused();

    public Godot.Collections.Array<Unit> Units;
    public TileMap Tilemap;
    public bool InCombat;
    public bool IsPaused = false;
    public Timer TurnTimer;
    private Texture BlueHex;
    private Texture RedHex;



    public Godot.Collections.Array<Unit> TurnOrder;
    public Godot.Collections.Array<TurnObject> TurnQueue;
    public int TurnOrderPos = 0;
    public float TurnTime = 0.5f;

    public RoundOverScreen RoundOverScreen;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Tilemap = GetNode<TileMap>("../Map/TileMap");
        TurnTimer = GetNode<Timer>("./Turn Timer");
        BlueHex = (Texture)GD.Load("res://Hexagons/BlueHexagon.png");
        RedHex = (Texture)GD.Load("res://Hexagons/RedHexagon.png");
        TurnQueue = new Godot.Collections.Array<TurnObject>();
        TurnOrder = new Godot.Collections.Array<Unit>();
        RoundOverScreen = GetNode<RoundOverScreen>("../RoundOverScreen");
    }

    public async void StartCombat()
    {
        GenerateEnemyTeam();
        SetCombatUnits(Tilemap.Units);
        InCombat = true;
        TurnOrderPos = 0;
        SetTurnOrder();
        int TurnsTaken = 0;
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
                    TurnObject = new TurnObject("Attack", Unit, Unit.GetDamagedTiles());
                }
                else
                {
                    TurnObject = new TurnObject("Move", Unit, new Godot.Collections.Array<Vector2>(Unit.GetNextMove()));
                }
                TurnQueue.Add(TurnObject);
                TurnOrderPos++;
                if (TurnOrderPos >= TurnOrder.Count)
                {
                    TurnOrderPos = 0;
                    // Call SetTurnOrder() here to shuffle the Unit turn order (higher damage units will still go first, but equal damage units may have their order shuffled)
                    SetTurnOrder();
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
            Units.Add(Tilemap.CloneUnit(IncUnit));
            IncUnit.Visible = false;
        }
        Tilemap.CombatUnits = Units;
    }

    public void SetTurnOrder()
    {
        TurnOrder = Units.Duplicate();
        TurnOrder.Shuffle();
        bool Swapped = true;
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
                TurnObject NextTurnObject = new TurnObject("Attack", TurnObject.Unit, TurnObject.Unit.GetDamagedTiles());
                TurnQueue.Add(NextTurnObject);
            }
            else if (TurnObject.Unit.CanMove())
            {
                TurnObject NextTurnObject = new TurnObject("Move", TurnObject.Unit, new Godot.Collections.Array<Vector2>(TurnObject.Unit.GetNextMove()));
                TurnQueue.Add(NextTurnObject);
            }

        }
        if (TurnObject.Type == "Attack")
        {
            foreach (Vector2 Tile in TurnObject.TargetTiles)
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
                        Unit.Health -= TurnObject.Unit.Damage;
                        Unit.UpdateText();
                        if (Unit.Health <= 0)
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
            foreach (Vector2 Tile in TurnObject.TargetTiles)
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

    // private void TakeNextTurn()
    // {
    //     Unit CurrUnit = Units[0];
    //     Vector2 CellUnitStartedTurnOn = CurrUnit.CurrentCell;
    //     ColorCell(CellUnitStartedTurnOn, CurrUnit.PlayerOwned ? 1 : 2);
    //     Tilemap.SetCell((int)CellUnitStartedTurnOn.x, (int)CellUnitStartedTurnOn.y, CurrUnit.PlayerOwned ? 2 : 1);

    //     CurrUnit.GetTarget();

    //     //move toward target, then attack if in range
    //     if (Tilemap.Distance(CurrUnit.CurrentCell, CurrUnit.TargetCell) > 1)
    //     {
    //         CurrUnit.MoveTowardTarget();
    //     }

    //     if (Tilemap.Distance(CurrUnit.CurrentCell, CurrUnit.TargetCell) == 1)
    //     {
    //         GD.Print("Attacking");
    //         CurrUnit.Attack();
    //     }

    //     ColorCell(CellUnitStartedTurnOn, 0);
    //     Units.RemoveAt(0);
    //     Units.Add(CurrUnit);
    //     Tilemap.ResetAllCellColours();
    // }

    private void ColorCell(Vector2 Cell, int Colour)
    {
        Tilemap.SetCell((int)Cell.x, (int)Cell.y, Colour);
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
        Tilemap.SpawnUnit(new Vector2(10, 1), false, 0);
        Tilemap.SpawnUnit(new Vector2(10, 2), false, 1);
        Tilemap.SpawnUnit(new Vector2(10, 3), false, 2);
    }

    private void DestroyUnit(Unit Unit)
    {
        Units.Remove(Unit);
        Unit.QueueFree();
    }
}
