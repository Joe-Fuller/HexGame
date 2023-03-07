using Godot;
using System;

public partial class Sun : Unit
{
    public override void SetStats()
    {
        UnitName = "Sun";
        UnitIcon = "\U00002609";
        Health = 6;
        Damage = 2;
        Description = "Heals for Damage Dealt";
    }

    public override void DealDamage(Unit Unit)
    {
        int UnitHealth = Unit.CombatHealth;
        Unit.CombatHealth -= Damage;
        CombatHealth += UnitHealth - Unit.CombatHealth;
        if (CombatHealth > Health)
        {
            CombatHealth = Health;
        }
        Unit.UpdateText();
        UpdateText();
    }
}
