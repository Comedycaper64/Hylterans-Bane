using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossStrikeAction : BaseAction
{
    private readonly string actionDescription =
        "Utilising both blades, swipe across enemies near you.";

    [SerializeField]
    private readonly StatBonus actionStatBonus = new(0, 2, 0);

    // [SerializeField]
    // private AudioClip strikeHitSFX;

    private enum State
    {
        SwingingSwordBeforeHit,
        SwingingSwordAfterHit,
    }

    private int maxStrikeDistance = 1;
    private State state;
    private float stateTimer;
    private List<Unit> targetUnits = new List<Unit>();

    public override string GetActionName()
    {
        return "Cross Strike";
    }

    public override string GetActionDescription()
    {
        return actionDescription;
    }

    public override (int, int) GetDamageArea()
    {
        return (3, 1);
    }

    public override bool GetIsAOE()
    {
        return true;
    }

    public override AOEType GetAOEType()
    {
        return AOEType.Line;
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
        return maxStrikeDistance;
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

        for (int x = -maxStrikeDistance; x <= maxStrikeDistance; x++)
        {
            for (int z = -maxStrikeDistance; z <= maxStrikeDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = gridPosition + offsetGridPosition;

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxStrikeDistance)
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

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnits = new List<Unit>();

        if (LevelGrid.Instance.TryGetUnitAtGridPosition(gridPosition, out Unit unit))
        {
            targetUnits.Add(unit);
        }
        GridPosition distanceFromAttacker = gridPosition - this.unit.GetGridPosition();
        if (distanceFromAttacker.x == 0)
        {
            if (
                LevelGrid.Instance.TryGetUnitAtGridPosition(
                    gridPosition + new GridPosition(1, 0),
                    out Unit unit1
                )
            )
            {
                targetUnits.Add(unit1);
            }
            if (
                LevelGrid.Instance.TryGetUnitAtGridPosition(
                    gridPosition + new GridPosition(-1, 0),
                    out Unit unit2
                )
            )
            {
                targetUnits.Add(unit2);
            }
        }
        else
        {
            if (
                LevelGrid.Instance.TryGetUnitAtGridPosition(
                    gridPosition + new GridPosition(0, 1),
                    out Unit unit1
                )
            )
            {
                targetUnits.Add(unit1);
            }
            if (
                LevelGrid.Instance.TryGetUnitAtGridPosition(
                    gridPosition + new GridPosition(0, -1),
                    out Unit unit2
                )
            )
            {
                targetUnits.Add(unit2);
            }
        }

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
