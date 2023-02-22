using Godot;
using System;

public class Fire : Unit
{
    public override void SetStats()
    {
        UnitName = "Fire";
        UnitIcon = "\U0001F702";
        Health = 3;
        Damage = 3;
        Range = 2;
    }
}