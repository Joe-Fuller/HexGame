using Godot;
using System;

public class Unit : Sprite
{
    public string UnitName;
    public string UnitIcon;
    public int Health;
    public int Damage;
    public int Range = 1;
    public int Movement = 1;

    public int MovesThisTurn = 0;
    public Vector2 CurrentCell;
    public Vector2 TargetCell;
    public Unit TargetUnit;
    public bool PlayerOwned;

    public TileMap Tilemap;

    private Label IconText;
    private Label HealthText;
    private Label DamageText;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SetStats();

        Tilemap = GetNode<TileMap>("..");

        //Tilemap = GetNode<TileMap>("../TileMap");
        IconText = GetNode<Label>("IconText");
        HealthText = GetNode<Label>("HealthText");
        DamageText = GetNode<Label>("DamageText");

        IconText.Text = UnitIcon;
        HealthText.Text = Health.ToString();
        DamageText.Text = Damage.ToString();
    }

    public virtual void SetStats()
    {
        UnitName = "X";
        Health = 1;
        Damage = 1;
        Range = 1;
        Movement = 1;
    }

    public void GetTarget(Godot.Collections.Array<Unit> Units)
    {
        int MinDist = 100;

        // look through each enemy unit, find closest
        foreach (Unit Unit in Units)
        {
            if (Unit.PlayerOwned != this.PlayerOwned && Unit != this && Tilemap.Distance(CurrentCell, Unit.CurrentCell) < MinDist)
            {
                TargetUnit = Unit;
                TargetCell = TargetUnit.CurrentCell;
            }
        }
    }

    public void Move(Vector2 CellCoords)
    {
        if (Tilemap == null)
        {
            Tilemap = GetNode<TileMap>("..");
        }
        CurrentCell = CellCoords;
        this.Position = Tilemap.MapToWorld(CurrentCell);
    }

    public void MoveTowardTarget()
    {
        Godot.Collections.Array<Vector2> Path = Tilemap.AStar(CurrentCell, TargetCell);
        Vector2 NextCell = Path[Path.Count - 2];
        Move(NextCell);
    }

    public Vector2 GetNextMove()
    {
        Godot.Collections.Array<Vector2> Path = Tilemap.AStar(CurrentCell, TargetCell);
        Vector2 NextCell = Path[Path.Count - 2];
        return NextCell;
    }

    public virtual Godot.Collections.Array<Vector2> GetDamagedTiles()
    {
        Godot.Collections.Array<Vector2> DamagedTiles = new Godot.Collections.Array<Vector2>();

        DamagedTiles.Add(TargetCell);

        return DamagedTiles;
    }

    public void Initialise(Vector2 CellCoords)
    {
        Move(CellCoords);
    }

    public bool CurrentCellIsHighlighted()
    {
        return Tilemap.GetCell((int)CurrentCell.x, (int)CurrentCell.y) == (PlayerOwned ? 3 : 4);
    }

    public bool CanAttack()
    {
        return Tilemap.Distance(CurrentCell, TargetCell) <= Range;
    }

    public bool CanMove()
    {
        return Movement - MovesThisTurn > 0;
    }

    public void UpdateText()
    {
        HealthText.Text = Health.ToString();
        DamageText.Text = Damage.ToString();
    }

    public Godot.Collections.Array<Vector2> GetNeighbours()
    {
        return Tilemap.GetNeighbours(CurrentCell, true);
    }
}
