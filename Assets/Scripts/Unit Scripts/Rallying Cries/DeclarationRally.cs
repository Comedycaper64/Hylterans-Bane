using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeclarationRally : RallyingCry
{
    private int abilityRange = 2;

    float waitTimer = 1f;

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        waitTimer -= Time.deltaTime;
        if (waitTimer < 0f)
        {
            waitTimer = 1f;
            AbilityComplete();
        }
    }

    public override string GetAbilityDescription()
    {
        return "Grants each unit within 2 spaces of the queen 1 held action.\nHeld Actions Used: 0 \n<i>'Brave soldiers of Korvard, this marks the turning of the tide!'</i>";
    }

    public override string GetAbilityName()
    {
        return "Declaration";
    }

    public override void PerformAbility(Action onAbilityCompleted)
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
                friendlyUnit.IncreaseSpirit();
            }
        }

        AbilityStart(onAbilityCompleted);
    }
}
