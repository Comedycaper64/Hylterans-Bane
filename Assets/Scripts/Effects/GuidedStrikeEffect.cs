using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidedStrikeEffect : MonoBehaviour
{
    Unit unit;
    UnitStats unitStats;
    private StatBonus guidingBonus = new StatBonus(10);

    private void OnEnable()
    {
        unitStats = GetComponent<UnitStats>();
        unitStats.currentStatBonus += guidingBonus;
        unit = GetComponent<Unit>();
        unit.OnUnitTurnEnd += DisableEffect;
    }

    private void OnDisable()
    {
        unit.OnUnitTurnEnd -= DisableEffect;
    }

    private void DisableEffect()
    {
        unitStats.currentStatBonus -= guidingBonus;
        Destroy(this);
    }
}
