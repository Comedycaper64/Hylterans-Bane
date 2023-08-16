using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleaveAction : BaseAction
{
    private string actionDescription = "A sweeping blow that hits enemies around the unit.";

    [SerializeField]
    private StatBonus actionStatBonus = new StatBonus(0, -2, 0);

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

    public override string GetActionName()
    {
        return "Cleave";
    }

    public override string GetActionDescription()
    {
        return actionDescription;
    }

    public override (int, int) GetDamageArea()
    {
        return (3, 3);
    }

    public override bool GetIsAOE()
    {
        return true;
    }

    public override bool ActionDealsDamage()
    {
        return true;
    }

    public override StatBonus GetStatBonus()
    {
        return actionStatBonus;
    }

    public override int GetActionRange()
    {
        return maxSlashDistance;
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        stateTimer -= Time.deltaTime;

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

                break;
        }
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
        targetUnits = GetUnitsInAOE(gridPosition, GetDamageArea());

        state = State.SwingingSwordBeforeHit;
        float beforeHitStateTime = 1.75f;
        stateTimer = beforeHitStateTime;

        ActionStart(onActionComplete);
    }

    private IEnumerator DealDamageToEachTarget(List<Unit> targetUnits)
    {
        List<Unit> hitUnits = new List<Unit>();
        foreach (Unit targetUnit in targetUnits)
        {
            AttackInteraction targetUnitAttackInteraction;
            bool unitHit = CombatSystem.Instance.TryAttack(
                unit.GetUnitStats(),
                targetUnit.GetUnitStats(),
                out targetUnitAttackInteraction
            );
            targetUnit.PerformAOEAttack(targetUnitAttackInteraction);
            if (unitHit)
            {
                hitUnits.Add(targetUnit);
            }
        }
        yield return new WaitForSeconds(1f);
        foreach (Unit hitUnit in hitUnits)
        {
            int damageAmount = unit.GetUnitStats().GetDamage();
            hitUnit.gameObject.GetComponent<Unit>().Damage(damageAmount);
            AttackHit(damageAmount);
        }
        yield return new WaitForSeconds(1f);
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
                    }
                }
            }
        }
        return new EnemyAIAction { gridPosition = gridPosition, actionValue = targetsInAOE * 150, };
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
