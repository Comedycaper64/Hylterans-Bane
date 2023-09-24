using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicalAegisAction : BaseAction
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
        return "A shield of abjuration forms around a set of allies that protects them from damage once. Lasts until broken. \nHeld Actions Used : 2";
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
        List<Unit> targetUnits = GetUnitsInAOE(gridPosition, GetDamageArea(), AOEType.Cube, false);

        foreach (Unit targetUnit in targetUnits)
        {
            AegisEffect aegis = targetUnit.gameObject.AddComponent<AegisEffect>();
            if (GetUnit().GetAbility<AbjuristAbility>())
            {
                aegis.AbjuristAegis();
            }
        }

        ActionStart(onActionComplete);
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction { gridPosition = gridPosition, actionValue = 0 };
    }
}
