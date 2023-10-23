using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AegisEffect : MonoBehaviour
{
    HealthSystem unitHealthSystem;
    UnitStats unitStats;

    private bool abjuristAegis = false;
    private StatBonus abjuristBonus = new StatBonus(0, 0, 0, 2);

    // When enabled, Aegis makes unit invulnerable to next attack
    private void OnEnable()
    {
        unitHealthSystem = GetComponent<HealthSystem>();
        unitStats = GetComponent<UnitStats>();

        unitHealthSystem.SetInvincible(true);
        unitHealthSystem.OnDamaged += DisableShield;
    }

    private void OnDisable()
    {
        unitHealthSystem.OnDamaged -= DisableShield;
    }

    //If unit has Abjurist ability, Aegis grants AC bonus
    public void AbjuristAegis()
    {
        abjuristAegis = true;
        unitStats.currentStatBonus += abjuristBonus;
    }

    //Disables shield when damage taken
    private void DisableShield(object sender, float e)
    {
        if (abjuristAegis)
        {
            unitStats.currentStatBonus -= abjuristBonus;
        }
        unitHealthSystem.SetInvincible(false);
        Destroy(this);
    }
}
