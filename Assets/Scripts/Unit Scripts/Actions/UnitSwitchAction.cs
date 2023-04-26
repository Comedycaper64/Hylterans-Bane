using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSwitchAction : BaseAction
{
    void Update()
    {
        if (!isActive)
        {
            return;
        }
        ActionComplete();
    }

    public override string GetActionName()
    {
        return "Select Unit";
    }

    public override int GetActionPointsCost()
    {
        return 0;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();
        int gridX = LevelGrid.Instance.GetWidth();
        int gridZ = LevelGrid.Instance.GetHeight();

        for (int x = 0; x <= gridX; x++)
        {
            for (int z = 0; z <= gridZ; z++)
            {
                GridPosition testGridPosition = new GridPosition(x, z);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    // Grid Position not occupied with unit
                    continue;
                }

                if (unitGridPosition == testGridPosition)
                {
                    // Same Grid Position where the unit is already at
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (targetUnit.IsEnemy() != unit.IsEnemy())
                {
                    // Units on different teams
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        LevelGrid.Instance.GetUnitAtGridPosition(gridPosition).TryGetComponent<Unit>(out Unit unit);
        UnitActionSystem.Instance.SetSelectedUnit(unit);
        ActionStart(onActionComplete);
    }
}
