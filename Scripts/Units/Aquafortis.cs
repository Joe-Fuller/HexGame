using Godot;
using System;

public class Aquafortis : Unit
{
    // UnitF Damages Tiles in a Cone in front of itself
    public override void SetStats()
    {
        UnitName = "Aquafortis";
        UnitIcon = "\U0001F705";
        Health = 7;
        Damage = 2;
        Description = "Also Damages Tiles Either Side of Target";
    }

    public override Godot.Collections.Array<Vector2> GetAffectedTiles()
    {
        Godot.Collections.Array<Vector2> DamagedTiles = new Godot.Collections.Array<Vector2>();

        // we can find the affected tiles (for now) by the overlap of the Unit's Neighbours and the TargetUnit's Neighbours

        Godot.Collections.Array<Vector2> TargetUnitsNeighbours = TargetUnit.GetNeighbours();

        foreach (Vector2 Tile in GetNeighbours())
        {
            if (Tilemap.Distance(Tile, TargetCell) <= 1)
            {
                DamagedTiles.Add(Tile);
            }
        }

        return DamagedTiles;
    }
}