using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitAction : BaseAction
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

    public override string GetActionName()
    {
        return "Wait";
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();

        return new List<GridPosition> { unitGridPosition };
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction { gridPosition = gridPosition, actionValue = 1, };
    }
}
