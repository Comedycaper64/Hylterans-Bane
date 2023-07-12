using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleaveAction : BaseAction
{
    //[SerializeField]
    private string actionDescription = "A sweeping blow that hits enemies around the unit.";

    public static event EventHandler<Unit> OnDamageUnit;
    public static event EventHandler OnAnyCleaveHit;
    public static event Action OnCleaveDamageFinished;

    // public event EventHandler OnCleaveActionStarted;
    // public event EventHandler OnCleaveActionCompleted;

    [SerializeField]
    private int actionDamageBonus = 2;

    [SerializeField]
    private AudioClip cleaveHitSFX;

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
                StartCoroutine(DealDamageToEachTarget(targetUnits));
                break;
            case State.SwingingSwordAfterHit:
                //OnCleaveActionCompleted?.Invoke(this, EventArgs.Empty);
                break;
        }
    }

    public override string GetActionName()
    {
        return "Cleave";
    }

    public override string GetActionDescription()
    {
        return actionDescription;
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        return GetValidActionGridPositionList(unit.GetGridPosition());
    }

    public override List<GridPosition> GetValidActionGridPositionList(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        for (int x = -maxSlashDistance; x <= maxSlashDistance; x++)
        {
            for (int z = -maxSlashDistance; z <= maxSlashDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = gridPosition + offsetGridPosition;

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

        //OnCleaveActionStarted?.Invoke(this, EventArgs.Empty);

        ActionStart(onActionComplete);
    }

    private IEnumerator DealDamageToEachTarget(List<Unit> targetUnits)
    {
        foreach (Unit targetUnit in targetUnits)
        {
            OnDamageUnit?.Invoke(this, targetUnit);
            bool unitHit = CombatSystem.Instance.TryAttack(
                unit.GetUnitStats(),
                targetUnit.GetUnitStats()
            );
            yield return new WaitForSeconds(1f);
            if (unitHit)
            {
                int damageAmount = unit.GetUnitStats().GetDamage();
                targetUnit.gameObject.GetComponent<Unit>().Damage(damageAmount);
                OnAnyCleaveHit?.Invoke(this, EventArgs.Empty);
            }
            yield return new WaitForSeconds(1f);
        }
        OnCleaveDamageFinished?.Invoke();
        ActionComplete();
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
                        // Debug.Log(
                        //     "Current Grid Position is "
                        //         + gridPosition.x
                        //         + ", "
                        //         + gridPosition.z
                        //         + " Target found at "
                        //         + testGridPosition.x
                        //         + ", "
                        //         + testGridPosition.z
                        // );
                    }
                }
            }
        }
        return new EnemyAIAction { gridPosition = gridPosition, actionValue = targetsInAOE * 150, };
    }

    public override bool GetIsAOE()
    {
        return true;
    }

    public override bool ActionDealsDamage()
    {
        return true;
    }

    public override int GetDamageBonus()
    {
        return actionDamageBonus;
    }

    public override int GetActionRange()
    {
        return maxSlashDistance;
    }

    public override int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        int targetsInAOE = 0;
        for (int x = gridPosition.x - 2; x <= gridPosition.x + 2; x++)
        {
            for (int z = gridPosition.z - 2; z <= gridPosition.z + 2; z++)
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
        return targetsInAOE;
    }
}
