using Godot;
using System;

public partial class TileMap : Godot.TileMap
{
    public Godot.Collections.Array<PackedScene> UnitScenes;
    public Godot.Collections.Array<Unit> Units;
    public Godot.Collections.Array<Unit> CombatUnits;
    public Godot.Collections.Array<Unit> ShopUnits;
    public Godot.Collections.Array<Vector2I> Tiles;

    public Vector2I StartPos = new Vector2I(-100, -100);
    public Vector2I EndPos;

    public Vector2I SelectedCell = new Vector2I(-100, -100);

    public void SpawnUnit(Vector2I Location, bool PlayerOwned, int IndeX)
    {
        // this was Instance() it might POSSIBLE FIX NEEDED
        Unit NewUnit = (Unit)UnitScenes[IndeX].Instantiate();

        NewUnit.PlayerOwned = PlayerOwned;
        if (!PlayerOwned)
        {
            NewUnit.Texture = (Texture2D)GD.Load("res://Hexagons/RedHexagon.png");
        }
        NewUnit.CurrentCell = Location;
        AddChild(NewUnit);
        NewUnit.Initialise(Location);
        Units.Add(NewUnit);
    }

    public void SpawnShopUnit(Vector2I Location, int IndeX)
    {
        Unit NewUnit = (Unit)UnitScenes[IndeX].Instantiate();

        NewUnit.PlayerOwned = false;
        // shop units are currentlY coloured greY, make sure to change this
        NewUnit.Texture = (Texture2D)GD.Load("res://Hexagons/GreyHexagon.png");
        NewUnit.CurrentCell = Location;
        AddChild(NewUnit);
        NewUnit.Initialise(Location);
        ShopUnits.Add(NewUnit);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Randomize();
        Units = new Godot.Collections.Array<Unit>();
        CombatUnits = new Godot.Collections.Array<Unit>();
        ShopUnits = new Godot.Collections.Array<Unit>();
        Tiles = new Godot.Collections.Array<Vector2I>();
        SetTiles();

        CollectUnitScenes();
    }

    public void FindCell(Vector2I Mousepos)
    {
        var Cell = LocalToMap(Mousepos);
        var CellTYpe = GetCellSourceId(0, Cell);
        bool isValid = CellTYpe != -1;
        if (isValid)
        {
            if (StartPos.X == -100)
            {
                StartPos = Cell;
                SetCell(0, StartPos, 1);
            }
            else
            {
                SetCell(0, EndPos, 0);
                EndPos = Cell;
                SetCell(0, EndPos, 1);
            }
        }
        GD.Print(StartPos, EndPos);
    }

    public Godot.Collections.Array<Vector2I> GetNeighbours(Vector2I Cell, bool GetAllNeighbours = false)
    {
        var Neighbours = new Godot.Collections.Array<Vector2I>();

        int X = (int)Cell.X;
        int Y = (int)Cell.Y;

        if (GetCellSourceId(0, new Vector2I(X, Y - 1)) != -1)
        {
            Neighbours.Add(new Vector2I(X, Y - 1));
        }
        if (GetCellSourceId(0, new Vector2I(X, Y + 1)) != -1)
        {
            Neighbours.Add(new Vector2I(X, Y + 1));
        }
        if (X % 2 == 0)
        {
            if (GetCellSourceId(0, new Vector2I(X + 1, Y - 1)) != -1)
            {
                Neighbours.Add(new Vector2I(X + 1, Y - 1));
            }
            if (GetCellSourceId(0, new Vector2I(X + 1, Y)) != -1)
            {
                Neighbours.Add(new Vector2I(X + 1, Y));
            }
            if (GetCellSourceId(0, new Vector2I(X - 1, Y - 1)) != -1)
            {
                Neighbours.Add(new Vector2I(X - 1, Y - 1));
            }
            if (GetCellSourceId(0, new Vector2I(X - 1, Y)) != -1)
            {
                Neighbours.Add(new Vector2I(X - 1, Y));
            }
        }
        else
        {
            if (GetCellSourceId(0, new Vector2I(X + 1, Y)) != -1)
            {
                Neighbours.Add(new Vector2I(X + 1, Y));
            }
            if (GetCellSourceId(0, new Vector2I(X + 1, Y + 1)) != -1)
            {
                Neighbours.Add(new Vector2I(X + 1, Y + 1));
            }
            if (GetCellSourceId(0, new Vector2I(X - 1, Y)) != -1)
            {
                Neighbours.Add(new Vector2I(X - 1, Y));
            }
            if (GetCellSourceId(0, new Vector2I(X - 1, Y + 1)) != -1)
            {
                Neighbours.Add(new Vector2I(X - 1, Y + 1));
            }
        }

        // GD.Print(Neighbours.Count);

        if (!GetAllNeighbours)
        {
            // Remove the neighbours containing a unit as theY are impassible
            for (int i = Neighbours.Count - 1; i >= 0; i--)
            {
                Vector2I Neighbour = Neighbours[i];
                foreach (Unit Unit in CombatUnits)
                {
                    if (Unit.CurrentCell == Neighbour)
                    {
                        Neighbours.RemoveAt(i);
                    }
                }
            }
        }

        // GD.Print(Neighbours.Count);

        Neighbours.Shuffle();

        return Neighbours;
    }

    public int Distance(Vector2I Cell1, Vector2I Cell2)
    {
        var AXialCell1 = new Vector2I(Cell1.X, Cell1.Y - (Cell1.X - ((int)Cell1.X & 1)) / 2);
        var AXialCell2 = new Vector2I(Cell2.X, Cell2.Y - (Cell2.X - ((int)Cell2.X & 1)) / 2);

        var Dist = (Mathf.Abs(AXialCell1.X - AXialCell2.X) + Mathf.Abs(AXialCell1.X + AXialCell1.Y - AXialCell2.X - AXialCell2.Y) + Mathf.Abs(AXialCell1.Y - AXialCell2.Y)) / 2;

        return (int)Dist;
    }

    public Godot.Collections.Array<Vector2I> ReconstructPath(Godot.Collections.Dictionary<Vector2I, Vector2I> CameFrom, Vector2I Current)
    {
        var TotalPath = new Godot.Collections.Array<Vector2I>();
        TotalPath.Add(Current);
        while (CameFrom.Keys.Contains(Current))
        {
            Current = CameFrom[Current];
            TotalPath.Add(Current);
        }
        return TotalPath;
    }

    public Godot.Collections.Array<Vector2I> AStar3D(Vector2I Start, Vector2I End)
    {
        var OpenSet = new Godot.Collections.Array<Vector2I>();
        OpenSet.Add(Start);

        var CameFrom = new Godot.Collections.Dictionary<Vector2I, Vector2I>();

        var GScore = new Godot.Collections.Dictionary<Vector2I, int>();
        GScore[Start] = 0;

        var FScore = new Godot.Collections.Dictionary<Vector2I, int>();
        FScore[Start] = Distance(Start, End);

        while (OpenSet.Count > 0)
        {
            Vector2I Current = OpenSet[0];

            // DUE TO CHANGING THE BELOW LINE ASTAR NOW FINDS ANY TILE AT DISTANCE ONE FROM THE TARGET
            if (Distance(Current, End) == 1)
            {
                return ReconstructPath(CameFrom, Current);
            }

            OpenSet.Remove(Current);

            foreach (Vector2I Neighbour in GetNeighbours(Current))
            {
                int TentativeGScore = 9999;
                if (GScore.ContainsKey(Current))
                {
                    TentativeGScore = GScore[Current] + Distance(Current, Neighbour);
                }
                int temp = 9999;
                if (GScore.ContainsKey(Neighbour))
                {
                    temp = GScore[Neighbour];
                }
                if (TentativeGScore < temp)
                {
                    CameFrom[Neighbour] = Current;
                    GScore[Neighbour] = TentativeGScore;
                    FScore[Neighbour] = TentativeGScore + Distance(Neighbour, End);
                    if (!OpenSet.Contains(Neighbour))
                    {
                        OpenSet.Add(Neighbour);
                    }
                }
            }
        }

        GD.Print("Returning null");
        return null;
    }

    public void ResetAllCellColours()
    {
        for (int i = -10; i < 20; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (GetCellSourceId(0, new Vector2I(i, j)) != -1)
                {
                    SetCell(0, new Vector2I(i, j), 0);
                }
            }
        }
    }

    public void ResetAllCellColoursExcept(Vector2I Cell)
    {
        for (int i = 0; i < 30; i++)
        {
            for (int j = 0; j < 30; j++)
            {
                if (!(i == Cell.X && j == Cell.Y) && GetCellSourceId(0, new Vector2I(i, j)) != -1)
                {
                    SetCell(0, new Vector2I(i, j), 0);
                }
            }
        }
    }

    public Unit GetUnitOnTile(Vector2I Tile)
    {
        foreach (Unit Unit in Units)
        {
            if (Unit.CurrentCell == Tile)
            {
                return Unit;
            }
        }
        foreach (Unit Unit in ShopUnits)
        {
            if (Unit.CurrentCell == Tile)
            {
                return Unit;
            }
        }

        return null;
    }

    private void SetTiles()
    {

        for (int i = 3; i < 15; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (GetCellSourceId(0, new Vector2I(i, j)) != -1)
                {
                    Tiles.Add(new Vector2I(i, j));
                }
            }
        }

    }

    public Unit CloneUnit(Unit Unit)
    {
        int UnitIndex = 0;
        // Weird magic function to search the UnitScenes for the right indeX
        for (int i = 0; i < UnitScenes.Count; i++)
        {
            PackedScene UnitScene = UnitScenes[i];
            Godot.Collections.Dictionary WeirdUnitSceneDictionary = UnitScene._Bundled;
            String[] StringArray = ((String[])WeirdUnitSceneDictionary["names"]);
            string UnitSceneUnitName = StringArray[0];
            if (UnitSceneUnitName == Unit.UnitName)
            {
                UnitIndex = i;
            }
        }
        Unit ClonedUnit = (Unit)UnitScenes[UnitIndex].Instantiate();
        ClonedUnit.PlayerOwned = Unit.PlayerOwned;
        if (!ClonedUnit.PlayerOwned)
        {
            ClonedUnit.Texture = (Texture2D)GD.Load("res://Hexagons/RedHexagon.png");
        }
        ClonedUnit.Health = Unit.Health;
        ClonedUnit.Damage = Unit.Damage;
        ClonedUnit.CurrentCell = Unit.CurrentCell;
        AddChild(ClonedUnit);
        ClonedUnit.Initialise(ClonedUnit.CurrentCell);

        return ClonedUnit;
    }

    public void ClearEnemyUnits()
    {
        for (int i = Units.Count - 1; i > 0; i--)
        {
            if (!Units[i].PlayerOwned)
            {
                Units.RemoveAt(i);
            }
        }
    }

    public void CollectUnitScenes()
    {
        UnitScenes = new Godot.Collections.Array<PackedScene>();
        using var Dir = DirAccess.Open("res://Scenes/Units");
        // Dir.Open("res://Scenes/Units");
        Dir.ListDirBegin();

        while (true)
        {
            var Scene = Dir.GetNext();
            if (Scene == "")
            {
                break;
            }
            if (!Scene.StartsWith(".") && Scene != "Unit.tscn")
            {
                PackedScene UnitScene = ResourceLoader.Load<PackedScene>("res://Scenes/Units/" + Scene);
                UnitScenes.Add(UnitScene);
            }
        }

        Dir.ListDirEnd();


    }
}

