using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrecisionPriorityRally : RallyingCry
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
        return "Grants each friendly unit +10 to hit until the end of their turn. \nSpirit Used: 1 \n<i>'Steady aim, make this one count!'</i>";
    }

    public override string GetAbilityName()
    {
        return "Precision Priority";
    }

    public override int GetRequiredSpirit()
    {
        return 1;
    }

    public override void PerformAbility(Action onAbilityCompleted)
    {
        List<Unit> friendlyUnits = UnitManager.Instance.GetFriendlyUnitList();
        foreach (Unit friendlyUnit in friendlyUnits)
        {
            if (friendlyUnit.GetAction<AttackAction>())
            {
                friendlyUnit.gameObject.AddComponent<GuidedStrikeEffect>();
            }
        }

        AbilityStart(onAbilityCompleted);
    }
}
