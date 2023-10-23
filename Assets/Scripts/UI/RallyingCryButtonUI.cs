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
    private Image buttonImage;

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

    [SerializeField]
    private Color availableColour;

    [SerializeField]
    private Color unavailableColour;

    [SerializeField]
    private Color usedColour;

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
        unitHasEnoughHeldActions = UnitHasRequiredSpirit(rallyingCry);
        //var colors = button.colors;
        if (!unitHasEnoughHeldActions)
        {
            buttonImage.color = unavailableColour;
        }
        else
        {
            buttonImage.color = availableColour;
        }
    }

    public void UpdateButton()
    {
        if (rallyingCryUsed)
        {
            //var colors = button.colors;
            buttonImage.color = usedColour;
            button.interactable = false;
        }
        else
        {
            CheckHeldActionRequirement(rallyingCry);
        }
    }

    public void UseRallyingCry()
    {
        if (UnitHasRequiredSpirit(rallyingCry))
        {
            OnChooseRallyingCry?.Invoke(this, rallyingCry);
            rallyingCryUsed = true;
        }
        else
        {
            //Unit is overexerted popup
        }
    }

    private bool UnitHasRequiredSpirit(RallyingCry rallyingCry)
    {
        return
            rallyingCry.GetUnit().GetSpiritSystem().GetSpirit() >= rallyingCry.GetRequiredSpirit()
            ? true
            : false;
    }
}
