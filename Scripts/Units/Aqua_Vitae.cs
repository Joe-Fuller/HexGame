using Godot;
using System;

public partial class Aqua_Vitae : Unit
{
    public override void SetStats()
    {
        UnitName = "Aqua Vitae";
        UnitIcon = "\U0001F708";
        Health = 10;
        Damage = 2;
        Haste = true;
        Description = "This Unit is First in Combat";
    }

}
