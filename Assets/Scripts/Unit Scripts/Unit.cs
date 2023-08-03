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

    private int heldActions;

    private GridPosition gridPosition;
    private HealthSystem healthSystem;
    private List<BaseAction> baseActionList = new List<BaseAction>();

    //SERIALIZABLES

    [SerializeField]
    private GameObject backSpriteAttacking;

    [SerializeField]
    private GameObject backSpriteDead;

    [SerializeField]
    private MeshRenderer baseMesh;

    [SerializeField]
    private Material availableMaterial;

    [SerializeField]
    private Material usedMaterial;

    [SerializeField]
    private Sprite unitInitiativeUI;

    //EVENTS
    public event EventHandler<int> OnHeldActionsChanged;
    public event EventHandler<AttackInteraction> OnAOEAttack;
    public static event EventHandler<GridPosition> OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;
    public Action OnUnitTurnStart;

    private void Awake()
    {
        heldActions = 0;
        healthSystem = GetComponent<HealthSystem>();
        //Puts each component that extends the BaseAction into the array
        BaseAction[] baseActionArray = GetComponents<BaseAction>();
        foreach (BaseAction baseAction in baseActionArray)
        {
            baseActionList.Add(baseAction);
        }
        baseActionList.Sort((BaseAction a, BaseAction b) => b.GetUIPriority() - a.GetUIPriority());

        SetActionCompleted(true);
        SetMovementCompleted(true);
    }

    //Subscribes to events, puts the Unit on the LevelGrid
    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        transform.position = LevelGrid.Instance.GetWorldPosition(gridPosition);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
        healthSystem.SetHealth(unitStats.GetMaxHealth());
        healthSystem.OnDead += HealthSystem_OnDead;

        OnAnyUnitSpawned?.Invoke(this, gridPosition);
    }

    private void OnDisable()
    {
        healthSystem.OnDead -= HealthSystem_OnDead;
    }

    public void SetGridPosition(GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
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
        if (turnActionCompleted)
        {
            baseMesh.material = usedMaterial;
        }
        else
        {
            baseMesh.material = availableMaterial;
            OnUnitTurnStart?.Invoke();
        }
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

    public int GetHeldActions()
    {
        return heldActions;
    }

    public void IncreaseHeldActions()
    {
        heldActions++;
        OnHeldActionsChanged?.Invoke(this, heldActions);
    }

    public void UseHeldActions(int numberUsed)
    {
        heldActions -= numberUsed;
        OnHeldActionsChanged?.Invoke(this, heldActions);
    }

    public void PerformAOEAttack(AttackInteraction attackInteraction)
    {
        OnAOEAttack?.Invoke(this, attackInteraction);
    }

    public bool IsEnemy()
    {
        return isEnemy;
    }

    public Sprite GetInitiativeUI()
    {
        return unitInitiativeUI;
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
}
