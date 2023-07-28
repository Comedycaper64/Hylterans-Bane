using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RallyingCry : MonoBehaviour
{
    protected Unit unit;

    public abstract string GetAbilityName();

    public abstract string GetAbilityDescription();

    public abstract void PerformAbility();

    public virtual int GetRequiredHeldActions()
    {
        return 0;
    }

    public Unit GetUnit()
    {
        return unit;
    }

    protected virtual void Awake()
    {
        unit = GetComponent<Unit>();
    }
}
