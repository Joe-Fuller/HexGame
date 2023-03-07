using Godot;
using System;

public partial class Water : Unit
{
    // UnitE Damages all of its Neighbouring Tiles
    public override void SetStats()
    {
        UnitName = "Water";
        UnitIcon = "\U0001F704";
        Health = 6;
        Damage = 1;
        Description = "Damages ALL Adjacent Tiles";
    }

    public override Godot.Collections.Array<Vector2I> GetAffectedTiles()
    {
        Godot.Collections.Array<Vector2I> DamagedTiles = GetNeighbours();
        return DamagedTiles;
    }
}
