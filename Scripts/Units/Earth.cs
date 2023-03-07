using Godot;
using System;

public partial class Earth : Unit
{
    public override void SetStats()
    {
        UnitName = "Earth";
        UnitIcon = "\U0001F703";
        Health = 1;
        Damage = 5;
        Description = "Can Move Twice Each Turn";
        Movement = 2;
    }

}