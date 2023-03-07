using Godot;
using System;

public partial class Mercury : Unit
{
    // UnitE Damages all of its Neighbouring Tiles
    public override void SetStats()
    {
        UnitName = "Mercury";
        UnitIcon = "\U0000263F";
        Health = 4;
        Damage = 0;
        DoesBuffs = true;
        Description = "Does no Damage. Instead Gives an Allied Unit +1 Damage";
    }

    public override void GetTarget(Godot.Collections.Array<Unit> Units)
    {

        int MinDist = 100;

        // look through each enemy unit, find closest
        foreach (Unit Unit in Units)
        {
            // set default target in case conditions arent met
            if (TargetUnit == null && Unit.PlayerOwned == this.PlayerOwned)
            {
                TargetUnit = Unit;
                TargetCell = Unit.CurrentCell;
            }

            if (Unit.PlayerOwned == this.PlayerOwned && Unit != this && Tilemap.Distance(CurrentCell, Unit.CurrentCell) < MinDist)
            {
                MinDist = Tilemap.Distance(CurrentCell, Unit.CurrentCell);
                TargetUnit = Unit;
                TargetCell = TargetUnit.CurrentCell;
            }
        }
    }

    public override void DoBuff(Unit TargetUnit)
    {
        TargetUnit.Damage += 1;
    }
}

