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
        return "'Pack the good powder!' \nImmediately allows this unit to use its Fire! action, with a +5 to hit.";
    }

    public override string GetAbilityName()
    {
        return "Pack The Good Powder!";
    }

    public override void PerformAbility(Action onAbilityComplete)
    {
        throw new NotImplementedException();
    }
}
