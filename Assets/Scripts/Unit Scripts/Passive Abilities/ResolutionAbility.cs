using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionAbility : PassiveAbility
{
    HealthSystem healthSystem;

    private void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
        unit.OnUnitTurnStart += Unit_OnUnitTurnStart;
    }

    private void OnDisable()
    {
        unit.OnUnitTurnStart -= Unit_OnUnitTurnStart;
    }

    public override string GetAbilityDescription()
    {
        return "Survives a lethal attack with 1 hit point once a turn. \n'A queen's fervor is unbreakable.'";
    }

    public override string GetAbilityName()
    {
        return "Resolution";
    }

    private void Unit_OnUnitTurnStart()
    {
        if (IsDisabled())
        {
            return;
        }
        healthSystem.SetResolute(true);
    }
}
