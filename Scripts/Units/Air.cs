using Godot;
using System;

public class Air : Unit
{
    public override void SetStats()
    {
        UnitName = "Air";
        UnitIcon = "\U0001F701";
        // UnitIcon = "\uD83D\uDF01";
        Health = 10;
        Damage = 2;
        Description = "";
    }

}
