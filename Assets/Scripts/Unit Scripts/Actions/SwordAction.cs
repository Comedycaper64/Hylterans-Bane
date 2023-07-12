using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAction : BaseAction
{
    //[SerializeField]
    private string actionDescription = "A basic melee attack";
    public static event EventHandler OnAnySwordHit;

    // public event EventHandler OnSwordActionStarted;
    // public event EventHandler OnSwordActionCompleted;

    private bool attackSucceeded;

    [SerializeField]
    private AudioClip attackHitSFX;

    private enum State
    {
        SwingingSwordBeforeHit,
        SwingingSwordAfterHit,
    }

    private State state;
    private float stateTimer;
    private Unit targetUnit;

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.SwingingSwordBeforeHit:
                Vector3 aimDir = (
                    targetUnit.GetWorldPosition() - unit.GetWorldPosition()
                ).normalized;

                float rotateSpeed = 10f;
                transform.forward = Vector3.Lerp(
                    transform.forward,
                    aimDir,
                    Time.deltaTime * rotateSpeed
                );
                break;
            case State.SwingingSwordAfterHit:
                break;
        }

        if (stateTimer <= 0f)
        {
            NextState();
        }
    }

    private void NextState()
    {
        switch (state)
        {
            case State.SwingingSwordBeforeHit:
                state = State.SwingingSwordAfterHit;
                float afterHitStateTime = 0.5f;
                stateTimer = afterHitStateTime;
                if (attackSucceeded)
                {
                    int damageAmount = unit.GetUnitStats().GetDamage();
                    targetUnit.Damage(damageAmount);
                    AudioSource.PlayClipAtPoint(
                        attackHitSFX,
                        Camera.main.transform.position,
                        SoundManager.Instance.GetSoundEffectVolume()
                    );
                    OnAnySwordHit?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    //Play whiff sfx
                    // AudioSource.PlayClipAtPoint(
                    //     attackHitSFX,
                    //     Camera.main.transform.position,
                    //     SoundManager.Instance.GetSoundEffectVolume()
                    // );
                }

                break;
            case State.SwingingSwordAfterHit:
                //OnSwordActionCompleted?.Invoke(this, EventArgs.Empty);
                ActionComplete();
                break;
        }
    }

    public override string GetActionName()
    {
        return "Attack";
    }

    public override string GetActionDescription()
    {
        return actionDescription;
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        int maxSwordDistance = unit.GetUnitStats().GetAttackRange();

        for (int x = -maxSwordDistance; x <= maxSwordDistance; x++)
        {
            for (int z = -maxSwordDistance; z <= maxSwordDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
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

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override List<GridPosition> GetValidActionGridPositionList(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        int maxSwordDistance = unit.GetUnitStats().GetAttackRange();

        for (int x = -maxSwordDistance; x <= maxSwordDistance; x++)
        {
            for (int z = -maxSwordDistance; z <= maxSwordDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = gridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
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

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        attackSucceeded = false;
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        attackSucceeded = CombatSystem.Instance.TryAttack(
            unit.GetUnitStats(),
            targetUnit.GetUnitStats()
        );

        state = State.SwingingSwordBeforeHit;
        float beforeHitStateTime = 0.75f;
        stateTimer = beforeHitStateTime;

        //OnSwordActionStarted?.Invoke(this, EventArgs.Empty);

        ActionStart(onActionComplete);
    }

    public override bool ActionDealsDamage()
    {
        return true;
    }

    public override int GetUIPriority()
    {
        return 5;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 200 + Mathf.RoundToInt((1f / targetUnit.GetHealth()) * 100f),
            //+ Mathf.RoundToInt((1f - targetUnit.GetHealthNormalized()) * 10f),
        };
    }

    public override int GetActionRange()
    {
        return unit.GetUnitStats().GetAttackRange();
    }

    public override int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList(gridPosition).Count;
    }
}
