using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleTimeAbility : PassiveAbility
{
    private MoveAction moveAction;
    private bool unitMoved;

    private void Start()
    {
        moveAction = unit.GetAction<MoveAction>();
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
        return "Not moving on your turn grants a held action. This does not apply when the unit is overexerted.";
    }

    public override string GetAbilityName()
    {
        return "Double Time!";
    }

    private void MoveAction_OnStartMoving(object sender, int distanceMoved)
    {
        if (IsDisabled())
        {
            return;
        }
        if (distanceMoved == 1)
        {
            unitMoved = false;
        }
        else if (distanceMoved != 1)
        {
            unitMoved = true;
        }
    }

    private void Unit_OnUnitTurnEnd()
    {
        if (IsDisabled())
        {
            return;
        }
        if (!unitMoved)
        {
            unit.IncreaseSpirit();
        }
    }
}
