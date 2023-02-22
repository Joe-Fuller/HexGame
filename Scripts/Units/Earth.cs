using Godot;
using System;

public class Earth : Unit
{
    public override void SetStats()
    {
        UnitName = "Earth";
        UnitIcon = "\U0001F703";
        Health = 2;
        Damage = 2;
        Movement = 2;
    }

}