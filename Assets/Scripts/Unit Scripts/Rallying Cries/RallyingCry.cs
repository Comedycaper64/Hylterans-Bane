using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RallyingCry : MonoBehaviour
{
    public event EventHandler OnAbilityStarted;
    public event EventHandler OnAbilityCompleted;

    protected Unit unit;
    protected bool isActive;
    protected Action onActionComplete;

    public abstract string GetAbilityName();

    public abstract string GetAbilityDescription();

    public abstract void PerformAbility(Action onAbilityComplete);

    protected void AbilityStart(Action onActionComplete)
    {
        unit.GetSpiritSystem().UseSpirit(GetRequiredSpirit());
        isActive = true;
        this.onActionComplete = onActionComplete;
        OnAbilityStarted?.Invoke(this, EventArgs.Empty);
    }

    protected void AbilityComplete()
    {
        isActive = false;
        if (onActionComplete != null)
            onActionComplete();
        OnAbilityCompleted?.Invoke(this, EventArgs.Empty);
    }

    public virtual int GetRequiredSpirit()
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
