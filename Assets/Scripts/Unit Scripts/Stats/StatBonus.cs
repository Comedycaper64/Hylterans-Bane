using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StatBonus
{
    public int toHitBonus;
    public int damageBonus;
    public int rangeBonus;

    public StatBonus(int toHitBonus = 0, int damageBonus = 0, int rangeBonus = 0)
    {
        this.toHitBonus = toHitBonus;
        this.damageBonus = damageBonus;
        this.rangeBonus = rangeBonus;
    }

    public static StatBonus operator +(StatBonus a, StatBonus b)
    {
        return new StatBonus(
            a.toHitBonus + b.toHitBonus,
            a.damageBonus + b.damageBonus,
            a.rangeBonus + b.rangeBonus
        );
    }

    public static StatBonus operator -(StatBonus a, StatBonus b)
    {
        return new StatBonus(
            a.toHitBonus - b.toHitBonus,
            a.damageBonus - b.damageBonus,
            a.rangeBonus - b.rangeBonus
        );
    }
}
