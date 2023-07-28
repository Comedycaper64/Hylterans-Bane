using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RallyingCryButtonUI : MonoBehaviour
{
    public static EventHandler<RallyingCry> OnChooseRallyingCry;

    private RallyingCry rallyingCry;

    [SerializeField]
    private Button button;

    [SerializeField]
    private TextMeshProUGUI unitName;

    [SerializeField]
    private TextMeshProUGUI abilityName;

    [SerializeField]
    private TextMeshProUGUI abilityDescription;

    private bool unitHasEnoughHeldActions;

    private bool rallyingCryUsed;

    private void Awake()
    {
        rallyingCryUsed = false;
    }

    public void Setup(RallyingCry rallyingCry)
    {
        this.rallyingCry = rallyingCry;
        unitName.text = rallyingCry.GetUnit().GetUnitName();
        abilityName.text = rallyingCry.GetAbilityName();
        abilityDescription.text = rallyingCry.GetAbilityDescription();
        CheckHeldActionRequirement(rallyingCry);
    }

    private void CheckHeldActionRequirement(RallyingCry rallyingCry)
    {
        unitHasEnoughHeldActions =
            rallyingCry.GetRequiredHeldActions() > rallyingCry.GetUnit().GetHeldActions()
                ? false
                : true;

        var colors = button.colors;
        if (!unitHasEnoughHeldActions)
        {
            colors.normalColor = Color.red;
        }
        else
        {
            colors.normalColor = Color.white;
        }
    }

    public void UpdateButton()
    {
        if (rallyingCryUsed)
        {
            var colors = button.colors;
            colors.normalColor = Color.grey;
            button.interactable = false;
        }
        else
        {
            CheckHeldActionRequirement(rallyingCry);
        }
    }

    public void UseRallyingCry()
    {
        OnChooseRallyingCry?.Invoke(this, rallyingCry);
        rallyingCryUsed = true;
    }
}
