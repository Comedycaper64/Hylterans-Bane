using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongbowExpertAbility : PassiveAbility
{
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
        return "Not moving on your turn grants advantage on attack rolls and +1 range on the current turn's attack. \n<i>'Extra time spent aiming a killing blow tends to pay off.'</i>";
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
            unitStats.currentStatBonus += new StatBonus(0, 0, rangeBonus);
            unitStats.attackAugment += 1;
        }
        else
        {
            unitStats.currentStatBonus -= new StatBonus(0, 0, rangeBonus);
            unitStats.attackAugment -= 1;
        }
    }

    private void MoveAction_OnStartMoving(object sender, int distanceMoved)
    {
        if (IsDisabled())
        {
            return;
        }

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
