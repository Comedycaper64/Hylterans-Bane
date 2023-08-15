using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    public event EventHandler OnActionStarted;
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
