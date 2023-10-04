using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinorBarrierAction : BaseAction
{
    private Unit shieldedUnit;
    private int acBuff = 5;

    float waitTimer = 1f;

    private void Start()
    {
        unit.OnUnitTurnStart += Unit_OnUnitTurnStart;
    }

    private void OnDisable()
    {
        unit.OnUnitTurnStart -= Unit_OnUnitTurnStart;
    }

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
        return "Formulates a simple ward on an ally that increases their AC by 5 until this unit's next turn.";
    }

    public override string GetActionName()
    {
        return "Minor Barrier";
    }

    public override int GetActionRange()
    {
        return 3;
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        return GetValidActionGridPositionList(unit.GetGridPosition());
    }

    public override List<GridPosition> GetValidActionGridPositionList(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        int barrierRange = GetActionRange();

        for (int x = -barrierRange; x <= barrierRange; x++)
        {
            for (int z = -barrierRange; z <= barrierRange; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = gridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    // Grid Position is empty, no Unit
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

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
        shieldedUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        shieldedUnit.GetUnitStats().currentStatBonus += new StatBonus(0, 0, 0, acBuff);

        ActionStart(onActionComplete);
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction { gridPosition = gridPosition, actionValue = -1, };
    }

    private void Unit_OnUnitTurnStart()
    {
        if (shieldedUnit)
        {
            shieldedUnit.GetUnitStats().currentStatBonus -= new StatBonus(0, 0, 0, acBuff);
            shieldedUnit = null;
        }
    }
}
