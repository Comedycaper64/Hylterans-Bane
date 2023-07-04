using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballAction : BaseAction
{
    [SerializeField]
    private Transform fireballProjectilePrefab;

    [SerializeField]
    private AudioClip fireballChargeSFX;

    public event EventHandler OnFireballActionStarted;
    public event EventHandler OnFireballActionCompleted;

    [SerializeField]
    private int maxThrowDistance = 4;

    [SerializeField]
    private float actionDamageMultiplier = 0.8f;

    [SerializeField]
    private float damageRadius = 3f;

    //private int minThrowDistance = 2;
    private State state;
    private float stateTimer;
    private GridPosition targetGridPosition;

    private enum State
    {
        Charging,
        Throwing,
        Cooloff,
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
                    fireballProjectilePrefab,
                    unit.GetWorldPosition(),
                    Quaternion.identity
                );
                FireballProjectile fireballProjectile =
                    fireballProjectileTransform.GetComponent<FireballProjectile>();
                int damageAmount = Mathf.RoundToInt(
                    unit.GetUnitStats().GetDamage() * actionDamageMultiplier
                );
                fireballProjectile.Setup(
                    targetGridPosition,
                    damageAmount,
                    damageRadius,
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

    public override string GetActionName()
    {
        return "Fireball";
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
                if (testDistance > maxThrowDistance)
                {
                    continue;
                }
                // if (testDistance < minThrowDistance)
                // {
                //     continue;
                // }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        AudioSource.PlayClipAtPoint(
            fireballChargeSFX,
            Camera.main.transform.position,
            SoundManager.Instance.GetSoundEffectVolume()
        );
        OnFireballActionStarted?.Invoke(this, EventArgs.Empty);
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

    public override int GetActionRange()
    {
        return maxThrowDistance;
    }

    public override bool GetIsAOE()
    {
        return true;
    }

    public override bool ActionDealsDamage()
    {
        return true;
    }

    public override int GetDamage()
    {
        return Mathf.RoundToInt(unit.GetUnitStats().GetDamage() * actionDamageMultiplier);
    }

    private void OnFireballBehaviourComplete()
    {
        OnFireballActionCompleted?.Invoke(this, EventArgs.Empty);
        ActionComplete();
    }
}
