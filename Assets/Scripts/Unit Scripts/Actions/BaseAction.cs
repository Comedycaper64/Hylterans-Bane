using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//An abstract class can't go directly on an object, it needs to have an extension of it do so instead
public abstract class BaseAction : MonoBehaviour
{
    public static event EventHandler OnAnyActionStarted;
    public static event EventHandler OnAnyActionCompleted;

    // Scripts that extend BaseAction (the other actions) can access the protected fields
    protected Unit unit;
    protected bool isActive;
    protected Action onActionComplete;

    protected virtual void Awake()
    {
        unit = GetComponent<Unit>();
    }

    //Abstract methods must be implemented by extenders
    public abstract string GetActionName();

    public virtual bool GetIsAOE()
    {
        return false;
    }

    public virtual float GetDamage()
    {
        return 0;
    }

    public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);

    //Base method for each action to check if a GridPosition is in their own constructed GetValidActionGridPositionList()
    public virtual bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositionList = GetValidActionGridPositionList();
        return validGridPositionList.Contains(gridPosition);
    }

    public abstract List<GridPosition> GetValidActionGridPositionList();

    //Base method that can be overwritten to return a different value, like 2 or whatever
    public virtual int GetActionPointsCost()
    {
        return 1;
    }

    //Steps that each action has to do, so they're collated here into methods
    protected void ActionStart(Action onActionComplete)
    {
        isActive = true;
        this.onActionComplete = onActionComplete;
        OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
    }

    protected void ActionComplete()
    {
        isActive = false;
        if (onActionComplete != null)
            onActionComplete();
        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetUnit()
    {
        return unit;
    }

    //Idk why it's in baseaction, but this finds the best action that an enemy unit can take and returns it
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
