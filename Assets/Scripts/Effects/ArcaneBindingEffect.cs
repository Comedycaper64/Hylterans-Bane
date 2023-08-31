using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneBindingEffect : MonoBehaviour
{
    Unit unit;

    private void OnEnable()
    {
        unit = GetComponent<Unit>();
        unit.OnUnitTurnEnd += DisableEffect;
    }

    private void OnDisable()
    {
        unit.OnUnitTurnEnd -= DisableEffect;
    }

    private void DisableEffect()
    {
        Destroy(this);
    }
}
