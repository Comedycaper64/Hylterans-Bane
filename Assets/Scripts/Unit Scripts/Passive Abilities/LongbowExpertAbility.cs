using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongbowExpertAbility : PassiveAbility
{
    private int toHitBonus = 2;
    private int rangeBonus = 1;

    private MoveAction moveAction;
    private UnitStats unitStats;

    private bool buffActive = false;

    private void Start()
    {
        moveAction = GetComponent<MoveAction>();
        unitStats = unit.GetUnitStats();
        moveAction.OnStartMoving += MoveAction_OnStartMoving;
        unit.OnUnitTurnEnd += Unit_OnUnitTurnEnd;
    }

    private void OnDisable()
    {
        moveAction.OnStartMoving -= MoveAction_OnStartMoving;
        unit.OnUnitTurnEnd -= Unit_OnUnitTurnEnd;
    }

    public override string GetAbilityDescription()
    {
        return "Not moving on your turn grants +2 to hit and +1 range";
    }

    public override string GetAbilityName()
    {
        return "Longbow Expert";
    }

    private void BuffUnit(bool enable)
    {
        buffActive = enable;
        if (enable)
        {
            unitStats.currentStatBonus += new StatBonus(toHitBonus, 0, rangeBonus);
        }
        else
        {
            unitStats.currentStatBonus -= new StatBonus(toHitBonus, 0, rangeBonus);
        }
    }

    private void MoveAction_OnStartMoving(object sender, int distanceMoved)
    {
        if ((distanceMoved == 1) && !buffActive)
        {
            BuffUnit(true);
        }
        else if ((distanceMoved != 1) && buffActive)
        {
            BuffUnit(false);
        }
    }

    private void Unit_OnUnitTurnEnd()
    {
        if (buffActive)
        {
            BuffUnit(false);
        }
    }
}
