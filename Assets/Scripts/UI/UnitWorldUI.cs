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

        UpdateHealthBar(0);
    }

    private void OnDisable()
    {
        healthSystem.OnDamaged -= HealthSystem_OnDamaged;
    }

    private void ShowPredictedHealthLoss(float damage)
    {
        if (!UnitActionSystem.Instance.GetSelectedAction())
        {
            return;
        }

        if (
            healthBarImage
            && (unit.IsEnemy() || UnitActionSystem.Instance.GetSelectedAction().GetIsAOE())
        )
        {
            if (unit != UnitActionSystem.Instance.GetSelectedUnit())
            {
                healthBarImage.fillAmount = healthSystem.GetAmountNormalised(
                    healthSystem.GetHealth() - (damage)
                );
            }
        }
        else if (healthBarImage)
        {
            healthBarImage.fillAmount = healthSystem.GetHealthNormalized();
        }
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

    private void UnitActionSystem_OnSelectedActionChanged(object sender, BaseAction e)
    {
        if (unit.IsEnemy())
            ShowPredictedHealthLoss(e.GetDamage());
        else
            ShowPredictedHealthLoss(e.GetDamage() * 2);
    }
}
