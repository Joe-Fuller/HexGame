using Godot;
using System;

public class Venus : Unit
{
    public override void SetStats()
    {
        UnitName = "Venus";
        UnitIcon = "\U00002640";
        Health = 6;
        Damage = 2;
        Description = "Takes 1 less Damage from Attacks";
    }

    public override void TakeDamage(int IncDamage)
    {
        CombatHealth -= (IncDamage - 1);
    }
}
