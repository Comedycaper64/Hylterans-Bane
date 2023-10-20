using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(MultiAttackAction))]
public class DualWieldingProdigyAbility : PassiveAbility
{
    AttackAction attackAction;
    bool attackedThisTurn = false;

    private void Start()
    {
        attackAction = unit.GetAction<AttackAction>();
        attackAction.OnActionStarted += AttackAction_OnActionStarted;
        unit.OnUnitTurnStart += Unit_OnUnitTurnStart;
    }

    private void OnDisable()
    {
        attackAction.OnActionStarted -= AttackAction_OnActionStarted;
        unit.OnUnitTurnStart -= Unit_OnUnitTurnStart;
    }

    private void Unit_OnUnitTurnStart()
    {
        attackedThisTurn = false;
    }

    private void AttackAction_OnActionStarted(object sender, EventArgs e)
    {
        if (!attackedThisTurn)
        {
            unit.IncreaseSpirit();
            attackedThisTurn = true;
        }
    }

    public override string GetAbilityDescription()
    {
        return "The first attack this unit makes on their turn doesn't cost Spirit.\n<i>'Many envied the dextrous style unique to the Swiftblade.'</i>";
    }

    public override string GetAbilityName()
    {
        return "Dual-Wielding Prodigy";
    }
}
