using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    //[SerializeField]
    private string actionDescription;
    public event EventHandler<int> OnStartMoving;
    public event EventHandler OnStopMoving;
    public static event EventHandler<GridPosition> OnAnyUnitMoved;

    [SerializeField]
    private int maxMoveDistance = 4;

    private List<Vector3> positionList;
    private int currentPositionIndex;

    public override string GetActionName()
    {
        return "Move";
    }

    public override string GetActionDescription()
    {
        return actionDescription;
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        Vector3 targetPosition = positionList[currentPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        float rotateSpeed = 10f;
        transform.forward = Vector3.Lerp(
            transform.forward,
            moveDirection,
            Time.deltaTime * rotateSpeed
        );

        float stoppingDistance = .1f;
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            float moveSpeed = 8f;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
        else
        {
            currentPositionIndex++;
            if (currentPositionIndex >= positionList.Count)
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);

                ActionComplete();
            }
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        //Debug.Log("Going from " + unit.GetGridPosition() + " to " + gridPosition);

        List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(
            unit.GetGridPosition(),
            gridPosition,
            out int pathLength
        );

        currentPositionIndex = 0;
        positionList = new List<Vector3>();

        //Debug.Log(pathGridPositionList);

        foreach (GridPosition pathGridPosition in pathGridPositionList)
        {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }

        OnStartMoving?.Invoke(this, positionList.Count);

        GridPosition oldUnitGridPosition = unit.GetGridPosition();
        unit.SetGridPosition(gridPosition);
        LevelGrid.Instance.UnitMovedGridPosition(unit, oldUnitGridPosition, gridPosition);

        OnAnyUnitMoved?.Invoke(this, gridPosition);

        //Remove Terrain effect from previous gridobject
        ITerrainEffect oldTerrainEffect = LevelGrid.Instance.GetTerrainEffectAtGridPosition(
            oldUnitGridPosition
        );
        oldTerrainEffect?.RemoveEffect(unit);

        //Apply terrain effect from new gridobject
        ITerrainEffect newTerrainEffect = LevelGrid.Instance.GetTerrainEffectAtGridPosition(
            gridPosition
        );
        newTerrainEffect?.ApplyEffect(unit);

        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidActionGridPositionList(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList();
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }
                if (
                    LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)
                    && (unitGridPosition != testGridPosition)
                )
                {
                    // Grid Position already occupied with another Unit
                    continue;
                }
                if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                {
                    continue;
                }
                if (!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition))
                {
                    continue;
                }
                int pathfindingDistanceMultiplier = 10;
                if (
                    Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition)
                    > maxMoveDistance * pathfindingDistanceMultiplier
                )
                {
                    // Path length is too long
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    private int GetMoveLocationValue(GridPosition gridPosition)
    {
        List<Unit> playerUnitList = UnitManager.Instance.GetFriendlyUnitList();
        Unit closestPlayerUnit = null;
        int closestPlayerDistance = 0;
        foreach (Unit playerUnit in playerUnitList)
        {
            if (closestPlayerUnit == null)
            {
                closestPlayerUnit = playerUnit;
                Pathfinding.Instance.FindPath(
                    unit.GetGridPosition(),
                    playerUnit.GetGridPosition(),
                    out closestPlayerDistance
                );
                continue;
            }

            Pathfinding.Instance.FindPath(
                unit.GetGridPosition(),
                playerUnit.GetGridPosition(),
                out int distanceToUnit
            );

            if (distanceToUnit < closestPlayerDistance)
            {
                closestPlayerUnit = playerUnit;
                closestPlayerDistance = distanceToUnit;
            }
        }

        if (closestPlayerUnit == null)
        {
            return 0;
        }

        if (
            Pathfinding.Instance.FindPath(
                gridPosition,
                closestPlayerUnit.GetGridPosition(),
                out int distanceFromGridPosition
            ) == null
        )
        {
            return -1;
        }

        int distanceMovedToClosestUnit = closestPlayerDistance - distanceFromGridPosition;
        if (distanceMovedToClosestUnit < 0)
        {
            distanceMovedToClosestUnit = 0;
        }
        return distanceMovedToClosestUnit;
    }

    //Needs to enumerate through actions to get best enemy ai action before moving
    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int targetCountAtGridPosition;
        BaseAction unitAction;

        // NEEDS IMPROVING ---------------------------------------------------------------------------------------------------

        if (unit.GetAction<AttackAction>())
        {
            unitAction = unit.GetAction<AttackAction>();
        }
        else if (unit.GetAction<FireballAction>())
        {
            unitAction = unit.GetAction<FireballAction>();
        }
        else if (unit.GetAction<CleaveAction>())
        {
            unitAction = unit.GetAction<CleaveAction>();
        }
        else
        {
            return new EnemyAIAction { gridPosition = gridPosition, actionValue = 0, };
        }

        targetCountAtGridPosition = unitAction.GetTargetCountAtPosition(gridPosition);

        if (targetCountAtGridPosition == 0)
        {
            int moveActionValue = GetMoveLocationValue(gridPosition);
            if (moveActionValue == -1)
            {
                gridPosition = unit.GetGridPosition();
            }
            return new EnemyAIAction
            {
                gridPosition = gridPosition,
                actionValue = moveActionValue,
            };
        }

        GridPosition gridToTarget = gridPosition - unit.GetGridPosition();
        int distanceToTarget = Mathf.Abs(gridToTarget.x) + Mathf.Abs(gridToTarget.z);
        //int distanceActionValueModifier = Mathf.RoundToInt((1f / distanceToTarget) * 100f);

        List<GridPosition> targetPositions = unitAction.GetValidActionGridPositionList(
            gridPosition
        );
        int targetActionValueTotal = 0;
        foreach (GridPosition targetPosition in targetPositions)
        {
            targetActionValueTotal += unitAction.GetEnemyAIAction(targetPosition).actionValue;
        }

        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = targetActionValueTotal //+ distanceActionValueModifier,
        };
    }

    public override int GetActionRange()
    {
        return maxMoveDistance;
    }
}
