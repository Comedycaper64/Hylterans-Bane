using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    //Contains logic for managing health, fires off events when taking damage or dying
    public event EventHandler OnDead;
    public event EventHandler<float> OnDamaged;

    [SerializeField]
    private int health = 100;
    private int healthMax;
    private bool invincible;
    private bool resolute;

    public void Damage(int damageAmount)
    {
        if (invincible)
        {
            damageAmount = 0;
        }

        health -= damageAmount;

        if (health < 0)
        {
            if (resolute)
            {
                Debug.Log("Resolute!");
                health = 1;
                SetResolute(false);
            }
            else
            {
                health = 0;
            }
        }

        OnDamaged?.Invoke(this, (float)damageAmount);

        if (health == 0)
        {
            Die();
        }
    }

    public void SetHealth(int health)
    {
        healthMax = health;
        this.health = healthMax;
    }

    private void Die()
    {
        OnDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetMaxHealth()
    {
        return (float)healthMax;
    }

    public float GetHealth()
    {
        return (float)health;
    }

    public float GetAmountNormalised(float amount)
    {
        return (float)amount / healthMax;
    }

    public float GetHealthNormalized()
    {
        return (float)health / healthMax;
    }

    public void SetInvincible(bool enable)
    {
        invincible = enable;
    }

    public void SetResolute(bool enable)
    {
        resolute = enable;
    }
}
