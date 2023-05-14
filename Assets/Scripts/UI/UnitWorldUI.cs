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

        UpdateHealthBar(0);
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

        // if (healthBarImage)
        // {
        //     if (unit != UnitActionSystem.Instance.GetSelectedUnit()) { }
        // }
        // else if (healthBarImage)
        // {
        //     healthBarImage.fillAmount = healthSystem.GetHealthNormalized();
        // }
    }

    private void UpdateHealthBar(float damage)
    {
        healthBarImage.fillAmount = healthSystem.GetHealthNormalized();
        healthBarPredictedImage.fillAmount = healthSystem.GetHealthNormalized();

        //Still desirable functionality, just needs to be re-implemented
        // if (damage > 0f)
        // {
        //     ShowPredictedHealthLoss(damage);
        // }
    }

    private void HealthSystem_OnDamaged(object sender, float e)
    {
        UpdateHealthBar(e);
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, BaseAction baseAction)
    {
        if (unit.IsEnemy() && baseAction.ActionDealsDamage())
        {
            ShowPredictedHealthLoss(baseAction.GetUnit().GetUnitStats().GetDamage());
        }
        else
        {
            UpdateHealthBar(0);
        }
    }

    private void UnitActionSystem_OnUnitActionStarted()
    {
        UpdateHealthBar(0);
    }
}
