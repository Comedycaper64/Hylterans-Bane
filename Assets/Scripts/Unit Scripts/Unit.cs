using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{ 
    [SerializeField] private String unitName;

    //Action points unit gets each turn
    [SerializeField] private int actionPointMax;

    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler<GridPosition> OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;
    public static event EventHandler OnTaunted;
    public static event EventHandler OnTauntRemoved;


    //Toggles unit to have enemyBehaviour
    [SerializeField] private bool isEnemy;

    private GridPosition gridPosition;
    private HealthSystem healthSystem;
    private BaseAction[] baseActionArray;
    private int actionPoints;

    [SerializeField] private GameObject skeletonBones;

    [SerializeField] private GameObject backSpriteAttacking;
    [SerializeField] private GameObject backSpriteDead;

    private Unit focusTargetUnit;

    private void Awake() 
    {
        healthSystem = GetComponent<HealthSystem>();
        //Puts each component that extends the BaseAction into the array
        baseActionArray = GetComponents<BaseAction>();
        actionPoints = actionPointMax;

    }

    //Subscribes to events, puts the Unit on the LevelGrid
    private void Start() 
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        transform.position = LevelGrid.Instance.GetWorldPosition(gridPosition);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);    
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        healthSystem.OnDead += HealthSystem_OnDead;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
        RemoveSummonIndicators();
        OnAnyUnitSpawned?.Invoke(this, gridPosition);
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
    public T GetAction<T>() where T : BaseAction
    {
         foreach (BaseAction baseAction in baseActionArray)
        {
            if (baseAction is T)
            {
                return (T)baseAction;
            }
        }
        return null;

    }

    private void RemoveSummonIndicators()
    {
        Collider[] colliderArray = Physics.OverlapSphere(LevelGrid.Instance.GetWorldPosition(gridPosition), 1f);
        foreach (Collider collider in colliderArray)
        {
            if (collider.gameObject.tag == "Summon")
            {
                Destroy(collider.gameObject);
            }
        }
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    //Returns all actions on Unit
    public BaseAction[] GetBaseActionArray()
    {
        return baseActionArray;
    }


    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    //Spends action points and returns true if has enough AP to perform action, returns false if not
    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (CanSpendActionPointsToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointsCost());
            return true;
        }
        else
        {
            return false;
        }
    }

    //Helper for above^
    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (actionPoints >= baseAction.GetActionPointsCost())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Decreases from actionPoints, fires off ActionsPointsChanged event
    private void SpendActionPoints(int amount)
    {
        actionPoints -= amount;
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        if (actionPoints < 0)
        {
            Debug.LogError("Action points for " + gameObject + "are lower than 0!");
        }
    }

    public void RefundActionPoints(int amount)
    {
        actionPoints += amount;
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetActionPoints()
    {
        return actionPoints;
    }

    //Regens action points for Player units if player unit turn, for enemy units instead if enemy turn
    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if ((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn()) || (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn()))
        {
            actionPoints = actionPointMax;
            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    //Removes unit from Grid when dead, invoked UnitDead event
    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
        GridObject gridObject = LevelGrid.Instance.GetGridObject(gridPosition);
        // if (!gridObject.HasBones() && skeletonBones)
        // {
        //     BonePile bonePile = Instantiate(skeletonBones, transform.position, Quaternion.identity).GetComponent<BonePile>();
        //     gridObject.SetBonePile(bonePile);
        // }
        // gridObject.AddBones();
        float deathDelayTimer = 1.5f;
        if (backSpriteAttacking)
            backSpriteAttacking.SetActive(false);
        if (backSpriteDead)
            backSpriteDead.SetActive(true);
        if (this)
            Destroy(gameObject, deathDelayTimer);
        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;
        if (unit == GetFocusTargetUnit())
        {
            RemoveTaunt();
        }
    }

    
    public bool IsEnemy()
    {
        return isEnemy;
    }

    public void TauntUnit(Unit targetUnit)
    {
        focusTargetUnit = targetUnit;
        OnTaunted?.Invoke(this, EventArgs.Empty);
    }
    
    public void RemoveTaunt()
    {
        focusTargetUnit = null;
        OnTauntRemoved?.Invoke(this, EventArgs.Empty);
    }

    public bool HasFocusTargetUnit()
    {
        return focusTargetUnit;
    }

    public Unit GetFocusTargetUnit()
    {
        return focusTargetUnit;
    }

    public void Damage(int damageAmount)
    {
        healthSystem.Damage(damageAmount);
    }
    
    public float GetHealthNormalized()
    {
        return healthSystem.GetHealthNormalized();
    }

    public String GetUnitName()
    {
        return unitName;
    }

}
