using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualWieldingProdigyAbility : PassiveAbility
{
    public override string GetAbilityDescription()
    {
        return "Allows this unit to perform an extra strike when taking the attack action. This stacks with the Extra Attack ability.";
    }

    public override string GetAbilityName()
    {
        return "Dual-Wielding Prodigy";
    }
}
