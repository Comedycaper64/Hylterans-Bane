using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelegationAction : BaseAction
{
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
            ActionComplete();
        }
    }

    public override string GetActionDescription()
    {
        return "Allows an adjacent unit to have a turn after this unit. \nHeld Actions Used : 1 \n'A queen can't be expected to do all the work. That's what subjects are for.'";
    }

    public override string GetActionName()
    {
        return "Delegation";
    }

    public override int GetRequiredHeldActions()
    {
        return 1;
    }

    public override (int, int) GetActionRange()
    {
        return (1, 1);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        return GetValidActionGridPositionList(unit.GetGridPosition());
    }

    public override List<GridPosition> GetValidActionGridPositionList(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        int delegationRange = GetActionRange().Item2;

        for (int x = -delegationRange; x <= delegationRange; x++)
        {
            for (int z = -delegationRange; z <= delegationRange; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = gridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > delegationRange)
                {
                    continue;
                }

                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    // Grid Position is empty, no Unit
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (targetUnit == unit)
                {
                    continue;
                }

                if (targetUnit.IsEnemy() != unit.IsEnemy())
                {
                    // Both Units on same 'team'
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        //Makes unit go next
        //unit.UseHeldActions(GetRequiredHeldActions());
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        TurnSystem.Instance.AddInitiativeToOrder(new Initiative(targetUnit, 0));
        ActionStart(onActionComplete);
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction { gridPosition = gridPosition, actionValue = -1, };
    }
}
