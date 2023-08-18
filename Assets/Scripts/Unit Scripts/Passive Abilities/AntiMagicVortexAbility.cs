using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiMagicVortexAbility : PassiveAbility
{
    private List<Unit> buffedUnits = new List<Unit>();

    private int abilityRange = 2;

    private void Start()
    {
        MoveAction.OnAnyUnitMoved += MoveAction_OnAnyUnitMoved;
        StartCoroutine(InitialBuff());
    }

    private void OnDisable()
    {
        MoveAction.OnAnyUnitMoved -= MoveAction_OnAnyUnitMoved;
    }

    //To circumvent script execution issues
    private IEnumerator InitialBuff()
    {
        yield return null;
        UpdateBuff();
    }

    private void UpdateBuff()
    {
        List<Unit> friendlyUnits = UnitManager.Instance.GetFriendlyUnitList();
        foreach (Unit friendlyUnit in friendlyUnits)
        {
            GridPosition unitDistance = friendlyUnit.GetGridPosition() - unit.GetGridPosition();
            int distanceInt = Mathf.Abs(unitDistance.x) + Mathf.Abs(unitDistance.z);
            bool unitOutOfRange = distanceInt > abilityRange ? true : false;

            if (buffedUnits.Contains(friendlyUnit) && unitOutOfRange)
            {
                DebuffUnit(friendlyUnit);
            }
            else if (!buffedUnits.Contains(friendlyUnit) && !unitOutOfRange)
            {
                BuffUnit(friendlyUnit);
            }
        }
    }

    private void BuffUnit(Unit allyUnit)
    {
        //Add effect to Unit World UI
        UnitStats unitStats = allyUnit.GetUnitStats();
        unitStats.savingThrowAugment += 1;
        buffedUnits.Add(allyUnit);
    }

    private void DebuffUnit(Unit allyUnit)
    {
        //Remove effect from Unit World UI
        UnitStats unitStats = allyUnit.GetUnitStats();
        unitStats.savingThrowAugment -= 1;
        buffedUnits.Remove(allyUnit);
    }

    private void MoveAction_OnAnyUnitMoved(object sender, GridPosition newPosition)
    {
        MoveAction sendingAction = (MoveAction)sender;
        Unit sendingUnit = sendingAction.GetUnit();

        if (!sendingUnit.IsEnemy())
        {
            if (sendingUnit == unit)
            {
                UpdateBuff();
                return;
            }

            GridPosition sendingUnitDistance = newPosition - unit.GetGridPosition();
            int distanceInt = Mathf.Abs(sendingUnitDistance.x) + Mathf.Abs(sendingUnitDistance.z);
            bool unitOutOfRange = distanceInt > abilityRange ? true : false;

            if (unitOutOfRange && buffedUnits.Contains(sendingUnit))
            {
                DebuffUnit(sendingUnit);
            }
            else if (!unitOutOfRange && !buffedUnits.Contains(sendingUnit))
            {
                BuffUnit(sendingUnit);
            }
        }
    }

    public override string GetAbilityDescription()
    {
        return "The weaving of abjuration gives nearby units advantage on saving throws against magic";
    }

    public override string GetAbilityName()
    {
        return "Anti-Magic Vortex";
    }
}
