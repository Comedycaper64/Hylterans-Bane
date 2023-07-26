using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PassiveAbility : MonoBehaviour
{
    protected Unit unit;

    public abstract string GetAbilityName();

    public abstract string GetAbilityDescription();

    protected virtual void Awake()
    {
        unit = GetComponent<Unit>();
    }
}
