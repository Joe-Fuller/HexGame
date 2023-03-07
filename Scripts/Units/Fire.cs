using Godot;
using System;

public partial class Fire : Unit
{
    public override void SetStats()
    {
        UnitName = "Fire";
        UnitIcon = "\U0001F702";
        Health = 5;
        Damage = 1;
        Description = "Can Attack Units 2 Tiles Away";
        Range = 2;
    }
}