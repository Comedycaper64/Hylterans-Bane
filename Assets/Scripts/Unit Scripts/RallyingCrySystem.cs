using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RallyingCrySystem : MonoBehaviour
{
    private UnitActionSystem unitActionSystem;

    private void Awake()
    {
        unitActionSystem = GetComponent<UnitActionSystem>();
        RallyingCryButtonUI.OnChooseRallyingCry += RallyingCryButtonUI_OnChooseRallyingCry;
    }

    private void OnDisable()
    {
        RallyingCryButtonUI.OnChooseRallyingCry -= RallyingCryButtonUI_OnChooseRallyingCry;
    }

    private void PerformRallyingCry(RallyingCry rallyingCry)
    {
        if (unitActionSystem.GetIsBusy()) { }
        else { }
    }

    private void RallyingCryButtonUI_OnChooseRallyingCry(object sender, RallyingCry rallyingCry)
    {
        PerformRallyingCry(rallyingCry);
    }

    //callback event runs a script that sees if the callback is empty (if so do nothing), then sees if the action system is busy.
    //If it's the enemy turn or the system is busy, a new initiative is created for the unit to do a rallying cry
    //Otherwise it's simply done
}
