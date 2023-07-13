using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlazeEffect : MonoBehaviour
{
    private Unit unit;
    private HealthSystem unitHealthSystem;
    private int effectDuration = 3;

    private void OnEnable()
    {
        unitHealthSystem = GetComponent<HealthSystem>();
        unit = GetComponent<Unit>();

        unit.OnUnitTurnStart += DealDamage;
    }

    private void OnDisable()
    {
        unit.OnUnitTurnStart -= DealDamage;
    }

    private void DealDamage()
    {
        unitHealthSystem.Damage(1);
        effectDuration--;
        if (effectDuration <= 0)
        {
            Destroy(this);
        }
    }
}
