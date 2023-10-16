using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhirlingSteelAbility : PassiveAbility
{
    private AttackAction attackAction;
    private StatBonus statBonus = new StatBonus();
    private UnitStats unitStats;

    private void Start()
    {
        attackAction = unit.GetAction<AttackAction>();
        unitStats = unit.GetUnitStats();
        attackAction.OnUnitHit += AttackAction_OnUnitHit;
        unit.OnUnitTurnStart += Unit_OnUnitTurnStart;
    }

    private void OnDisable()
    {
        attackAction.OnUnitHit -= AttackAction_OnUnitHit;
        unit.OnUnitTurnStart -= Unit_OnUnitTurnStart;
    }

    private void Unit_OnUnitTurnStart()
    {
        if (IsDisabled())
        {
            return;
        }
        unitStats.currentStatBonus -= statBonus;
        statBonus = new StatBonus();
    }

    private void AttackAction_OnUnitHit(object sender, Unit e)
    {
        if (IsDisabled())
        {
            return;
        }
        unitStats.currentStatBonus -= statBonus;
        statBonus = new StatBonus(0, 0, 0, statBonus.acBonus + 1);
        unitStats.currentStatBonus += statBonus;
    }

    public override string GetAbilityDescription()
    {
        return "Every time this unit attacks using the Attack action it gains +1 to its AC. This bonus resets on the start of its turn. \n<i>'A cyclone of steel forms an impenetrable barrier around the wielder.'</i>";
    }

    public override string GetAbilityName()
    {
        return "Whirling Steel";
    }
}
