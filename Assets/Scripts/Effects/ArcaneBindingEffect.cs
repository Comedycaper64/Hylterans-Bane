using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneBindingEffect : MonoBehaviour
{
    Unit unit;
    List<BaseAction> spellActions = new List<BaseAction>();

    private void OnEnable()
    {
        unit = GetComponent<Unit>();
        unit.OnUnitTurnEnd += DisableEffect;
    }

    private void OnDisable()
    {
        unit.OnUnitTurnEnd -= DisableEffect;
    }

    private void BindSpells()
    {
        foreach (BaseAction baseAction in unit.GetBaseActionList())
        {
            if (baseAction.IsSpell())
            {
                baseAction.SetDisabled(true);
                spellActions.Add(baseAction);
            }
        }
    }

    private void UnbindSpells()
    {
        foreach (BaseAction baseAction in spellActions)
        {
            baseAction.SetDisabled(false);
        }
    }

    private void DisableEffect()
    {
        UnbindSpells();
        Destroy(this);
    }
}
