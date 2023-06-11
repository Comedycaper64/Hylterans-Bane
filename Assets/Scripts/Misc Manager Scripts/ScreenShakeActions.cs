using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeActions : MonoBehaviour
{
    private void Start()
    {
        ShootAction.OnAnyShoot += ShootAction_OnAnyShoot;
        SwordAction.OnAnySwordHit += SwordAction_OnAnySwordHit;
        CleaveAction.OnAnyCleaveHit += SlashAction_OnAnySlashHit;
        FireballProjectile.OnAnyFireballExploded += FireballProjectile_OnAnyFireballExploded;
        FireboltAction.OnAnyShoot += FireboltAction_OnAnyShoot;
    }

    private void OnDisable()
    {
        ShootAction.OnAnyShoot -= ShootAction_OnAnyShoot;
        SwordAction.OnAnySwordHit -= SwordAction_OnAnySwordHit;
        CleaveAction.OnAnyCleaveHit -= SlashAction_OnAnySlashHit;
        FireballProjectile.OnAnyFireballExploded -= FireballProjectile_OnAnyFireballExploded;
        FireboltAction.OnAnyShoot -= FireboltAction_OnAnyShoot;
    }

    private void SlashAction_OnAnySlashHit(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake(10f);
    }

    private void SwordAction_OnAnySwordHit(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake(4f);
    }

    private void FireballProjectile_OnAnyFireballExploded(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake(5f);
    }

    private void ShootAction_OnAnyShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        ScreenShake.Instance.Shake(2f);
    }

    private void FireboltAction_OnAnyShoot(object sender, FireboltAction.OnShootEventArgs e)
    {
        ScreenShake.Instance.Shake(2f);
    }
}
