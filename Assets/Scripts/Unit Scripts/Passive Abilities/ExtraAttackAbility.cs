using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(MultiAttackAction))]
public class ExtraAttackAbility : PassiveAbility
{
    public override string GetAbilityDescription()
    {
        return "Allows this unit to perform extra attacks at the cost of <b>Spirit</b>.";
    }

    public override string GetAbilityName()
    {
        return "Fighting Spirit";
    }

    public override int GetAbilityUnlockLevel()
    {
        return 5;
    }
}
