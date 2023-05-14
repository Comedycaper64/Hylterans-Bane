using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction
{
    [SerializeField]
    private Transform grenadeProjectilePrefab;

    [SerializeField]
    private AudioClip fireBallChargeSFX;

    public event EventHandler OnGrenadeActionStarted;
    public event EventHandler OnGrenadeActionCompleted;

    [SerializeField]
    private int maxThrowDistance;

    [SerializeField]
    private float damageAmount = 40;

    private int minThrowDistance = 3;
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
                Transform grenadeProjectileTransform = Instantiate(
                    grenadeProjectilePrefab,
                    unit.GetWorldPosition(),
                    Quaternion.identity
                );
                //GrenadeProjectile grenadeProjectile = grenadeProjectileTransform.GetComponent<GrenadeProjectile>();
                //grenadeProjectile.Setup(targetGridPosition, damageAmount, OnGrenadeBehaviourComplete);
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
                ActionComplete();
                break;
        }
    }

    public override string GetActionName()
    {
        return "Fireball";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction { gridPosition = gridPosition, actionValue = 0, };
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
                if (testDistance < minThrowDistance)
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
        AudioSource.PlayClipAtPoint(
            fireBallChargeSFX,
            Camera.main.transform.position,
            SoundManager.Instance.GetSoundEffectVolume()
        );
        OnGrenadeActionStarted?.Invoke(this, EventArgs.Empty);
        targetGridPosition = gridPosition;
        state = State.Charging;
        float aimingStateTime = 1f;
        stateTimer = aimingStateTime;

        ActionStart(onActionComplete);
    }

    public override bool GetIsAOE()
    {
        return true;
    }

    public override bool ActionDealsDamage()
    {
        return true;
    }

    private void OnGrenadeBehaviourComplete()
    {
        OnGrenadeActionCompleted?.Invoke(this, EventArgs.Empty);
        ActionComplete();
    }
}
