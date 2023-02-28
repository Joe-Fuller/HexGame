using Godot;
using System;

public class Sulphur : Unit
{
    // UnitE Damages all of its Neighbouring Tiles
    public override void SetStats()
    {
        UnitName = "Sulphur";
        UnitIcon = "\U0001F70D";
        Health = 6;
        Damage = 0;
        Description = "Does no Damage. Instead Heals an Allied Unit";
    }

    public override Godot.Collections.Array<Vector2> GetDamagedTiles()
    {
        Godot.Collections.Array<Vector2> DamagedTiles = GetNeighbours();
        return DamagedTiles;
    }
}
