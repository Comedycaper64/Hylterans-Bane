using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonSkeletonAction : BaseAction
{
    public static event EventHandler<GridPosition> OnAnySummonStart;

    [SerializeField] private int maxSummonDistance = 5;
    [SerializeField] private GameObject basicSkeleton;
    [SerializeField] private GameObject summonIndicator;
    [SerializeField] private AudioClip skeletonSummoned;
    
    private void Update() 
    {
        if (!isActive)
        {
            return;
        }
        ActionComplete();
    }

    public override string GetActionName()
    {
        return "Summon Skeleton";
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

        for (int x = -maxSummonDistance; x <= maxSummonDistance; x++)
        {
            for (int z = -maxSummonDistance; z <= maxSummonDistance; z++)
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

                int pathfindingDistanceMultiplier = 10;
                if (Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition) > maxSummonDistance * pathfindingDistanceMultiplier)
                {
                    // Path length is too long
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        //Unit newUnit = Instantiate(basicSkeleton, new Vector3(gridPosition.x * LevelGrid.Instance.GetCellSize(), 0 , gridPosition.z * LevelGrid.Instance.GetCellSize()), Quaternion.identity).GetComponent<Unit>();
        //SummonBuffer.Instance.AddToSummonBuffer(basicSkeleton, gridPosition);
        Instantiate(summonIndicator, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);        
        AudioSource.PlayClipAtPoint(skeletonSummoned, Camera.main.transform.position, SoundManager.Instance.GetSoundEffectVolume());
        OnAnySummonStart?.Invoke(this, gridPosition);
        ActionStart(onActionComplete);
    }

    public override int GetActionPointsCost()
    {
        return 1;
    }
}
