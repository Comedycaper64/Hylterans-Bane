using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryFireAction : BaseAction
{
    private readonly string actionDescription =
        "A powerful cannon blast hitting enemies in a line from this unit. \n+2 damage \nHeld Actions Used : 3 \nAttack Roll against Enemy Armour.";

    [SerializeField]
    private readonly StatBonus actionStatBonus = new(0, 2, 0);

    // [SerializeField]
    // private AudioClip strikeHitSFX;

    private enum State
    {
        SwingingSwordBeforeHit,
        SwingingSwordAfterHit,
    }

    private int fireDistance = 1;
    private State state;
    private float stateTimer;
    private List<Unit> targetUnits = new List<Unit>();

    public override string GetActionName()
    {
        return "Fire!";
    }

    public override string GetActionDescription()
    {
        return actionDescription;
    }

    public override (int, int) GetDamageArea()
    {
        return (3, 10);
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

    public override int GetRequiredHeldActions()
    {
        return 3;
    }

    public override StatBonus GetStatBonus()
    {
        return actionStatBonus;
    }

    public override int GetActionRange()
    {
        return fireDistance;
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

        for (int x = -fireDistance; x <= fireDistance; x++)
        {
            for (int z = -fireDistance; z <= fireDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = gridPosition + offsetGridPosition;
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);

                if (testDistance != fireDistance)
                {
                    continue;
                }

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
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

        // GridPosition distanceFromAttacker = gridPosition - unit.GetGridPosition();

        // var damageRange = GetDamageArea();
        // if (distanceFromAttacker.z == 0)
        // {
        //     (damageRange.Item1, damageRange.Item2) = (damageRange.Item2, damageRange.Item1);
        // }

        // (damageRange.Item1, damageRange.Item2) = (
        //     Mathf.RoundToInt(damageRange.Item1 - 1) / 2,
        //     Mathf.RoundToInt(damageRange.Item2 - 1) / 2
        // );

        // for (int x = -damageRange.Item1; x <= damageRange.Item1; x++)
        // {
        //     for (int z = -damageRange.Item2; z <= damageRange.Item2; z++)
        //     {
        //         GridPosition offsetGridPosition = new GridPosition(x, z);
        //         GridPosition testGridPosition = gridPosition + offsetGridPosition;

        //         if (
        //             LevelGrid.Instance.TryGetUnitAtGridPosition(
        //                 testGridPosition,
        //                 out Unit targetUnit
        //             )
        //         )
        //         {
        //             targetUnits.Add(targetUnit);
        //         }
        //     }
        // }

        targetUnits = GetUnitsInAOE(gridPosition, GetDamageArea(), GetAOEType());

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
