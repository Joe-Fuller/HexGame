using Godot;
using System;

public partial class Sulphur : Unit
{
    // UnitE Damages all of its Neighbouring Tiles
    public override void SetStats()
    {
        UnitName = "Sulphur";
        UnitIcon = "\U0001F70D";
        Health = 6;
        Damage = 0;
        DoesBuffs = true;
        Description = "Does no Damage. Instead Heals an Allied Unit for 2";
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

            if (Unit.PlayerOwned == this.PlayerOwned && Unit != this && Unit.CombatHealth < Unit.Health && Tilemap.Distance(CurrentCell, Unit.CurrentCell) < MinDist)
            {
                MinDist = Tilemap.Distance(CurrentCell, Unit.CurrentCell);
                TargetUnit = Unit;
                TargetCell = TargetUnit.CurrentCell;
            }
        }
    }

    public override void DoBuff(Unit TargetUnit)
    {
        if (TargetUnit.CombatHealth < TargetUnit.Health)
        {
            TargetUnit.CombatHealth += 2;
            if (TargetUnit.CombatHealth > TargetUnit.Health)
            {
                TargetUnit.CombatHealth = TargetUnit.Health;
            }
        }
    }
}
