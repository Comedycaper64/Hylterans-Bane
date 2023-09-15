using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PassiveAbility : MonoBehaviour
{
    protected Unit unit;
    private bool isDisabled;

    public abstract string GetAbilityName();

    public abstract string GetAbilityDescription();

    public virtual bool IsDisabled()
    {
        return isDisabled;
    }

    public virtual int GetAbilityUnlockLevel()
    {
        return 1;
    }

    public void SetIsDisabled(bool enable)
    {
        isDisabled = enable;
    }

    protected virtual void Awake()
    {
        unit = GetComponent<Unit>();
    }
}
