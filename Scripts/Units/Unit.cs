using Godot;
using System;

public class Unit : Sprite
{
    public string UnitName;
    public int Health;
    public int Damage;
    public int Range = 1;
    public Vector2 CurrentCell;
    public Vector2 TargetCell;
    public Unit TargetUnit;
    public bool PlayerOwned;

    private TileMap Tilemap;

    private Label NameText;
    private Label HealthText;
    private Label DamageText;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SetStats();

        //Tilemap = GetNode<TileMap>("../TileMap");
        NameText = GetNode<Label>("NameText");
        HealthText = GetNode<Label>("HealthText");
        DamageText = GetNode<Label>("DamageText");

        NameText.Text = UnitName;
        HealthText.Text = Health.ToString();
        DamageText.Text = Damage.ToString();
    }

    public virtual void SetStats()
    {
        UnitName = "X";
        Health = 1;
        Damage = 1;
        Range = 1;
    }

    public void GetTarget()
    {
        // look through each enemy unit, find closest
        foreach (Unit Unit in Tilemap.Units)
        {
            if (Unit.PlayerOwned != this.PlayerOwned && Unit != this)
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

    public void Attack()
    {
        TargetUnit.TakeDamage(Damage);
    }

    public void TakeDamage(int IncDamage)
    {
        Health -= IncDamage;
        if (Health <= 0)
        {
            Tilemap.Units.Remove(this);
            QueueFree();
        }
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

    public void UpdateText()
    {
        HealthText.Text = Health.ToString();
        DamageText.Text = Damage.ToString();
    }
}
