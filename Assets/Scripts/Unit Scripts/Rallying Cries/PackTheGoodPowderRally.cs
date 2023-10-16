using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackTheGoodPowderRally : RallyingCry
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
        return "Immediately allows this unit to use its Fire! action, with a +5 to hit.\nHeld Actions Used: 3 \n<i>'Pack the good powder!'</i>";
    }

    public override string GetAbilityName()
    {
        return "Pack The Good Powder!";
    }

    public override void PerformAbility(Action onAbilityComplete)
    {
        //Add Apprentice Fire! action to initiative order
        TurnSystem.Instance.AddInitiativeToOrder(
            new Initiative(unit, unit.GetAction<ArtilleryFireAction>(), 0)
        );
        StatChangeEffect statChange = unit.gameObject.AddComponent<StatChangeEffect>();
        statChange.SetStatChange(new StatBonus(5));
        AbilityStart(onAbilityComplete);
    }
}
