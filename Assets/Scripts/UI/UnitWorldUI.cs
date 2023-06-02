using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitWorldUI : MonoBehaviour
{
    [SerializeField]
    private Unit unit;

    [SerializeField]
    private Image healthBarImage;

    [SerializeField]
    private Image healthBarPredictedImage;

    [SerializeField]
    private HealthSystem healthSystem;

    //Logic for the healthbars and actionPoint trackers above a unit's head
    private void Start()
    {
        healthSystem.OnDamaged += HealthSystem_OnDamaged;
        UnitActionSystem.Instance.OnSelectedActionChanged +=
            UnitActionSystem_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnUnitActionStarted += UnitActionSystem_OnUnitActionStarted;

        UpdateHealthBar();
    }

    private void OnDisable()
    {
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

    private void HealthSystem_OnDamaged(object sender, float e)
    {
        UpdateHealthBar();
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, BaseAction baseAction)
    {
        if (unit.IsEnemy() && baseAction.ActionDealsDamage())
        {
            ShowPredictedHealthLoss(baseAction.GetDamage());
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
