using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    public static TurnSystem Instance { get; private set; }

    public event EventHandler OnTurnChanged;
    public event Action OnNextUnitInitiative;
    public event Action OnNextActionInitiative;
    public event EventHandler<Queue<Initiative>> OnNewInitiative;

    //Keeps track of what turn it is
    private int turnNumber = 1;
    private bool isPlayerTurn;

    private Queue<Initiative> initiativeOrder = new Queue<Initiative>();

    //Singleton-ed
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one TurnSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        EnemyAI.Instance.OnEnemyTurnFinished += EnemyAI_OnEnemyTurnFinished;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;

        StartCoroutine(BeginCombat());
    }

    private void OnDisable()
    {
        EnemyAI.Instance.OnEnemyTurnFinished -= EnemyAI_OnEnemyTurnFinished;
        Unit.OnAnyUnitDead -= Unit_OnAnyUnitDead;
    }

    private IEnumerator BeginCombat()
    {
        yield return new WaitForSeconds(0.5f);
        GetNewInitiativeRound();
    }

    public void NextInitiative()
    {
        if (initiativeOrder.TryDequeue(out Initiative initiative))
        {
            OnNextUnitInitiative?.Invoke();
            Initiative currentInitiative = initiative;
            if (!currentInitiative.unit)
            {
                currentInitiative.rallyingCry.PerformAbility(NextInitiative);
                return;
            }
            isPlayerTurn = !currentInitiative.unit.IsEnemy();
            if (currentInitiative.unitAction != null)
            {
                currentInitiative.unit.SetMovementCompleted(true);
                currentInitiative.unit.SetActionCompleted(false);

                if (!isPlayerTurn)
                {
                    EnemyAI.Instance.TakeEnemyAction(
                        currentInitiative.unit,
                        currentInitiative.unitAction
                    );
                }
                else
                {
                    OnNextActionInitiative?.Invoke();

                    UnitActionSystem.Instance.BeginUnitAction(
                        currentInitiative.unit,
                        currentInitiative.unitAction
                    );
                }
            }
            else
            {
                OnTurnChanged?.Invoke(this, EventArgs.Empty);
                currentInitiative.unit.SetMovementCompleted(false);
                currentInitiative.unit.SetActionCompleted(false);

                if (!isPlayerTurn)
                {
                    EnemyAI.Instance.TakeEnemyTurn(currentInitiative.unit);
                }
                else
                {
                    UnitActionSystem.Instance.BeginUnitTurn(currentInitiative.unit);
                }
            }
        }
        else
        {
            NextTurn();
        }
    }

    //Advances turn, fires off OnTurnChanged event
    public void NextTurn()
    {
        turnNumber++;
        GetNewInitiativeRound();
    }

    public int GetTurnNumber()
    {
        return turnNumber;
    }

    public bool IsPlayerTurn()
    {
        return isPlayerTurn;
    }

    private void GetNewInitiativeRound()
    {
        initiativeOrder.Clear();
        List<Unit> unitList = UnitManager.Instance.GetUnitList();
        List<Initiative> tempInitiativeList = new List<Initiative>();
        foreach (Unit unit in unitList)
        {
            Initiative newInitiative = new Initiative(unit, unit.GetUnitStats().GetInitiative());
            tempInitiativeList.Add(newInitiative);
        }
        tempInitiativeList.Sort(
            (Initiative a, Initiative b) => b.unitInitiative - a.unitInitiative
        );

        foreach (Initiative initiative in tempInitiativeList)
        {
            initiativeOrder.Enqueue(initiative);
        }

        OnNewInitiative?.Invoke(this, initiativeOrder);
        NextInitiative();
    }

    public void AddInitiativeToOrder(Initiative initiativeToAdd)
    {
        List<Initiative> tempInitiativeList = new List<Initiative>();
        tempInitiativeList.Add(initiativeToAdd);
        Debug.Log("Old Initiative Count: " + initiativeOrder.Count);

        while (initiativeOrder.TryDequeue(out Initiative currentInitiative))
        {
            tempInitiativeList.Add(currentInitiative);
        }

        initiativeOrder.Clear();
        foreach (Initiative initiative in tempInitiativeList)
        {
            initiativeOrder.Enqueue(initiative);
        }

        Debug.Log("New Initiative Count: " + initiativeOrder.Count);

        OnNewInitiative?.Invoke(this, initiativeOrder);
    }

    private void RemoveUnitFromInitiative(Unit deadUnit)
    {
        List<Initiative> tempInitiativeList = new List<Initiative>();

        while (initiativeOrder.TryDequeue(out Initiative currentInitiative))
        {
            tempInitiativeList.Add(currentInitiative);
        }

        Initiative initiativeToRemove = tempInitiativeList.Find(
            (Initiative a) => a.unit == deadUnit
        );
        if (initiativeToRemove != null)
        {
            if (!tempInitiativeList.Remove(initiativeToRemove))
            {
                Debug.Log("Unit not removed from initiative");
            }
        }

        initiativeOrder.Clear();
        foreach (Initiative initiative in tempInitiativeList)
        {
            initiativeOrder.Enqueue(initiative);
        }

        OnNewInitiative?.Invoke(this, initiativeOrder);
    }

    public void FinishAction()
    {
        UnitActionSystem.Instance.FinishCurrentUnitTurn();
        NextInitiative();
    }

    private void EnemyAI_OnEnemyTurnFinished(object sender, EventArgs e)
    {
        NextInitiative();
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;
        RemoveUnitFromInitiative(unit);
    }
}
