using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitWorldUI : MonoBehaviour
{
    private Unit thisUnit;

    [SerializeField]
    private Image healthBarImage;

    [SerializeField]
    private Image healthBarPredictedImage;

    [SerializeField]
    private HealthSystem healthSystem;

    [SerializeField]
    private TextMeshProUGUI heldActionText;

    //Logic for the healthbars and actionPoint trackers above a unit's head
    private void Start()
    {
        thisUnit = GetComponentInParent<Unit>();
        thisUnit.OnHeldActionsChanged += Unit_OnHeldActionsChanged;
        healthSystem.OnDamaged += HealthSystem_OnDamaged;
        UnitActionSystem.Instance.OnSelectedActionChanged +=
            UnitActionSystem_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnUnitActionStarted += UnitActionSystem_OnUnitActionStarted;

        UpdateHealthBar();
    }

    private void OnDisable()
    {
        thisUnit.OnHeldActionsChanged -= Unit_OnHeldActionsChanged;
        healthSystem.OnDamaged -= HealthSystem_OnDamaged;
        UnitActionSystem.Instance.OnSelectedActionChanged -=
            UnitActionSystem_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnUnitActionStarted -= UnitActionSystem_OnUnitActionStarted;
    }

    private void ShowPredictedHealthLoss(float damage)
    {
        healthBarImage.fillAmount = healthSystem.GetAmountNormalised(
            healthSystem.GetHealth() - (damage)
        );
    }

    private void UpdateHealthBar()
    {
        healthBarImage.fillAmount = healthSystem.GetHealthNormalized();
        healthBarPredictedImage.fillAmount = healthSystem.GetHealthNormalized();
    }

    private void UpdateHeldActionsText(int newHeldActions)
    {
        heldActionText.text = newHeldActions.ToString();
    }

    private void Unit_OnHeldActionsChanged(object sender, int newHeldActions)
    {
        UpdateHeldActionsText(newHeldActions);
    }

    private void HealthSystem_OnDamaged(object sender, float e)
    {
        UpdateHealthBar();
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, BaseAction baseAction)
    {
        if (thisUnit.IsEnemy() && baseAction.ActionDealsDamage())
        {
            ShowPredictedHealthLoss(thisUnit.GetUnitStats().GetDamage());
        }
        else
        {
            UpdateHealthBar();
        }
    }

    private void UnitActionSystem_OnUnitActionStarted()
    {
        UpdateHealthBar();
    }
}
