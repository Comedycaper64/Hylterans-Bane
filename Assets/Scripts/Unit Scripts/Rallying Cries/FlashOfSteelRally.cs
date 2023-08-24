using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashOfSteelRally : RallyingCry
{
    private int abilityRange = 2;
    private Queue<Unit> unitsInRange;

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
            waitTimer = 0.5f;
            if (unitsInRange.TryDequeue(out Unit attackingUnit))
            {
                UnitAttack(attackingUnit);
            }
            else
            {
                AbilityComplete();
            }
        }
    }

    public override string GetAbilityDescription()
    {
        return "'Now! Strike them!' \nAllows each unit within 2 spaces of Firkin to perform an attack on the closest enemy within their range, prioritising lower health enemies.";
    }

    public override string GetAbilityName()
    {
        return "Flash of Steel";
    }

    public override void PerformAbility(Action onAbilityCompleted)
    {
        GridPosition unitPosition = unit.GetGridPosition();
        List<Unit> friendlyUnits = UnitManager.Instance.GetFriendlyUnitList();
        unitsInRange = new Queue<Unit>();

        foreach (Unit friendlyUnit in friendlyUnits)
        {
            GridPosition friendlyUnitPosition = friendlyUnit.GetGridPosition();
            GridPosition gridDistanceBetweenUnits = friendlyUnitPosition - unitPosition;
            int distanceBetweenUnits =
                Mathf.Abs(gridDistanceBetweenUnits.x) + Mathf.Abs(gridDistanceBetweenUnits.z);
            bool unitOutOfRange = distanceBetweenUnits > abilityRange ? true : false;

            if (!unitOutOfRange && friendlyUnit.GetAction<AttackAction>())
            {
                unitsInRange.Enqueue(friendlyUnit);
            }
        }

        AbilityStart(onAbilityCompleted);
    }

    private void UnitAttack(Unit attackingUnit)
    {
        isActive = false;
        AttackAction attackAction = attackingUnit.GetAction<AttackAction>();
        List<GridPosition> attackablePositions = attackAction.GetValidActionGridPositionList();
        if (attackablePositions.Count == 0)
        {
            UnitAttacked();
            return;
        }

        List<Unit> attackableEnemies = new List<Unit>();
        foreach (GridPosition position in attackablePositions)
        {
            if (LevelGrid.Instance.TryGetUnitAtGridPosition(position, out Unit enemyUnit))
            {
                attackableEnemies.Add(enemyUnit);
            }
        }
        attackableEnemies.Sort((Unit a, Unit b) => (int)b.GetHealth() - (int)a.GetHealth());

        attackAction.TakeAction(attackableEnemies[0].GetGridPosition(), UnitAttacked);
    }

    private void UnitAttacked()
    {
        isActive = true;
    }
}
