using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneBindingRally : RallyingCry
{
    float waitTimer = 1f;

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        waitTimer -= Time.deltaTime;
        if (waitTimer < 0f)
        {
            waitTimer = 1f;
            AbilityComplete();
        }
    }

    public override string GetAbilityDescription()
    {
        return "'Go forth freely!' \nPrevents enemy units from casting spells on their next turn";
    }

    public override string GetAbilityName()
    {
        return "Arcane Binding";
    }

    public override void PerformAbility(Action onAbilityCompleted)
    {
        List<Unit> enemyUnits = UnitManager.Instance.GetFriendlyUnitList();
        foreach (Unit enemyUnit in enemyUnits)
        {
            enemyUnit.gameObject.AddComponent<ArcaneBindingEffect>();
        }

        AbilityStart(onAbilityCompleted);
    }
}
