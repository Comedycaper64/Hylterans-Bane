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

    //Deals 10% of unit health as damage at the start of unit turn. Destroys effect once effect duration is 0
    private void DealDamage()
    {
        unitHealthSystem.Damage(Mathf.RoundToInt(unitHealthSystem.GetMaxHealth() / 10));
        effectDuration--;
        if (effectDuration <= 0)
        {
            Destroy(this);
        }
    }
}
