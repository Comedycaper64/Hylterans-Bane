using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitWorldUI : MonoBehaviour
{

    //[SerializeField] private TextMeshProUGUI actionPointsText;
    //[SerializeField] private Transform actionPointContainer;
    //[SerializeField] private Transform actionPointPrefab;
    [SerializeField] private Unit unit;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private Image healthBarPredictedImage;
    //[SerializeField] private MeshRenderer tauntedUI;
    [SerializeField] private HealthSystem healthSystem;

    //Logic for the healthbars and actionPoint trackers above a unit's head
    private void Start()
    {
        //Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
        //Unit.OnTaunted += Unit_OnTaunted;
        //Unit.OnTauntRemoved += Unit_OnTauntRemoved;
        //UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        healthSystem.OnDamaged += HealthSystem_OnDamaged;

        //UpdateActionPointsText();
        UpdateHealthBar(0);
    }
    

    private void UnitActionSystem_OnSelectedActionChanged(object sender, BaseAction e)
    {
        if (unit.IsEnemy())
            ShowPredictedHealthLoss(e.GetDamage());
        else
            ShowPredictedHealthLoss(e.GetDamage() * 2);
    }

    private void ShowPredictedHealthLoss(float damage)
    {
        if (!UnitActionSystem.Instance.GetSelectedAction()) {return;}

        if (healthBarImage && (unit.IsEnemy() || UnitActionSystem.Instance.GetSelectedAction().GetIsAOE()))
        {
            if (unit != UnitActionSystem.Instance.GetSelectedUnit())
            {
                healthBarImage.fillAmount = healthSystem.GetAmountNormalised(healthSystem.GetHealth() - (damage));
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

        if (damage > 0f)
        {
            ShowPredictedHealthLoss(damage);
        }
    }

    private void HealthSystem_OnDamaged(object sender, float e)
    {
        UpdateHealthBar(e);
    }

    private void OnDisable() 
    {
        
    }

    // public void ClearPoints()
    // {
    //     if (!actionPointContainer) {return;}
    //     foreach (Transform actionPoint in actionPointContainer)
    //     {
    //         Destroy(actionPoint.gameObject);
    //     }
    // }

    // public void CreateActionPoints(int actionPoints)
    // {
    //     ClearPoints();
    //     for (int i = 0; i < actionPoints; i++)
    //     {
    //         Transform actionPointTransform;
    //         if (actionPointPrefab && actionPointContainer)
    //             actionPointTransform = Instantiate(actionPointPrefab, actionPointContainer);
    //     }
    // }

    // private void UpdateActionPointsText()
    // {
    //     if (unit)
    //         CreateActionPoints(unit.GetActionPoints());
    // }

    // private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    // {
    //     UpdateActionPointsText();
    // }

    // private void Unit_OnTaunted(object sender, EventArgs e)
    // {
    //     if (unit.HasFocusTargetUnit() && tauntedUI)
    //         tauntedUI.enabled = true;
    // }

    // private void Unit_OnTauntRemoved(object sender, EventArgs e)
    // {
    //     Unit unit = sender as Unit;
    //     if (this.unit == unit && tauntedUI)
    //     {
    //         tauntedUI.enabled = false;
    //     }
    // }

}
