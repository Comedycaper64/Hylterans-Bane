using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AegisEffect : MonoBehaviour
{
    HealthSystem unitHealthSystem;

    private void OnEnable()
    {
        unitHealthSystem = GetComponent<HealthSystem>();

        unitHealthSystem.SetInvincible(true);
        unitHealthSystem.OnDamaged += DisableShield;
    }

    private void OnDisable()
    {
        unitHealthSystem.OnDamaged -= DisableShield;
    }

    private void DisableShield(object sender, float e)
    {
        unitHealthSystem.SetInvincible(false);
        Destroy(this);
    }
}
