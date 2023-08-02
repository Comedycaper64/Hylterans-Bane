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

    [SerializeField]
    private TextMeshProUGUI aoeDamageText;

    //Logic for the healthbars and actionPoint trackers above a unit's head
    private void Start()
    {
        thisUnit = GetComponentInParent<Unit>();
        thisUnit.OnHeldActionsChanged += Unit_OnHeldActionsChanged;
        thisUnit.OnAOEAttack += Unit_OnAOEAttack;
        healthSystem.OnDamaged += HealthSystem_OnDamaged;
        UnitActionSystem.Instance.OnSelectedActionChanged +=
            UnitActionSystem_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnUnitActionStarted += UnitActionSystem_OnUnitActionStarted;

        aoeDamageText.text = "";

        UpdateHealthBar();
    }

    private void OnDisable()
    {
        thisUnit.OnHeldActionsChanged -= Unit_OnHeldActionsChanged;
        thisUnit.OnAOEAttack -= Unit_OnAOEAttack;
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

    private IEnumerator ShowAOEAttackResult(AttackInteraction attackInteraction)
    {
        if (attackInteraction.spellAttack)
        {
            aoeDamageText.text =
                "Save: " + (attackInteraction.defenseRoll + attackInteraction.defenseBonus);

            yield return new WaitForSeconds(1f);

            if (
                (attackInteraction.defenseRoll + attackInteraction.defenseBonus)
                >= attackInteraction.attackRoll
            )
            {
                aoeDamageText.text = "Saved!";
            }
            else
            {
                aoeDamageText.text = "Failed.";
            }
            yield return new WaitForSeconds(1f);

            aoeDamageText.text = "";
        }
        else
        {
            aoeDamageText.text =
                "To Hit: " + (attackInteraction.attackRoll + attackInteraction.attackBonus);

            yield return new WaitForSeconds(1f);

            if (
                (attackInteraction.attackRoll + attackInteraction.attackBonus)
                >= attackInteraction.defenseRoll
            )
            {
                aoeDamageText.text = "Attack Hit!";
            }
            else
            {
                aoeDamageText.text = "Attack Missed.";
            }
            yield return new WaitForSeconds(1f);

            aoeDamageText.text = "";
        }
    }

    private void Unit_OnHeldActionsChanged(object sender, int newHeldActions)
    {
        UpdateHeldActionsText(newHeldActions);
    }

    private void Unit_OnAOEAttack(object sender, AttackInteraction attackInteraction)
    {
        StartCoroutine(ShowAOEAttackResult(attackInteraction));
    }

    private void HealthSystem_OnDamaged(object sender, float e)
    {
        UpdateHealthBar();
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, BaseAction baseAction)
    {
        if (thisUnit.IsEnemy() && baseAction.ActionDealsDamage())
        {
            ShowPredictedHealthLoss(baseAction.GetUnit().GetUnitStats().GetDamage());
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
