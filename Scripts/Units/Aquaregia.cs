using Godot;
using System;

public class Aquaregia : Unit
{
    public override void SetStats()
    {
        UnitName = "Aquaregia";
        UnitIcon = "\U0001F706";
        Health = 12;
        Damage = 2;
        Description = "Also Damages Tile Behind the Target";
    }

    public override Godot.Collections.Array<Vector2> GetDamagedTiles()
    {
        // im confident this could be generalised, but it works
        // to expand to more tiles just go again with CurrentCell = TargetCell (OG) and TargetCell = Cell generated first time around

        Godot.Collections.Array<Vector2> DamagedTiles = new Godot.Collections.Array<Vector2>(TargetCell);

        Vector2 Direction = TargetCell - CurrentCell;

        // First Handle Straight Up / Down - They Are Easy
        if (Direction.x == 0 && Math.Abs(Direction.y) == 1)
        {
            DamagedTiles.Add(TargetCell + Direction);
        }

        // Now Split into Even / Odd x
        // Even
        if (CurrentCell.x % 2 == 0)
        {
            // Up + Right
            if (Direction.x == 1 && Direction.y == -1)
            {
                DamagedTiles.Add(CurrentCell + new Vector2(2, -1));
            }

            // Down + Right
            if (Direction.x == 1 && Direction.y == 0)
            {
                DamagedTiles.Add(CurrentCell + new Vector2(2, 1));
            }

            // Down + Left
            if (Direction.x == -1 && Direction.y == 0)
            {
                DamagedTiles.Add(CurrentCell + new Vector2(-2, 1));
            }

            // Up + Left
            if (Direction.x == -1 && Direction.y == -1)
            {
                DamagedTiles.Add(CurrentCell + new Vector2(-2, -1));
            }
        }
        // Odd
        else
        {
            // Up + Right
            if (Direction.x == 1 && Direction.y == 0)
            {
                DamagedTiles.Add(CurrentCell + new Vector2(2, -1));
            }

            // Down + Right
            if (Direction.x == 1 && Direction.y == 1)
            {
                DamagedTiles.Add(CurrentCell + new Vector2(2, 1));
            }

            // Down + Left
            if (Direction.x == -1 && Direction.y == 1)
            {
                DamagedTiles.Add(CurrentCell + new Vector2(-2, 1));
            }

            // Up + Left
            if (Direction.x == -1 && Direction.y == 0)
            {
                DamagedTiles.Add(CurrentCell + new Vector2(-2, -1));
            }
        }

        return DamagedTiles;
    }
}
