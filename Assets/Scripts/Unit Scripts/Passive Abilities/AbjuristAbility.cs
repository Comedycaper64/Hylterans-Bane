using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbjuristAbility : PassiveAbility
{
    public override string GetAbilityDescription()
    {
        return "Units under the effect of 'Magical Aegis' gain +2 AC and +2 to saving throws. \n'After centuries of study it becomes trivial to empower established magicks.'";
    }

    public override string GetAbilityName()
    {
        return "Abjurist";
    }
}
