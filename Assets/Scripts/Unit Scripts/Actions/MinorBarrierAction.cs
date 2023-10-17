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

    public override (int, int) GetActionRange()
    {
        return (0, 3);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        return GetValidActionGridPositionList(unit.GetGridPosition());
    }

    public override List<GridPosition> GetValidActionGridPositionList(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        (int, int) barrierRange = GetActionRange();
        int minBarrierRange = barrierRange.Item1;
        int maxBarrierRange = barrierRange.Item2;

        for (int x = -maxBarrierRange; x <= maxBarrierRange; x++)
        {
            for (int z = -maxBarrierRange; z <= maxBarrierRange; z++)
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

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if ((testDistance > maxBarrierRange) || (testDistance < minBarrierRange))
                {
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
