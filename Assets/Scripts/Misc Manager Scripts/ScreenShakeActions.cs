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
        // ShootAction.OnAnyShoot += ShootAction_OnAnyShoot;
        // SwordAction.OnAnySwordHit += SwordAction_OnAnySwordHit;
        // CleaveAction.OnAnyCleaveHit += SlashAction_OnAnySlashHit;
        FireballProjectile.OnAnyFireballExploded += FireballProjectile_OnAnyFireballExploded;
        // FireboltAction.OnAnyShoot += FireboltAction_OnAnyShoot;
    }

    private void OnDisable()
    {
        BaseAction.OnAnyAttackHit -= BaseAction_OnAnyAttackHit;
        // ShootAction.OnAnyShoot -= ShootAction_OnAnyShoot;
        // SwordAction.OnAnySwordHit -= SwordAction_OnAnySwordHit;
        // CleaveAction.OnAnyCleaveHit -= SlashAction_OnAnySlashHit;
        FireballProjectile.OnAnyFireballExploded -= FireballProjectile_OnAnyFireballExploded;
        // FireboltAction.OnAnyShoot -= FireboltAction_OnAnyShoot;
    }

    private void BaseAction_OnAnyAttackHit(object sender, float damageAmount)
    {
        ScreenShake.Instance.Shake(damageAmount * damageToShakeModifier);
    }

    // private void SwordAction_OnAnySwordHit(object sender, EventArgs e)
    // {
    //     ScreenShake.Instance.Shake(4f);
    // }

    private void FireballProjectile_OnAnyFireballExploded(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake(5f);
    }

    // private void ShootAction_OnAnyShoot(object sender, ShootAction.OnShootEventArgs e)
    // {
    //     ScreenShake.Instance.Shake(2f);
    // }

    // private void FireboltAction_OnAnyShoot(object sender, FireboltAction.OnShootEventArgs e)
    // {
    //     ScreenShake.Instance.Shake(2f);
    // }
}
