using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatChangeEffect : MonoBehaviour
{
    Unit unit;
    UnitStats unitStats;
    private StatBonus statBonus;
    private int duration;

    public void SetStatChange(StatBonus statBonus, int duration = 1)
    {
        unitStats = GetComponent<UnitStats>();
        unitStats.currentStatBonus += statBonus;
        this.statBonus = statBonus;
        this.duration = duration;
        unit = GetComponent<Unit>();
        unit.OnUnitTurnEnd += DecrementDuration;
    }

    private void OnDisable()
    {
        unit.OnUnitTurnEnd -= DecrementDuration;
    }

    private void DecrementDuration()
    {
        duration--;
        if (duration <= 0)
        {
            unitStats.currentStatBonus -= statBonus;
            Destroy(this);
        }
    }
}
