using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public static EnemyAI Instance { get; private set; }
    public event EventHandler<Unit> OnEnemyUnitBeginAction;
    public event EventHandler OnEnemyTurnFinished;

    private Unit currentEnemyUnit;

    //Logic for doing the enemy's turn
    private enum State
    {
        WaitingForEnemyTurn,
        TakingTurn,
        Busy,
    }

    //private State state;

    //private float timer;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one EnemyAI! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //state = State.WaitingForEnemyTurn;
    }

    public bool IsEnemyAIActive()
    {
        return (currentEnemyUnit != null);
    }

    // private void Start()
    // {
    //     TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    // }

    // private void OnDisable()
    // {
    //     TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChanged;
    // }

    //Goes through state machine in similar manner to Shootaction
    // private void Update()
    // {
    //     if (TurnSystem.Instance.IsPlayerTurn())
    //     {
    //         return;
    //     }

    //     switch (state)
    //     {
    //         case State.WaitingForEnemyTurn:
    //             break;
    //         case State.TakingTurn:
    //             timer -= Time.deltaTime;
    //             if (timer <= 0f)
    //             {
    //                 if (TryTakeEnemyAIAction(SetStateTakingTurn))
    //                 {
    //                     state = State.Busy;
    //                 }
    //                 else
    //                 {
    //                     // No more enemies have actions they can take, end enemy turn
    //                     OnEnemyTurnFinished?.Invoke(this, EventArgs.Empty);
    //                     TurnSystem.Instance.NextTurn();
    //                 }
    //             }
    //             break;
    //         case State.Busy:
    //             break;
    //     }
    // }

    // private void SetStateTakingTurn()
    // {
    //     timer = 0.25f;
    //     state = State.TakingTurn;
    // }

    private void FinishEnemyTurn()
    {
        if (!currentEnemyUnit.GetActionCompleted())
        {
            TryTakeEnemyAIAction(currentEnemyUnit, FinishEnemyTurn);
        }
        else
        {
            currentEnemyUnit = null;
            OnEnemyTurnFinished?.Invoke(this, EventArgs.Empty);
        }
    }

    //Resets state machine when it's the enemy's turn
    // private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    // {
    //     if (!TurnSystem.Instance.IsPlayerTurn())
    //     {
    //         state = State.TakingTurn;
    //         timer = 1f;
    //     }
    // }

    public void TakeEnemyTurn(Unit enemyUnit)
    {
        OnEnemyUnitBeginAction?.Invoke(this, enemyUnit);
        currentEnemyUnit = enemyUnit;
        TryTakeEnemyAIAction(enemyUnit, FinishEnemyTurn);
    }

    //Each enemy unit tries to take as many actions as it can
    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
    {
        foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList())
        {
            OnEnemyUnitBeginAction?.Invoke(this, enemyUnit);
            if (TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete))
            {
                return true;
            }
        }

        return false;
    }

    //Does as many actions as it can based on AP, goes through all possible actions and does the one with the best action value
    //Value calculation for an action is done in the actions themselves (hence baseAction is being used)
    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)
    {
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestBaseAction = null;

        // Debug.Log(
        //     "Enemy Moved: "
        //         + enemyUnit.GetMovementCompleted()
        //         + ", Enemy Acted: "
        //         + enemyUnit.GetActionCompleted()
        // );

        if (enemyUnit.GetActionCompleted() && enemyUnit.GetMovementCompleted())
        {
            return false;
        }

        if (enemyUnit.GetMovementCompleted())
        {
            foreach (BaseAction baseAction in enemyUnit.GetBaseActionList())
            {
                if (baseAction == enemyUnit.GetAction<MoveAction>())
                {
                    continue;
                }

                if (bestEnemyAIAction == null)
                {
                    bestEnemyAIAction = baseAction.GetBestEnemyAIAction();
                    bestBaseAction = baseAction;
                }
                else
                {
                    EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
                    if (
                        testEnemyAIAction != null
                        && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue
                    )
                    {
                        bestEnemyAIAction = testEnemyAIAction;
                        bestBaseAction = baseAction;
                    }
                }
            }
            enemyUnit.SetActionCompleted(true);
        }
        else
        {
            BaseAction actionToDo = enemyUnit.GetAction<MoveAction>();

            bestEnemyAIAction = actionToDo.GetBestEnemyAIAction();
            bestBaseAction = actionToDo;

            enemyUnit.SetMovementCompleted(true);
        }

        if (bestEnemyAIAction != null)
        {
            bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);
            return true;
        }
        else
        {
            return false;
        }
    }
}
