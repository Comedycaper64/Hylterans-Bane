using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraAttackAbility : PassiveAbility
{
    public override string GetAbilityDescription()
    {
        return "Allows this unit to perform an extra strike when taking the attack action.";
    }

    public override string GetAbilityName()
    {
        return "Extra Attack";
    }
}
