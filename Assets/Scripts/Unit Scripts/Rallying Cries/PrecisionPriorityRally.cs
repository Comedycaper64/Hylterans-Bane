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
        return "'Steady aim!' \nGrants each friendly unit +10 to hit until the end of their turn.";
    }

    public override string GetAbilityName()
    {
        return "Precision Priority";
    }

    public override int GetRequiredHeldActions()
    {
        return 2;
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
