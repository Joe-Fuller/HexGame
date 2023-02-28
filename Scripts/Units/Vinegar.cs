using Godot;
using System;

public class Vinegar : Unit
{
    // UnitE Damages all of its Neighbouring Tiles
    public override void SetStats()
    {
        UnitName = "Vinegar";
        UnitIcon = "\U0001F70A";
        Health = 14;
        Damage = 2;
        Description = "Targets the Weakest Enemy";
    }

    public override void GetTarget(Godot.Collections.Array<Unit> Units)
    {
        int MinHealth = 1000;

        // look through each enemy unit, find closest
        foreach (Unit Unit in Units)
        {
            if (Unit.PlayerOwned != this.PlayerOwned && Unit != this && Unit.CombatHealth < MinHealth)
            {
                MinHealth = Unit.CombatHealth;
                TargetUnit = Unit;
                TargetCell = TargetUnit.CurrentCell;
            }
        }
    }
}
