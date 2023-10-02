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
    private float combatResultAppearanceTime = 2f;

    //Logic for the healthbars and actionPoint trackers above a unit's head
    private void Start()
    {
        thisUnit = GetComponentInParent<Unit>();
        thisUnit.OnHeldActionsChanged += Unit_OnHeldActionsChanged;
        thisUnit.OnAOEAttack += Unit_OnAOEAttack;
        healthSystem.OnDamaged += HealthSystem_OnDamaged;
        CombatSystem.Instance.OnAttackRoll += CombatSystem_OnAttackRoll;
        CombatSystem.Instance.OnSpellSave += CombatSystem_OnSpellSave;
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
        CombatSystem.Instance.OnAttackRoll -= CombatSystem_OnAttackRoll;
        CombatSystem.Instance.OnSpellSave -= CombatSystem_OnSpellSave;
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

            yield return new WaitForSeconds(combatResultAppearanceTime);

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
            yield return new WaitForSeconds(combatResultAppearanceTime);

            aoeDamageText.text = "";
        }
        else
        {
            aoeDamageText.text =
                "To Hit: " + (attackInteraction.attackRoll + attackInteraction.attackBonus);

            yield return new WaitForSeconds(combatResultAppearanceTime);

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
            yield return new WaitForSeconds(combatResultAppearanceTime);

            aoeDamageText.text = "";
        }
    }

    private void ShowAttackerToHit(bool isSpell = false)
    {
        UnitStats thisUnitStats = thisUnit.GetUnitStats();
        if (isSpell)
        {
            aoeDamageText.text = "Spell Save DC: " + thisUnitStats.GetAbilitySaveDC();
        }
        else
        {
            aoeDamageText.text = "To Hit Bonus: " + thisUnitStats.GetToHit();
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
            aoeDamageText.text =
                "Save Bonus: " + savingThrow + "\nChance to hit: " + chanceToHit + "%";
        }
        else
        {
            int armourClass = thisUnitStats.GetArmourClass();
            aoeDamageText.text = "AC: " + armourClass + "\nChance to hit: " + chanceToHit + "%";
        }
    }

    private void ClearAttackUI()
    {
        aoeDamageText.text = "";
    }

    private IEnumerator TemporarilyDisplayAttackUI(
        bool attacker,
        string attackRoll = "",
        string attackBonus = "",
        string defendingAC = ""
    )
    {
        if (attacker)
        {
            aoeDamageText.text = "Attack Roll: " + attackRoll + " + " + attackBonus;
        }
        else
        {
            aoeDamageText.text = "Defender AC: " + defendingAC;
        }
        yield return new WaitForSeconds(combatResultAppearanceTime);
        ClearAttackUI();
    }

    private IEnumerator TemporarilyDisplaySpellUI(
        bool attacker,
        string spellSave = "",
        string defendingUnitSavingThrowRoll = "",
        string defendingUnitSavingThrowBonus = ""
    )
    {
        if (attacker)
        {
            aoeDamageText.text = "Spell Save DC: " + spellSave;
        }
        else
        {
            aoeDamageText.text =
                "Defender Saving Throw: "
                + defendingUnitSavingThrowRoll
                + " + "
                + defendingUnitSavingThrowBonus;
        }
        yield return new WaitForSeconds(combatResultAppearanceTime);
        ClearAttackUI();
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

    private void CombatSystem_OnAttackRoll(object sender, object[] attackInteraction)
    {
        if (thisUnit == attackInteraction[0] as Unit)
        {
            StartCoroutine(
                TemporarilyDisplayAttackUI(
                    true,
                    attackInteraction[2].ToString(),
                    attackInteraction[3].ToString()
                )
            );
        }
        else if (thisUnit == attackInteraction[1] as Unit)
        {
            StartCoroutine(
                TemporarilyDisplayAttackUI(false, "", "", attackInteraction[4].ToString())
            );
        }
    }

    private void CombatSystem_OnSpellSave(object sender, object[] spellInteraction)
    {
        if (thisUnit == spellInteraction[0] as Unit)
        {
            StartCoroutine(TemporarilyDisplaySpellUI(true, spellInteraction[2].ToString()));
        }
        else if (thisUnit == spellInteraction[1] as Unit)
        {
            StartCoroutine(
                TemporarilyDisplaySpellUI(
                    false,
                    "",
                    spellInteraction[3].ToString(),
                    spellInteraction[4].ToString()
                )
            );
        }
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, BaseAction baseAction)
    {
        if ((thisUnit == baseAction.GetUnit()) && baseAction.ActionDealsDamage())
        {
            ShowAttackerToHit(baseAction.IsSpell());
        }
        else if (thisUnit.IsEnemy() && baseAction.ActionDealsDamage())
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
