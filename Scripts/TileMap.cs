using Godot;
using System;

public class TileMap : Godot.TileMap
{
    [Export]
    public PackedScene UnitScene;

    [Export]
    public Godot.Collections.Array<PackedScene> UnitScenes;
    public Godot.Collections.Array<Unit> Units;
    public Godot.Collections.Array<Unit> ShopUnits;
    public Godot.Collections.Array<Vector2> Tiles;

    public Vector2 StartPos = new Vector2(-100, -100);
    public Vector2 EndPos;

    public Vector2 SelectedCell = new Vector2(-100, -100);

    public void SpawnUnit(Vector2 Location, bool PlayerOwned, int Index)
    {
        Unit NewUnit = (Unit)UnitScenes[Index].Instance();

        NewUnit.PlayerOwned = PlayerOwned;
        if (!PlayerOwned)
        {
            NewUnit.Texture = (Texture)GD.Load("res://Hexagons/RedHexagon.png");
        }
        NewUnit.CurrentCell = Location;
        AddChild(NewUnit);
        NewUnit.Initialise(Location);
        Units.Add(NewUnit);
    }

    public void SpawnShopUnit(Vector2 Location, int Index)
    {
        Unit NewUnit = (Unit)UnitScenes[Index].Instance();

        NewUnit.PlayerOwned = false;
        // shop units are currently coloured grey, make sure to change this
        NewUnit.Texture = (Texture)GD.Load("res://Hexagons/GreyHexagon.png");
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
        ShopUnits = new Godot.Collections.Array<Unit>();
        Tiles = new Godot.Collections.Array<Vector2>();
        SetTiles();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        //   if (Input.IsActionJustPressed("left_click"))
        //   {
        // 	// Reset the selected cell and its neighbours colours
        // 	SetCell((int)SelectedCell.x, (int)SelectedCell.y, 0);
        // 	foreach (var Cell in GetNeighbours(SelectedCell))
        // 	{
        // 		SetCell((int)Cell.x, (int)Cell.y, 0);
        // 	}

        // 	var Mousepos = GetGlobalMousePosition();
        // 	var ClickedCell = WorldToMap(Mousepos);

        // 	GD.Print(Distance(SelectedCell, ClickedCell));

        // 	if (GetCell((int)ClickedCell.x, (int)ClickedCell.y) != -1)
        // 	{
        // 		if (SelectedCell.x != -100)
        // 		{
        // 			foreach (Vector2 Cell in AStar(SelectedCell, ClickedCell))
        // 			{
        // 				SetCell((int)Cell.x, (int)Cell.y, 2);
        // 			}
        // 		}
        // 		SelectedCell = ClickedCell;
        // 		SetCell((int)SelectedCell.x, (int)SelectedCell.y, 2);
        // 		var Neighbours = GetNeighbours(ClickedCell);
        // 		foreach (var Cell in Neighbours)
        // 		{
        // 			SetCell((int)Cell.x, (int)Cell.y, 1);
        // 		}
        // 	}


        //   } 
    }

    public void FindCell(Vector2 Mousepos)
    {
        var Cell = WorldToMap(Mousepos);
        var CellType = GetCell((int)Cell.x, (int)Cell.y);
        bool isValid = CellType != -1;
        if (isValid)
        {
            if (StartPos.x == -100)
            {
                StartPos = Cell;
                SetCell((int)StartPos.x, (int)StartPos.y, 1);
            }
            else
            {
                SetCell((int)EndPos.x, (int)EndPos.y, 0);
                EndPos = Cell;
                SetCell((int)EndPos.x, (int)EndPos.y, 1);
            }
        }
        GD.Print(StartPos, EndPos);
    }

    public Godot.Collections.Array<Vector2> GetNeighbours(Vector2 Cell, bool GetAllNeighbours = false)
    {
        var Neighbours = new Godot.Collections.Array<Vector2>();

        int x = (int)Cell.x;
        int y = (int)Cell.y;

        if (GetCell(x, y - 1) != -1)
        {
            Neighbours.Add(new Vector2(x, y - 1));
        }
        if (GetCell(x, y + 1) != -1)
        {
            Neighbours.Add(new Vector2(x, y + 1));
        }
        if (x % 2 == 0)
        {
            if (GetCell(x + 1, y - 1) != -1)
            {
                Neighbours.Add(new Vector2(x + 1, y - 1));
            }
            if (GetCell(x + 1, y) != -1)
            {
                Neighbours.Add(new Vector2(x + 1, y));
            }
            if (GetCell(x - 1, y - 1) != -1)
            {
                Neighbours.Add(new Vector2(x - 1, y - 1));
            }
            if (GetCell(x - 1, y) != -1)
            {
                Neighbours.Add(new Vector2(x - 1, y));
            }
        }
        else
        {
            if (GetCell(x + 1, y) != -1)
            {
                Neighbours.Add(new Vector2(x + 1, y));
            }
            if (GetCell(x + 1, y + 1) != -1)
            {
                Neighbours.Add(new Vector2(x + 1, y + 1));
            }
            if (GetCell(x - 1, y) != -1)
            {
                Neighbours.Add(new Vector2(x - 1, y));
            }
            if (GetCell(x - 1, y + 1) != -1)
            {
                Neighbours.Add(new Vector2(x - 1, y + 1));
            }
        }

        // GD.Print(Neighbours.Count);

        if (!GetAllNeighbours)
        {
            // Remove the neighbours containing a unit as they are impassible
            for (int i = Neighbours.Count - 1; i >= 0; i--)
            {
                Vector2 Neighbour = Neighbours[i];
                foreach (Unit Unit in Units)
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

    public int Distance(Vector2 Cell1, Vector2 Cell2)
    {
        var AxialCell1 = new Vector2(Cell1.x, Cell1.y - (Cell1.x - ((int)Cell1.x & 1)) / 2);
        var AxialCell2 = new Vector2(Cell2.x, Cell2.y - (Cell2.x - ((int)Cell2.x & 1)) / 2);

        var Dist = (Mathf.Abs(AxialCell1.x - AxialCell2.x) + Mathf.Abs(AxialCell1.x + AxialCell1.y - AxialCell2.x - AxialCell2.y) + Mathf.Abs(AxialCell1.y - AxialCell2.y)) / 2;

        return (int)Dist;
    }

    public Godot.Collections.Array<Vector2> ReconstructPath(Godot.Collections.Dictionary<Vector2, Vector2> CameFrom, Vector2 Current)
    {
        var TotalPath = new Godot.Collections.Array<Vector2>();
        TotalPath.Add(Current);
        while (CameFrom.Keys.Contains(Current))
        {
            Current = CameFrom[Current];
            TotalPath.Add(Current);
        }
        return TotalPath;
    }

    public Godot.Collections.Array<Vector2> AStar(Vector2 Start, Vector2 End)
    {
        var OpenSet = new Godot.Collections.Array<Vector2>();
        OpenSet.Add(Start);

        var CameFrom = new Godot.Collections.Dictionary<Vector2, Vector2>();

        var GScore = new Godot.Collections.Dictionary<Vector2, int>();
        GScore[Start] = 0;

        var FScore = new Godot.Collections.Dictionary<Vector2, int>();
        FScore[Start] = Distance(Start, End);

        while (OpenSet.Count > 0)
        {
            Vector2 Current = OpenSet[0];

            // DUE TO CHANGING THE BELOW LINE ASTAR NOW FINDS ANY TILE AT DISTANCE ONE FROM THE TARGET
            if (Distance(Current, End) == 1)
            {
                return ReconstructPath(CameFrom, Current);
            }

            OpenSet.Remove(Current);

            foreach (Vector2 Neighbour in GetNeighbours(Current))
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
        for (int i = 0; i < 30; i++)
        {
            for (int j = 0; j < 30; j++)
            {
                if (GetCell(i, j) != -1)
                {
                    SetCell(i, j, 0);
                }
            }
        }
    }

    public void ResetAllCellColoursExcept(Vector2 Cell)
    {
        for (int i = 0; i < 30; i++)
        {
            for (int j = 0; j < 30; j++)
            {
                if (!(i == Cell.x && j == Cell.y) && GetCell(i, j) != -1)
                {
                    SetCell(i, j, 0);
                }
            }
        }
    }

    public Unit GetUnitOnTile(Vector2 Tile)
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
            for (int j = 0; j < 5; j++)
            {
                if (GetCell(i, j) != -1)
                {
                    Tiles.Add(new Vector2(i, j));
                }
            }
        }

    }

    public Unit CloneUnit(Unit Unit)
    {
        int UnitIndex = 0;
        // Weird magic function to search the UnitScenes for the right index
        for (int i = 0; i < UnitScenes.Count; i++)
        {
            PackedScene UnitScene = UnitScenes[i];
            Godot.Collections.Dictionary WeirdUnitSceneDictionary = UnitScene._Bundled;
            String[] StringArray = WeirdUnitSceneDictionary["names"] as String[];
            string UnitSceneUnitName = StringArray[0];
            if (UnitSceneUnitName == Unit.UnitName)
            {
                UnitIndex = i;
            }
        }
        Unit ClonedUnit = (Unit)UnitScenes[UnitIndex].Instance();
        ClonedUnit.PlayerOwned = Unit.PlayerOwned;
        if (!ClonedUnit.PlayerOwned)
        {
            ClonedUnit.Texture = (Texture)GD.Load("res://Hexagons/RedHexagon.png");
        }
        ClonedUnit.Health = Unit.Health;
        ClonedUnit.Damage = Unit.Damage;
        ClonedUnit.CurrentCell = Unit.CurrentCell;
        AddChild(ClonedUnit);
        ClonedUnit.Initialise(ClonedUnit.CurrentCell);

        return ClonedUnit;
    }
}

