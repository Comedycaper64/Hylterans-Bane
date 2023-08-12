using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicalAegisAction : BaseAction
{
    private List<Unit> targetUnits = new List<Unit>();

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
        return "A shield of abjuration protects your allies from damage once.";
    }

    public override string GetActionName()
    {
        return "Magical Aegis";
    }

    public override int GetActionRange()
    {
        return 4;
    }

    public override int GetRequiredHeldActions()
    {
        return 2;
    }

    public override (int, int) GetDamageArea()
    {
        return (3, 3);
    }

    public override bool GetIsAOE()
    {
        return true;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction { gridPosition = gridPosition, actionValue = 0 };
    }

    public override List<GridPosition> GetValidActionGridPositionList(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList();
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();
        int maxCastDistance = GetActionRange();

        for (int x = -maxCastDistance; x <= maxCastDistance; x++)
        {
            for (int z = -maxCastDistance; z <= maxCastDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxCastDistance)
                {
                    continue;
                }

                float damageRadius = 3f;
                List<Unit> tempUnitList = new List<Unit>();
                Collider[] colliderArray = Physics.OverlapSphere(
                    LevelGrid.Instance.GetWorldPosition(testGridPosition),
                    damageRadius
                );
                foreach (Collider collider in colliderArray)
                {
                    if (collider.TryGetComponent<Unit>(out Unit tempUnit))
                    {
                        if (!tempUnit.IsEnemy())
                        {
                            tempUnitList.Add(tempUnit);
                        }
                    }
                }

                if (tempUnitList.Count < 1)
                {
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnits = new List<Unit>();

        var damageRange = GetDamageArea();

        (damageRange.Item1, damageRange.Item2) = (
            Mathf.RoundToInt(damageRange.Item1 - 1) / 2,
            Mathf.RoundToInt(damageRange.Item2 - 1) / 2
        );

        for (int x = -damageRange.Item1; x <= damageRange.Item1; x++)
        {
            for (int z = -damageRange.Item2; z <= damageRange.Item2; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = gridPosition + offsetGridPosition;

                if (
                    LevelGrid.Instance.TryGetUnitAtGridPosition(
                        testGridPosition,
                        out Unit targetUnit
                    ) && (!targetUnit.IsEnemy())
                )
                {
                    targetUnit.gameObject.AddComponent<AegisEffect>();
                }
            }
        }

        ActionStart(onActionComplete);
    }
}
