using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    public event EventHandler OnActionStarted;
    public event EventHandler<Unit> OnUnitHit;
    public static event EventHandler<float> OnAnyAttackHit;

    // Scripts that extend BaseAction (the other actions) can access the protected fields
    protected Unit unit;
    protected bool isActive;
    protected Action onActionComplete;

    protected virtual void Awake()
    {
        unit = GetComponent<Unit>();
    }

    public virtual bool GetIsAOE()
    {
        return false;
    }

    public virtual bool IsSpell()
    {
        return false;
    }

    public virtual StatType SpellSave()
    {
        return new StatType();
    }

    public virtual (int, int) GetDamageArea()
    {
        return (1, 1);
    }

    public virtual AOEType GetAOEType()
    {
        return AOEType.Cube;
    }

    public virtual bool ActionDealsDamage()
    {
        return false;
    }

    public virtual StatBonus GetStatBonus()
    {
        return new StatBonus();
    }

    public virtual int GetUIPriority()
    {
        return 0;
    }

    public virtual int GetActionRange()
    {
        return 0;
    }

    public virtual int GetRequiredHeldActions()
    {
        return 0;
    }

    public virtual int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return 0;
    }

    public abstract string GetActionName();

    public abstract string GetActionDescription();

    public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);

    public virtual bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositionList = GetValidActionGridPositionList();
        return validGridPositionList.Contains(gridPosition);
    }

    public abstract List<GridPosition> GetValidActionGridPositionList();

    public abstract List<GridPosition> GetValidActionGridPositionList(GridPosition gridPosition);

    protected void ActionStart(Action onActionComplete)
    {
        isActive = true;
        this.onActionComplete = onActionComplete;
        OnActionStarted?.Invoke(this, EventArgs.Empty);
    }

    protected void ActionComplete()
    {
        isActive = false;
        if (onActionComplete != null)
        {
            onActionComplete();
        }
    }

    protected void AttackHit(float damageDealt)
    {
        OnAnyAttackHit?.Invoke(this, damageDealt);
    }

    protected void UnitHit(Unit hitUnit)
    {
        OnUnitHit?.Invoke(this, hitUnit);
    }

    //For Cubes range = width
    //For Spheres range = radius
    protected List<Unit> GetUnitsInAOE(
        GridPosition aoeCentre,
        (int, int) aoeRange,
        AOEType aOEType = AOEType.Cube,
        bool enemyUnits = true
    )
    {
        List<Unit> unitsInAoe = new List<Unit>();
        if (unit.IsEnemy())
        {
            enemyUnits = !enemyUnits;
        }

        switch (aOEType)
        {
            default:
            case AOEType.Cube:

                aoeRange = (
                    Mathf.RoundToInt((aoeRange.Item1 - 1) / 2),
                    Mathf.RoundToInt((aoeRange.Item2 - 1) / 2)
                );

                for (int x = -aoeRange.Item1; x <= aoeRange.Item1; x++)
                {
                    for (int z = -aoeRange.Item2; z <= aoeRange.Item2; z++)
                    {
                        GridPosition testGridPosition = aoeCentre + new GridPosition(x, z);
                        if (
                            LevelGrid.Instance.TryGetUnitAtGridPosition(
                                testGridPosition,
                                out Unit targetUnit
                            ) && (targetUnit.IsEnemy() == enemyUnits)
                        )
                        {
                            unitsInAoe.Add(targetUnit);
                        }
                    }
                }
                break;
            case AOEType.Sphere:
                for (int x = -aoeRange.Item1; x <= aoeRange.Item1; x++)
                {
                    for (int z = -aoeRange.Item1; z <= aoeRange.Item1; z++)
                    {
                        int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                        if (testDistance > aoeRange.Item1)
                        {
                            continue;
                        }

                        GridPosition testGridPosition = aoeCentre + new GridPosition(x, z);
                        if (
                            LevelGrid.Instance.TryGetUnitAtGridPosition(
                                testGridPosition,
                                out Unit targetUnit
                            ) && (targetUnit.IsEnemy() == enemyUnits)
                        )
                        {
                            unitsInAoe.Add(targetUnit);
                        }
                    }
                }
                break;
            case AOEType.Line:
                GridPosition distanceFromAttacker = aoeCentre - unit.GetGridPosition();
                aoeCentre = unit.GetGridPosition() + (distanceFromAttacker * (aoeRange.Item2 / 2));

                aoeRange = (
                    Mathf.RoundToInt((aoeRange.Item1 - 1) / 2),
                    Mathf.RoundToInt((aoeRange.Item2 - 1) / 2)
                );

                if (distanceFromAttacker.z == 0)
                {
                    (aoeRange.Item1, aoeRange.Item2) = (aoeRange.Item2, aoeRange.Item1);
                }

                for (int x = -aoeRange.Item1; x <= aoeRange.Item1; x++)
                {
                    for (int z = -aoeRange.Item2; z <= aoeRange.Item2; z++)
                    {
                        GridPosition offsetGridPosition = new GridPosition(x, z);
                        GridPosition testGridPosition = aoeCentre + offsetGridPosition;

                        if (
                            LevelGrid.Instance.TryGetUnitAtGridPosition(
                                testGridPosition,
                                out Unit targetUnit
                            ) && (targetUnit.IsEnemy() == enemyUnits)
                        )
                        {
                            unitsInAoe.Add(targetUnit);
                        }
                    }
                }
                break;
            case AOEType.Cone:
                Debug.Log("Not implemented yet");
                break;
        }
        return unitsInAoe;
    }

    public Unit GetUnit()
    {
        return unit;
    }

    //finds the best action that an enemy unit can take and returns it
    public EnemyAIAction GetBestEnemyAIAction()
    {
        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>();

        List<GridPosition> validActionGridPositionList = GetValidActionGridPositionList();

        foreach (GridPosition gridPosition in validActionGridPositionList)
        {
            EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPosition);
            enemyAIActionList.Add(enemyAIAction);
        }

        //If the enemy is capable of taking an action then it finds the action with the highest action value and returns it
        if (enemyAIActionList.Count > 0)
        {
            enemyAIActionList.Sort(
                (EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue
            );
            // foreach (EnemyAIAction action in enemyAIActionList)
            // {
            //     Debug.Log(
            //         "Enemy Action Position: "
            //             + action.gridPosition
            //             + ", Enemy Action Value: "
            //             + action.actionValue
            //     );
            // }

            return enemyAIActionList[0];
        }
        else
        {
            // No possible Enemy AI Actions
            return null;
        }
    }

    public abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition);
}
