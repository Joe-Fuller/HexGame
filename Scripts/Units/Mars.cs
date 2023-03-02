using Godot;
using System;

public class Mars : Unit
{
    public override void SetStats()
    {
        UnitName = "Mars";
        UnitIcon = "\U00002642";
        Health = 15;
        Damage = 1;
        Description = "Deals 1 Damage to Attackers";
    }

    public override void TakeDamage(int IncDamage, Unit AttackingUnit)
    {
        CombatHealth -= IncDamage;
        AttackingUnit.CombatHealth -= 1;
        AttackingUnit.UpdateText();
    }

}
