using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField]
    private String unitName;

    [SerializeField]
    private UnitStats unitStats;

    //Toggles unit to have enemyBehaviour
    [SerializeField]
    private bool isEnemy;
    private bool turnMovementCompleted;
    private bool turnActionCompleted;

    private GridPosition gridPosition;
    private HealthSystem healthSystem;
    private List<BaseAction> baseActionList = new List<BaseAction>();

    [SerializeField]
    private GameObject backSpriteAttacking;

    [SerializeField]
    private GameObject backSpriteDead;

    public static event EventHandler<GridPosition> OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        //Puts each component that extends the BaseAction into the array
        BaseAction[] baseActionArray = GetComponents<BaseAction>();
        foreach (BaseAction baseAction in baseActionArray)
        {
            baseActionList.Add(baseAction);
        }
        baseActionList.Sort((BaseAction a, BaseAction b) => b.GetUIPriority() - a.GetUIPriority());
    }

    //Subscribes to events, puts the Unit on the LevelGrid
    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        transform.position = LevelGrid.Instance.GetWorldPosition(gridPosition);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        healthSystem.SetHealth(unitStats.GetMaxHealth());
        healthSystem.OnDead += HealthSystem_OnDead;

        OnAnyUnitSpawned?.Invoke(this, gridPosition);
    }

    private void OnDisable()
    {
        TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChanged;
        healthSystem.OnDead -= HealthSystem_OnDead;
    }

    private void Update()
    {
        //Tracks to see if the Unit is changing position on the Grid
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            //Unit changed grid position
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newGridPosition;
            LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);
        }
    }

    //Return for each script attatched to Unit
    public T GetAction<T>()
        where T : BaseAction
    {
        foreach (BaseAction baseAction in baseActionList)
        {
            if (baseAction is T)
            {
                return (T)baseAction;
            }
        }
        return null;
    }

    public void SetActionCompleted(bool completed)
    {
        turnActionCompleted = completed;
    }

    public bool GetActionCompleted()
    {
        return turnActionCompleted;
    }

    public void SetMovementCompleted(bool completed)
    {
        turnMovementCompleted = completed;
    }

    public bool GetMovementCompleted()
    {
        return turnMovementCompleted;
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    //Returns all actions on Unit
    public List<BaseAction> GetBaseActionList()
    {
        return baseActionList;
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public void Damage(int damageAmount)
    {
        healthSystem.Damage(damageAmount);
    }

    public float GetHealth()
    {
        return healthSystem.GetHealth();
    }

    public float GetHealthNormalized()
    {
        return healthSystem.GetHealthNormalized();
    }

    public String GetUnitName()
    {
        return unitName;
    }

    public UnitStats GetUnitStats()
    {
        return unitStats;
    }

    public bool IsEnemy()
    {
        return isEnemy;
    }

    //Removes unit from Grid when dead, invoked UnitDead event
    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
        GridObject gridObject = LevelGrid.Instance.GetGridObject(gridPosition);

        float deathDelayTimer = 1.5f;
        if (backSpriteAttacking)
            backSpriteAttacking.SetActive(false);
        if (backSpriteDead)
            backSpriteDead.SetActive(true);
        if (this)
            Destroy(gameObject, deathDelayTimer);
        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }

    //Regens action points for Player units if player unit turn, for enemy units instead if enemy turn
    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (
            (IsEnemy() && !TurnSystem.Instance.IsPlayerTurn())
            || (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn())
        )
        {
            turnActionCompleted = false;
            turnMovementCompleted = false;
        }
    }
}
