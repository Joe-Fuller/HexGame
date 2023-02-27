using Godot;
using System;

public class Aqua_Vitae : Unit
{
    public override void SetStats()
    {
        UnitName = "Aqua Vitae";
        UnitIcon = "\U0001F708";
        Health = 5;
        Damage = 2;
        Haste = true;
        Description = "This Unit is First in Combat";
    }

}
