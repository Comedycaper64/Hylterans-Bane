using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticalCommandAction : BaseAction
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
        return "The presence of the queen inspires her troops even in the most dire straits.";
    }

    public override string GetActionName()
    {
        return "Tactical Command";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction { gridPosition = gridPosition, actionValue = -1, };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();

        return new List<GridPosition> { unitGridPosition };
    }

    public override List<GridPosition> GetValidActionGridPositionList(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList();
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        //Gets friendly units around unit. Increases their held actions
        ActionStart(onActionComplete);
    }
}
