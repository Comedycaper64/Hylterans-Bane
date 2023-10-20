using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField]
    private String unitName;

    [SerializeField]
    private String unitClass;

    private UnitStats unitStats;

    //Toggles unit to have enemyBehaviour
    [SerializeField]
    private bool isEnemy;
    private bool turnMovementCompleted;
    private bool turnActionCompleted;

    private int spirit;
    private int maxSpirit = 3;
    private int minSpirit = -3;

    private GridPosition gridPosition;
    private HealthSystem healthSystem;
    private List<BaseAction> baseActionList = new List<BaseAction>();
    private List<PassiveAbility> passiveAbilityList = new List<PassiveAbility>();

    //SERIALIZABLES

    // [SerializeField]
    // private GameObject backSpriteAttacking;

    // [SerializeField]
    // private GameObject backSpriteDead;

    [SerializeField]
    private MeshRenderer baseMesh;

    // [SerializeField]
    // private Material availableMaterial;

    // [SerializeField]
    // private Material usedMaterial;

    [SerializeField]
    private Sprite unitInitiativeUI;

    //EVENTS
    public event EventHandler<int> OnSpiritChanged;
    public event EventHandler<AttackInteraction> OnAOEAttack;
    public static event EventHandler<GridPosition> OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;
    public Action OnUnitTurnStart;
    public Action OnUnitTurnEnd;

    private void Awake()
    {
        spirit = 0;
        healthSystem = GetComponent<HealthSystem>();
        unitStats = GetComponent<UnitStats>();
        //Puts each component that extends the BaseAction into the array
        BaseAction[] baseActionArray = GetComponents<BaseAction>();
        foreach (BaseAction baseAction in baseActionArray)
        {
            if (baseAction.GetActionUnlockLevel() <= unitStats.GetLevel())
            {
                baseAction.SetDisabled(false);
                baseActionList.Add(baseAction);
            }
            else
            {
                baseAction.SetDisabled(true);
            }
        }
        baseActionList.Sort((BaseAction a, BaseAction b) => b.GetUIPriority() - a.GetUIPriority());

        PassiveAbility[] passiveAbilities = GetComponents<PassiveAbility>();
        foreach (PassiveAbility passiveAbility in passiveAbilities)
        {
            if (passiveAbility.GetAbilityUnlockLevel() <= unitStats.GetLevel())
            {
                passiveAbility.SetIsDisabled(false);
                passiveAbilityList.Add(passiveAbility);
            }
            else
            {
                passiveAbility.SetIsDisabled(true);
            }
        }

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
            if (baseAction is T t)
            {
                return t;
            }
        }
        return null;
    }

    public T GetAbility<T>()
        where T : PassiveAbility
    {
        foreach (PassiveAbility passiveAbility in passiveAbilityList)
        {
            if (passiveAbility is T t)
            {
                return t;
            }
        }
        return null;
    }

    public void StartUnitTurn()
    {
        SetMovementCompleted(false);
        SetActionCompleted(false);
        IncreaseSpirit();
        OnUnitTurnStart?.Invoke();
    }

    public void FinishUnitTurn()
    {
        SetMovementCompleted(true);
        SetActionCompleted(true);
        OnUnitTurnEnd?.Invoke();
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

    public List<PassiveAbility> GetPassiveAbilityList()
    {
        return passiveAbilityList;
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

    public String GetUnitClass()
    {
        return unitClass;
    }

    public UnitStats GetUnitStats()
    {
        return unitStats;
    }

    public int GetHeldActions()
    {
        return spirit;
    }

    public void IncreaseSpirit()
    {
        if (spirit >= maxSpirit)
        {
            return;
        }

        spirit++;
        OnSpiritChanged?.Invoke(this, spirit);
    }

    public int GetSpirit()
    {
        return spirit;
    }

    public int GetMinSpirit()
    {
        return minSpirit;
    }

    public void UseSpirit(int numberUsed)
    {
        spirit -= numberUsed;
        OnSpiritChanged?.Invoke(this, spirit);
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
        // if (backSpriteAttacking)
        //     backSpriteAttacking.SetActive(false);
        // if (backSpriteDead)
        //     backSpriteDead.SetActive(true);
        if (this)
            Destroy(gameObject, deathDelayTimer);
        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }
}
