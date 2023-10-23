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

    //[SerializeField]
    private HealthSystem healthSystem;

    //[SerializeField]
    private SpiritSystem spiritSystem;

    [SerializeField]
    private TextMeshProUGUI heldActionText;

    [SerializeField]
    private TextMeshProUGUI aoeDamageText;

    [SerializeField]
    private Transform attackTextStack;

    [SerializeField]
    private GameObject attackTextPrefab;

    //private float combatResultAppearanceTime = 2f;

    //Logic for the healthbars and actionPoint trackers above a unit's head
    private void Start()
    {
        thisUnit = GetComponentInParent<Unit>();
        spiritSystem = thisUnit.GetSpiritSystem();
        healthSystem = GetComponentInParent<HealthSystem>();
        spiritSystem.OnSpiritChanged += Unit_OnHeldActionsChanged;
        //thisUnit.OnAOEAttack += Unit_OnAOEAttack;
        healthSystem.OnDamaged += HealthSystem_OnDamaged;
        CombatSystem.Instance.OnAttackInteraction += CombatSystem_OnAttackRoll;
        //CombatSystem.Instance.OnSpellSave += CombatSystem_OnSpellSave;
        UnitActionSystem.Instance.OnSelectedActionChanged +=
            UnitActionSystem_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnUnitActionStarted += UnitActionSystem_OnUnitActionStarted;

        aoeDamageText.text = "";

        UpdateHealthBar();
    }

    private void OnDisable()
    {
        spiritSystem.OnSpiritChanged -= Unit_OnHeldActionsChanged;
        //thisUnit.OnAOEAttack -= Unit_OnAOEAttack;
        healthSystem.OnDamaged -= HealthSystem_OnDamaged;
        CombatSystem.Instance.OnAttackInteraction -= CombatSystem_OnAttackRoll;
        //CombatSystem.Instance.OnSpellSave -= CombatSystem_OnSpellSave;
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

    private void ShowAOEAttackResult(AttackInteraction attackInteraction)
    {
        TemporarilyDisplayAttackUI(
            attackInteraction.attackHit,
            attackInteraction.attackCrit,
            attackInteraction.attackDamage
        );
        // string attackText;
        // Color textColour;

        // if (attackInteraction.spellAttack)
        // {
        //     attackText =
        //         "Save: " + (attackInteraction.defenseRoll + attackInteraction.defenseBonus);

        //     if (
        //         (attackInteraction.defenseRoll + attackInteraction.defenseBonus)
        //         >= attackInteraction.attackRoll
        //     )
        //     {
        //         stateText = "Saved!";
        //     }
        //     else
        //     {
        //         stateText = "Failed.";
        //     }
        // }
        // else
        // {
        //     attackText =
        //         "Attack: " + (attackInteraction.attackRoll + attackInteraction.attackBonus);

        //     if (
        //         (attackInteraction.attackRoll + attackInteraction.attackBonus)
        //         >= attackInteraction.defenseRoll
        //     )
        //     {
        //         stateText = "Hit!";
        //     }
        //     else
        //     {
        //         stateText = "Missed.";
        //     }
        // }
        // UnitAttackTextUI unitAttackTextUI = Instantiate(attackTextPrefab, attackTextStack)
        //     .GetComponent<UnitAttackTextUI>();
        // unitAttackTextUI.SetupText(attackText, textColour);
    }

    private void ShowAttackerToHit(bool isSpell = false)
    {
        UnitStats thisUnitStats = thisUnit.GetUnitStats();
        if (isSpell)
        {
            aoeDamageText.text = "Save DC: " + thisUnitStats.GetAbilitySaveDC();
        }
        else
        {
            aoeDamageText.text = "To Hit: +" + thisUnitStats.GetToHit();
        }
    }

    private void ShowChanceToHit(
        string chanceToHit,
        bool isSpell = false,
        StatType saveStat = StatType.DEX
    )
    {
        UnitStats thisUnitStats = thisUnit.GetUnitStats();
        if (isSpell)
        {
            int savingThrow = thisUnitStats.GetSavingThrow(saveStat);
            aoeDamageText.text = "Save: +" + savingThrow + "\nHit: " + chanceToHit + "%";
        }
        else
        {
            int armourClass = thisUnitStats.GetArmourClass();
            aoeDamageText.text = "AC: " + armourClass + "\nHit: " + chanceToHit + "%";
        }
    }

    private void ClearAttackUI()
    {
        aoeDamageText.text = "";
    }

    private void TemporarilyDisplayAttackUI(bool attackHit, bool attackCrit, int attackDamage)
    {
        string attackText;
        Color textColour;
        if (attackCrit)
        {
            attackText = attackDamage.ToString();
            textColour = Color.yellow;
        }
        else if (attackHit)
        {
            attackText = attackDamage.ToString();
            textColour = Color.red;
        }
        else
        {
            attackText = "Miss";
            textColour = Color.white;
        }
        UnitAttackTextUI unitAttackTextUI = Instantiate(attackTextPrefab, attackTextStack)
            .GetComponent<UnitAttackTextUI>();
        unitAttackTextUI.SetupText(attackText, textColour);
    }

    // private void TemporarilyDisplaySpellUI(bool spellHit, int spellDamage)
    // {
    //     string attackText;
    //     Color textColour;
    //     if (spellHit)
    //     {
    //         attackText = spellDamage.ToString();
    //         textColour = Color.red;
    //     }
    //     else
    //     {
    //         attackText = "Miss";
    //         textColour = Color.white;
    //     }
    //     UnitAttackTextUI unitAttackTextUI = Instantiate(attackTextPrefab, attackTextStack)
    //         .GetComponent<UnitAttackTextUI>();
    //     unitAttackTextUI.SetupText(attackText, textColour);
    // }

    private void Unit_OnHeldActionsChanged(object sender, int newHeldActions)
    {
        UpdateHeldActionsText(newHeldActions);
    }

    // private void Unit_OnAOEAttack(object sender, AttackInteraction attackInteraction)
    // {
    //     ShowAOEAttackResult(attackInteraction);
    // }

    private void HealthSystem_OnDamaged(object sender, float e)
    {
        UpdateHealthBar();
    }

    private void CombatSystem_OnAttackRoll(object sender, AttackInteraction attackInteraction)
    {
        if (thisUnit == attackInteraction.defender)
        {
            TemporarilyDisplayAttackUI(
                attackInteraction.attackHit,
                attackInteraction.attackCrit,
                attackInteraction.attackDamage
            );
        }
    }

    // private void CombatSystem_OnSpellSave(object sender, AttackInteraction spellInteraction)
    // {
    //     if (thisUnit == spellInteraction.defender)
    //     {
    //         TemporarilyDisplaySpellUI(spellInteraction.attackHit, spellInteraction.attackDamage);
    //     }
    // }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, BaseAction baseAction)
    {
        (int, int) actionRange = baseAction.GetActionRange();
        GridPosition gridPositionDistance =
            thisUnit.GetGridPosition() - baseAction.GetUnit().GetGridPosition();
        int testDistance = Mathf.Abs(gridPositionDistance.x) + Mathf.Abs(gridPositionDistance.z);
        if ((thisUnit == baseAction.GetUnit()) && baseAction.ActionDealsDamage())
        {
            ShowAttackerToHit(baseAction.IsSpell());
        }
        else if (
            (thisUnit.IsEnemy() || baseAction.IsFriendlyFire())
            && baseAction.ActionDealsDamage()
            && (
                ((testDistance <= actionRange.Item2) && (testDistance >= actionRange.Item1))
                || baseAction.GetIsAOE()
            )
        )
        {
            ShowPredictedHealthLoss(baseAction.GetUnit().GetUnitStats().GetDamage());
            BattleForecast unitBattleForecast = CombatSystem.Instance.GetBattleForecast(
                baseAction.GetUnit(),
                thisUnit,
                baseAction
            );
            ShowChanceToHit(
                unitBattleForecast.attackingUnitChanceToHit.ToString(),
                baseAction.IsSpell(),
                baseAction.SpellSave()
            );
        }
        else
        {
            UpdateHealthBar();
            aoeDamageText.text = "";
        }
    }

    private void UnitActionSystem_OnUnitActionStarted()
    {
        UpdateHealthBar();
        aoeDamageText.text = "";
    }
}
