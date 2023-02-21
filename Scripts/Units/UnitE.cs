using Godot;
using System;

public class UnitE : Unit
{
    // UnitE Damages all of its Neighbouring Tiles
    public override void SetStats()
    {
        UnitName = "E";
        Health = 4;
        Damage = 2;
    }

    public override Godot.Collections.Array<Vector2> GetDamagedTiles()
    {
        Godot.Collections.Array<Vector2> DamagedTiles = Tilemap.GetNeighbours(CurrentCell, true);
        return DamagedTiles;
    }
}
