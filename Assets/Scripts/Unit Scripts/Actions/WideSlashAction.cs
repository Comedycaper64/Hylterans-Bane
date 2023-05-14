using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WideSlashAction : BaseAction
{
    public static event EventHandler OnAnySlashHit;
    public event EventHandler OnSlashActionStarted;
    public event EventHandler OnSlashActionCompleted;

    [SerializeField]
    private int damageAmount;

    [SerializeField]
    private AudioClip slashHitSFX;

    private enum State
    {
        SwingingSwordBeforeHit,
        SwingingSwordAfterHit,
    }

    private int maxSlashDistance = 1;
    private State state;
    private float stateTimer;
    private List<Unit> targetUnits = new List<Unit>();

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
                //Vector3 aimDir = (targetUnits[0].GetWorldPosition() - unit.GetWorldPosition()).normalized;

                //float rotateSpeed = 10f;
                //transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * rotateSpeed);
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
                float afterHitStateTime = 2f;
                stateTimer = afterHitStateTime;
                foreach (Unit targetUnit in targetUnits)
                {
                    if (targetUnit.IsEnemy())
                        targetUnit.Damage(damageAmount);
                    else
                        targetUnit.Damage(damageAmount * 2);
                }
                AudioSource.PlayClipAtPoint(
                    slashHitSFX,
                    Camera.main.transform.position,
                    SoundManager.Instance.GetSoundEffectVolume()
                );
                OnAnySlashHit?.Invoke(this, EventArgs.Empty);
                break;
            case State.SwingingSwordAfterHit:
                OnSlashActionCompleted?.Invoke(this, EventArgs.Empty);
                ActionComplete();
                break;
        }
    }

    public override string GetActionName()
    {
        return "Slash";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction { gridPosition = gridPosition, actionValue = 200, };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxSlashDistance; x <= maxSlashDistance; x++)
        {
            for (int z = -maxSlashDistance; z <= maxSlashDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxSlashDistance)
                {
                    continue;
                }

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                if (LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition) == unit)
                {
                    continue;
                }

                float damageRadius = 3f;
                List<Unit> tempUnitList = new List<Unit>();
                Collider[] colliderArray = Physics.OverlapSphere(
                    LevelGrid.Instance.GetWorldPosition(testGridPosition),
                    damageRadius
                );
                foreach (Collider collider in colliderArray)
                {
                    if (collider.TryGetComponent<Unit>(out Unit tempUnit))
                    {
                        if (tempUnit != unit)
                        {
                            tempUnitList.Add(tempUnit);
                        }
                    }
                }

                if (tempUnitList.Count < 1)
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
        //targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        targetUnits = new List<Unit>();
        float damageRadius = 3f;
        Collider[] colliderArray = Physics.OverlapSphere(
            LevelGrid.Instance.GetWorldPosition(gridPosition),
            damageRadius
        );
        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent<Unit>(out Unit targetUnit))
            {
                if (targetUnit != unit)
                {
                    targetUnits.Add(targetUnit);
                }
            }
        }

        state = State.SwingingSwordBeforeHit;
        float beforeHitStateTime = 1.75f;
        stateTimer = beforeHitStateTime;

        OnSlashActionStarted?.Invoke(this, EventArgs.Empty);

        ActionStart(onActionComplete);
    }

    public override int GetActionPointsCost()
    {
        return 2;
    }

    public override bool GetIsAOE()
    {
        return true;
    }

    public override bool ActionDealsDamage()
    {
        return true;
    }

    public int GetMaxSlashDistance()
    {
        return maxSlashDistance;
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList().Count;
    }
}
