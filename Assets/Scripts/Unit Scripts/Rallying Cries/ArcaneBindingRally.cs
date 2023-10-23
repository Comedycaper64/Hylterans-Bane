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
        return "Prevents enemy units from casting spells on their next turn \nSpirit Used: 2 \n<i>'Go forth freely, no spells will escape their lips!'</i>";
    }

    public override string GetAbilityName()
    {
        return "Arcane Binding";
    }

    public override int GetRequiredSpirit()
    {
        return 2;
    }

    public override void PerformAbility(Action onAbilityCompleted)
    {
        List<Unit> enemyUnits = UnitManager.Instance.GetEnemyUnitList();
        foreach (Unit enemyUnit in enemyUnits)
        {
            enemyUnit.gameObject.AddComponent<ArcaneBindingEffect>();
        }

        AbilityStart(onAbilityCompleted);
    }
}
