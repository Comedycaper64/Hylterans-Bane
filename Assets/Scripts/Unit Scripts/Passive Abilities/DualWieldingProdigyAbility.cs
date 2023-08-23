using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualWieldingProdigyAbility : PassiveAbility
{
    public override string GetAbilityDescription()
    {
        return "This unit strikes twice when the attack action is taken";
    }

    public override string GetAbilityName()
    {
        return "Dual-Wielding Prodigy";
    }
}
