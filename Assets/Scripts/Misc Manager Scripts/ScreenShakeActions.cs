using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeActions : MonoBehaviour
{
    private float damageToShakeModifier = 0.5f;

    private void Start()
    {
        BaseAction.OnAnyAttackHit += BaseAction_OnAnyAttackHit;
        AbilityProjectile.OnAnyProjectileExploded += FireballProjectile_OnAnyFireballExploded;
    }

    private void OnDisable()
    {
        BaseAction.OnAnyAttackHit -= BaseAction_OnAnyAttackHit;
        AbilityProjectile.OnAnyProjectileExploded -= FireballProjectile_OnAnyFireballExploded;
    }

    private void BaseAction_OnAnyAttackHit(object sender, float damageAmount)
    {
        ScreenShake.Instance.Shake(damageAmount * damageToShakeModifier);
    }

    private void FireballProjectile_OnAnyFireballExploded(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake(5f);
    }
}
