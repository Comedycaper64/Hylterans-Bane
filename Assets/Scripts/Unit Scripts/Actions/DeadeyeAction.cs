using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadeyeAction : BaseAction
{
    private string actionDescription =
        "A precise ranged attack bolstered by austere confidence.\n+3 to hit, +2 damage, +2 range \nHeld Actions Used : 1";

    private bool attackSucceeded;

    [SerializeField]
    private AudioClip shootCrossbowSFX;

    private enum State
    {
        Aiming,
        Shooting,
        Cooloff,
    }

    private State state;

    private float stateTimer;
    private Unit targetUnit;
    private bool canShoot;

    [SerializeField]
    private StatBonus actionStatBonus = new StatBonus(3, 2, 2);

    [SerializeField]
    private LayerMask obstaclesLayerMask;

    public override string GetActionName()
    {
        return "Deadeye Shot";
    }

    public override string GetActionDescription()
    {
        return actionDescription;
    }

    public override int GetRequiredHeldActions()
    {
        return 2;
    }

    public override bool ActionDealsDamage()
    {
        return true;
    }

    public override StatBonus GetStatBonus()
    {
        return actionStatBonus;
    }

    public override int GetUIPriority()
    {
        return 5;
    }

    public override (int, int) GetActionRange()
    {
        return unit.GetUnitStats().GetAttackRange();
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            //Looks at the target to shoot at
            case State.Aiming:
                //Vector3 aimDir = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;

                //float rotateSpeed = 10f;
                //transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * rotateSpeed);
                break;
            //Fires the shot
            case State.Shooting:
                if (canShoot)
                {
                    Shoot();
                    canShoot = false;
                }
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
            case State.Aiming:
                state = State.Shooting;
                float shootingStateTime = 0.1f;
                stateTimer = shootingStateTime;
                break;
            case State.Shooting:
                state = State.Cooloff;
                float coolOffStateTime = 0.2f;
                stateTimer = coolOffStateTime;
                break;
            case State.Cooloff:
                ActionComplete();
                break;
        }
    }

    private void Shoot()
    {
        AudioSource.PlayClipAtPoint(
            shootCrossbowSFX,
            Camera.main.transform.position,
            SoundManager.Instance.GetSoundEffectVolume()
        );

        if (attackSucceeded)
        {
            int damageAmount = unit.GetUnitStats().GetDamage();
            targetUnit.Damage(damageAmount);
            AttackHit(damageAmount);
        }
    }

    //See MoveAction
    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();

        return GetValidActionGridPositionList(unitGridPosition);
    }

    //Very similar logic to MoveAction
    public override List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        (int, int) attackRange = unit.GetUnitStats().GetAttackRange();
        int minShootDistance = attackRange.Item1;
        int maxShootDistance = attackRange.Item2;

        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if ((testDistance > maxShootDistance) || (testDistance < minShootDistance))
                {
                    continue;
                }

                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    // Grid Position is empty, no Unit
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (targetUnit.IsEnemy() == unit.IsEnemy())
                {
                    // Both Units on same 'team'
                    continue;
                }

                Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
                Vector3 shootDir = (targetUnit.GetWorldPosition() - unitWorldPosition).normalized;

                float unitShoulderHeight = 1.7f;
                if (
                    Physics.Raycast(
                        unitWorldPosition + Vector3.up * unitShoulderHeight,
                        shootDir,
                        Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()),
                        obstaclesLayerMask
                    )
                )
                {
                    // Blocked by an Obstacle
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        //unit.UseHeldActions(GetRequiredHeldActions());

        attackSucceeded = false;
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        attackSucceeded = CombatSystem.Instance.TryAttack(unit, targetUnit);
        state = State.Aiming;
        float aimingStateTime = 0.75f;
        stateTimer = aimingStateTime;

        canShoot = true;
        ActionStart(onActionComplete);
    }

    //Action value to shoot a player Unit is high, very likely to do it if possible
    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 100 + Mathf.RoundToInt((1f / targetUnit.GetHealth()) * 100f),
        };
    }

    //Returns how many units could be shot from GridPosition
    public override int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList(gridPosition).Count;
    }
}
