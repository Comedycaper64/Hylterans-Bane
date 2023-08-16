using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombsAwayAction : BaseAction
{
    private string actionDescription = "An explosive shell launched at high velocity";

    [SerializeField]
    private Transform bombProjectilePrefab;

    // [SerializeField]
    // private AudioClip fireballChargeSFX;

    private int minThrowDistance = 4;
    private int maxThrowDistance = 8;

    [SerializeField]
    private StatBonus actionStatBonus = new StatBonus(0, 2, 0);

    // [SerializeField]
    // private float damageRadius = 3f;

    private State state;
    private float stateTimer;
    private GridPosition targetGridPosition;

    private enum State
    {
        Charging,
        Throwing,
        Cooloff,
    }

    public override string GetActionName()
    {
        return "Bombs Away!";
    }

    public override string GetActionDescription()
    {
        return actionDescription;
    }

    public override (int, int) GetDamageArea()
    {
        return (3, 3);
    }

    public override int GetRequiredHeldActions()
    {
        return 3;
    }

    public override int GetActionRange()
    {
        return maxThrowDistance;
    }

    public override bool GetIsAOE()
    {
        return true;
    }

    public override AOEType GetAOEType()
    {
        return AOEType.Sphere;
    }

    public override bool ActionDealsDamage()
    {
        return true;
    }

    public override StatBonus GetStatBonus()
    {
        return actionStatBonus;
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        stateTimer -= Time.deltaTime;
        //Moves between the states at the speed of the stateTimer
        switch (state)
        {
            case State.Charging:

                break;
            //Fires the shot
            case State.Throwing:
                Transform fireballProjectileTransform = Instantiate(
                    bombProjectilePrefab,
                    unit.GetWorldPosition(),
                    Quaternion.identity
                );
                AbilityProjectile fireballProjectile =
                    fireballProjectileTransform.GetComponent<AbilityProjectile>();
                int damageAmount = Mathf.RoundToInt(unit.GetUnitStats().GetDamage());
                fireballProjectile.Setup(
                    targetGridPosition,
                    damageAmount,
                    GetDamageArea().Item1,
                    false,
                    SpellSave(),
                    unit.GetUnitStats(),
                    OnFireballBehaviourComplete
                );
                break;
            //Grace period before ending the action
            case State.Cooloff:
                break;
        }

        if (stateTimer <= 0f)
        {
            NextState();
        }
    }

    private void NextState()
    {
        //Changes state + timer between each state
        switch (state)
        {
            case State.Charging:
                state = State.Throwing;
                float shootingStateTime = 0f;
                stateTimer = shootingStateTime;
                break;
            case State.Throwing:
                state = State.Cooloff;
                float coolOffStateTime = 1f;
                stateTimer = coolOffStateTime;
                break;
            case State.Cooloff:
                break;
        }
    }

    public override List<GridPosition> GetValidActionGridPositionList(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList();
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxThrowDistance; x <= maxThrowDistance; x++)
        {
            for (int z = -maxThrowDistance; z <= maxThrowDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                if (unitGridPosition == testGridPosition)
                {
                    // Same Grid Position where the unit is already at
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if ((testDistance > maxThrowDistance) || (testDistance < minThrowDistance))
                {
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        // AudioSource.PlayClipAtPoint(
        //     fireballChargeSFX,
        //     Camera.main.transform.position,
        //     SoundManager.Instance.GetSoundEffectVolume()
        // );
        targetGridPosition = gridPosition;
        state = State.Charging;
        float aimingStateTime = 1f;
        stateTimer = aimingStateTime;

        ActionStart(onActionComplete);
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int targetsInAOE = 0;
        for (int x = gridPosition.x - 1; x <= gridPosition.x + 1; x++)
        {
            for (int z = gridPosition.z - 1; z <= gridPosition.z + 1; z++)
            {
                GridPosition testGridPosition = new GridPosition(x, z);
                if (
                    LevelGrid.Instance.IsValidGridPosition(testGridPosition)
                    && LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)
                )
                {
                    if (!LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition).IsEnemy())
                    {
                        targetsInAOE++;
                    }
                }
            }
        }
        return new EnemyAIAction { gridPosition = gridPosition, actionValue = targetsInAOE * 100, };
    }

    private void OnFireballBehaviourComplete()
    {
        ActionComplete();
    }
}
