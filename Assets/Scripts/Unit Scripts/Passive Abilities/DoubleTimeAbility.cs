using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleTimeAbility : PassiveAbility
{
    private MoveAction moveAction;
    private bool unitMoved;

    private void Start()
    {
        moveAction = GetComponent<MoveAction>();
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
        return "Not moving on your turn grants an held action.";
    }

    public override string GetAbilityName()
    {
        return "Double Time!";
    }

    private void MoveAction_OnStartMoving(object sender, int distanceMoved)
    {
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
        if (!unitMoved)
        {
            unit.IncreaseHeldActions();
        }
    }
}
