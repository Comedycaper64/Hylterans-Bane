using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeclarationRally : RallyingCry
{
    private int abilityRange = 2;

    public override string GetAbilityDescription()
    {
        return "'Brave soldiers of Korvard, this marks the turning of the tide!' \nGrants each unit within 2 spaces of the queen 1 held action.";
    }

    public override string GetAbilityName()
    {
        return "Declaration";
    }

    public override void PerformAbility()
    {
        GridPosition unitPosition = unit.GetGridPosition();
        List<Unit> friendlyUnits = UnitManager.Instance.GetFriendlyUnitList();

        foreach (Unit friendlyUnit in friendlyUnits)
        {
            GridPosition friendlyUnitPosition = friendlyUnit.GetGridPosition();
            GridPosition gridDistanceBetweenUnits = friendlyUnitPosition - unitPosition;
            int distanceBetweenUnits =
                Mathf.Abs(gridDistanceBetweenUnits.x) + Mathf.Abs(gridDistanceBetweenUnits.z);
            bool unitOutOfRange = distanceBetweenUnits > abilityRange ? true : false;

            if (!unitOutOfRange)
            {
                friendlyUnit.IncreaseHeldActions();
            }
        }
    }
}
