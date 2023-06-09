using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initiative
{
    public Initiative(Unit unit, int unitInitiative)
    {
        this.unit = unit;
        this.unitInitiative = unitInitiative;
    }

    public int unitInitiative;
    public Unit unit;
}
