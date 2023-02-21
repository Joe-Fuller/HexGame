using Godot;
using System;

public class UnitC : Unit
{
    public override void SetStats()
    {
        UnitName = "C";
        Health = 3;
        Damage = 3;
        Range = 2;
    }
}
