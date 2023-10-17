using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(MultiAttackAction))]
public class DualWieldingProdigyAbility : PassiveAbility
{
    public override string GetAbilityDescription()
    {
        return "Allows this unit to perform an extra strike when taking the attack action. This stacks with the Extra Attack ability. \n<i>'Many envied the dextrous style unique to the Swiftblade.'</i>";
    }

    public override string GetAbilityName()
    {
        return "Dual-Wielding Prodigy";
    }
}
