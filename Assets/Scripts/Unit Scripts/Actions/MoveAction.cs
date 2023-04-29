using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{

    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;
    [SerializeField] private int maxMoveDistance = 4;

    private List<Vector3> positionList;
    private int currentPositionIndex;

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        Vector3 targetPosition = positionList[currentPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        float rotateSpeed = 10f;
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

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
        List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);

        currentPositionIndex = 0;
        positionList = new List<Vector3>();

        foreach (GridPosition pathGridPosition in pathGridPositionList)
        {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }

        OnStartMoving?.Invoke(this, EventArgs.Empty);

        ActionStart(onActionComplete);
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

                if (unitGridPosition == testGridPosition)
                {
                    // Same Grid Position where the unit is already at
                    continue;
                }
                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
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
                if (Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition) > maxMoveDistance * pathfindingDistanceMultiplier)
                {
                    // Path length is too long
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }


    public override string GetActionName()
    {
        return "Move";
    }

    private int GetMoveLocationValue(GridPosition gridPosition)
    {
        List<Unit> playerUnitList = UnitManager.Instance.GetFriendlyUnitList();
        Unit closestPlayerUnit = null;
        int closestPlayerDistance = 0;
        foreach(Unit playerUnit in playerUnitList)
        {
            // if (unit.HasFocusTargetUnit())
            // {
            //     if (unit.GetFocusTargetUnit() != playerUnit)
            //         continue;
            // }

            if (closestPlayerUnit == null)
            {
                closestPlayerUnit = playerUnit;
                Pathfinding.Instance.FindPath(unit.GetGridPosition(), playerUnit.GetGridPosition(), out closestPlayerDistance);
                continue;
            }

            Pathfinding.Instance.FindPath(unit.GetGridPosition(), playerUnit.GetGridPosition(), out int distanceToUnit);

            if (distanceToUnit < closestPlayerDistance)
            {
                closestPlayerUnit = playerUnit;
                closestPlayerDistance = distanceToUnit;
            }
        }

        if (closestPlayerUnit == null) {return 0;}

        if (Pathfinding.Instance.FindPath(gridPosition, closestPlayerUnit.GetGridPosition(), out int distanceFromGridPosition) == null)
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

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int targetCountAtGridPosition;
        if (unit.GetAction<ShootAction>())
        {
            /*
            if (!Pathfinding.Instance.HasPath(unit.GetGridPosition(), gridPosition))
            {
                return new EnemyAIAction
                {
                    gridPosition = gridPosition,
                    actionValue = 50,
                };
            }
            */

            targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);

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

            return new EnemyAIAction
            {
                gridPosition = gridPosition,
                actionValue = targetCountAtGridPosition * 10,
            };
        }
        else if (unit.GetAction<SwordAction>())
        {
            targetCountAtGridPosition = unit.GetAction<SwordAction>().GetTargetCountAtPosition(gridPosition);

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

            return new EnemyAIAction
            {
                gridPosition = gridPosition,
                actionValue = targetCountAtGridPosition * 10,
            };
        }
        else
        {
            return new EnemyAIAction
            {
                gridPosition = gridPosition,
                actionValue = 0,
            };
        }
        
    }

    public int GetMaxMoveDistance()
    {
        return maxMoveDistance;
    }
    
}
