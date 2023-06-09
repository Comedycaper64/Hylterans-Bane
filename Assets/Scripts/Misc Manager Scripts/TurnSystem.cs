using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    //Instanced because there should only be one
    public static TurnSystem Instance { get; private set; }

    [SerializeField]
    private AudioClip turnButtonPressedSFX;

    public event EventHandler OnTurnChanged;

    public event Action OnNewTurn;

    //Keeps track of what turn it is
    private int turnNumber = 1;
    private bool isPlayerTurn;

    private int initiativeTracker;
    private List<Initiative> initiativeOrder = new List<Initiative>();

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

        GetNewInitiativeRound();
        OnNewTurn?.Invoke();
    }

    private void OnDisable()
    {
        EnemyAI.Instance.OnEnemyTurnFinished -= EnemyAI_OnEnemyTurnFinished;
    }

    public void NextInitiative()
    {
        initiativeTracker++;

        if (initiativeTracker >= initiativeOrder.Count)
        {
            NextTurn();
            return;
        }

        Unit nextUnit = initiativeOrder[initiativeTracker].unit;
        isPlayerTurn = !nextUnit.IsEnemy();
        OnTurnChanged?.Invoke(this, EventArgs.Empty);
        nextUnit.SetMovementCompleted(false);
        nextUnit.SetActionCompleted(false);
        if (!isPlayerTurn)
        {
            EnemyAI.Instance.TakeEnemyTurn(nextUnit);
        }
    }

    //Advances turn, fires off OnTurnChanged event
    public void NextTurn()
    {
        turnNumber++;

        AudioSource.PlayClipAtPoint(
            turnButtonPressedSFX,
            Camera.main.transform.position,
            SoundManager.Instance.GetSoundEffectVolume()
        );
        OnNewTurn?.Invoke();
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
        initiativeTracker = -1;
        initiativeOrder.Clear();
        List<Unit> unitList = UnitManager.Instance.GetUnitList();
        foreach (Unit unit in unitList)
        {
            Initiative newInitiative = new Initiative(unit, unit.GetUnitStats().GetInitiative());
            initiativeOrder.Add(newInitiative);
        }
        initiativeOrder.Sort((Initiative a, Initiative b) => b.unitInitiative - a.unitInitiative);
        NextInitiative();
    }

    private void EnemyAI_OnEnemyTurnFinished(object sender, EventArgs e)
    {
        NextInitiative();
    }
}
