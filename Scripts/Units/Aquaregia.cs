using Godot;
using System;

public partial class Aquaregia : Unit
{
    public override void SetStats()
    {
        UnitName = "Aquaregia";
        UnitIcon = "\U0001F706";
        Health = 12;
        Damage = 2;
        Description = "Also Damages Tile Behind the Target";
    }

    public override Godot.Collections.Array<Vector2I> GetAffectedTiles()
    {
        // im confident this could be generalised, but it works
        // to expand to more tiles just go again with CurrentCell = TargetCell (OG) and TargetCell = Cell generated first time around

        Godot.Collections.Array<Vector2I> DamagedTiles = new Godot.Collections.Array<Vector2I>();
        DamagedTiles.Add(TargetCell);

        Vector2I Direction = TargetCell - CurrentCell;

        // First Handle Straight Up / Down - They Are Easy
        if (Direction.X == 0 && Math.Abs(Direction.Y) == 1)
        {
            DamagedTiles.Add(TargetCell + Direction);
        }

        // Now Split into Even / Odd x
        // Even
        if (CurrentCell.X % 2 == 0)
        {
            // Up + Right
            if (Direction.X == 1 && Direction.Y == -1)
            {
                DamagedTiles.Add(CurrentCell + new Vector2I(2, -1));
            }

            // Down + Right
            if (Direction.X == 1 && Direction.Y == 0)
            {
                DamagedTiles.Add(CurrentCell + new Vector2I(2, 1));
            }

            // Down + Left
            if (Direction.X == -1 && Direction.Y == 0)
            {
                DamagedTiles.Add(CurrentCell + new Vector2I(-2, 1));
            }

            // Up + Left
            if (Direction.X == -1 && Direction.Y == -1)
            {
                DamagedTiles.Add(CurrentCell + new Vector2I(-2, -1));
            }
        }
        // Odd
        else
        {
            // Up + Right
            if (Direction.X == 1 && Direction.Y == 0)
            {
                DamagedTiles.Add(CurrentCell + new Vector2I(2, -1));
            }

            // Down + Right
            if (Direction.X == 1 && Direction.Y == 1)
            {
                DamagedTiles.Add(CurrentCell + new Vector2I(2, 1));
            }

            // Down + Left
            if (Direction.X == -1 && Direction.Y == 1)
            {
                DamagedTiles.Add(CurrentCell + new Vector2I(-2, 1));
            }

            // Up + Left
            if (Direction.X == -1 && Direction.Y == 0)
            {
                DamagedTiles.Add(CurrentCell + new Vector2I(-2, -1));
            }
        }

        return DamagedTiles;
    }
}
