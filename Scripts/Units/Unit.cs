using Godot;
using System;

public partial class Unit : Sprite2D
{
    public string UnitName;
    public string UnitIcon;
    public int Health;
    public int Damage;
    public string Description;
    public int Range = 1;
    public int Movement = 1;
    public bool Haste = false;
    public bool DoesBuffs = false;
    public int CombatHealth;

    public int MovesThisTurn = 0;
    public Vector2I CurrentCell;
    public Vector2I TargetCell;
    public Unit TargetUnit;
    public bool PlayerOwned;

    public TileMap Tilemap;

    private Label IconText;
    private Label HealthText;
    private Label DamageText;
    private Label DescriptionText;

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
        Description = "X";
        Range = 1;
        Movement = 1;
        Haste = false;
        DoesBuffs = false;
    }

    public virtual void GetTarget(Godot.Collections.Array<Unit> Units)
    {
        int MinDist = 100;

        // look through each enemy unit, find closest
        foreach (Unit Unit in Units)
        {
            if (Unit.PlayerOwned != this.PlayerOwned && Unit != this && Tilemap.Distance(CurrentCell, Unit.CurrentCell) < MinDist)
            {
                MinDist = Tilemap.Distance(CurrentCell, Unit.CurrentCell);
                TargetUnit = Unit;
                TargetCell = TargetUnit.CurrentCell;
            }
        }
    }

    public void Move(Vector2I CellCoords)
    {
        if (Tilemap == null)
        {
            Tilemap = GetNode<TileMap>("..");
        }
        CurrentCell = CellCoords;
        this.Position = Tilemap.MapToLocal(CurrentCell);
    }

    public void MoveTowardTarget()
    {
        Godot.Collections.Array<Vector2I> Path3D = Tilemap.AStar3D(CurrentCell, TargetCell);
        Vector2I NextCell = (Vector2I)Path3D[Path3D.Count - 2];
        Move(NextCell);
    }

    public Vector2I GetNextMove()
    {
        Godot.Collections.Array<Vector2I> Path3D = Tilemap.AStar3D(CurrentCell, TargetCell);
        Vector2I NextCell = (Vector2I)Path3D[Path3D.Count - 2];
        return NextCell;
    }

    public virtual Godot.Collections.Array<Vector2I> GetAffectedTiles()
    {
        Godot.Collections.Array<Vector2I> DamagedTiles = new Godot.Collections.Array<Vector2I>();

        DamagedTiles.Add(TargetCell);

        return DamagedTiles;
    }

    public void Initialise(Vector2I CellCoords)
    {
        Move(CellCoords);
    }

    public bool CurrentCellIsHighlighted()
    {
        return Tilemap.GetCellSourceId(0, CurrentCell) == (PlayerOwned ? 3 : 4);
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
        HealthText.Text = CombatHealth.ToString();
        DamageText.Text = Damage.ToString();
    }

    public Godot.Collections.Array<Vector2I> GetNeighbours()
    {
        return Tilemap.GetNeighbours(CurrentCell, true);
    }

    public virtual void DoBuff(Unit Unit)
    { }

    public virtual void DealDamage(Unit Unit)
    {
        Unit.TakeDamage(Damage, this);
        Unit.UpdateText();
    }

    public virtual void TakeDamage(int IncDamage, Unit AttackingUnit)
    {
        CombatHealth -= IncDamage;
    }
}
