using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritSystem : MonoBehaviour
{
    private int spirit;
    private int maxSpirit = 3;
    private int minSpirit = -3;
    public event EventHandler<int> OnSpiritChanged;

    private void Awake()
    {
        spirit = 0;
    }

    public void IncreaseSpirit()
    {
        if (spirit >= maxSpirit)
        {
            return;
        }

        spirit++;
        OnSpiritChanged?.Invoke(this, spirit);
    }

    public int GetSpirit()
    {
        return spirit;
    }

    public int GetMinSpirit()
    {
        return minSpirit;
    }

    public void UseSpirit(int numberUsed)
    {
        spirit -= numberUsed;
        OnSpiritChanged?.Invoke(this, spirit);
    }
}
