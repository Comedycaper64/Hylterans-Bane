using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitiativeUI : MonoBehaviour
{
    private Unit initiativeUnit;

    public void SetupInitiativeUnit(Unit initiativeUnit)
    {
        this.initiativeUnit = initiativeUnit;
    }

    public void InitiativeUIPressed()
    {
        if (initiativeUnit)
        {
            GetComponentInParent<TurnSystemUI>().InitiativeUIPressed(initiativeUnit);
        }
    }
}
