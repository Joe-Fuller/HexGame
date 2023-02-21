using Godot;
using System;

public class UnitB : Unit
{
    public override void SetStats()
    {
        UnitName = "B";
        Health = 1;
        Damage = 5;
    }
}
