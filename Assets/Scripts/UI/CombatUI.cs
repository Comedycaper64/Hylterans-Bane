using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class CombatUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI attackRollText;

    [SerializeField]
    private TextMeshProUGUI defendingACText;

    [SerializeField]
    private float attackRollAppearanceTime = 3f;

    private void Start()
    {
        ToggleAttackUI(false);
        CombatSystem.Instance.OnAttackRoll += CombatSystem_OnAttackRoll;
        CombatSystem.Instance.OnSpellSave += CombatSystem_OnSpellSave;
    }

    private void OnDisable()
    {
        CombatSystem.Instance.OnAttackRoll -= CombatSystem_OnAttackRoll;
        CombatSystem.Instance.OnSpellSave -= CombatSystem_OnSpellSave;
    }

    private void ToggleAttackUI(bool toggle)
    {
        attackRollText.enabled = toggle;
        defendingACText.enabled = toggle;
    }

    private IEnumerator TemporarilyDisplayAttackUI(int attackRoll, int attackBonus, int defendingAC)
    {
        attackRollText.text =
            "Attack Roll: ("
            + attackRoll
            + " + "
            + attackBonus
            + ") = "
            + (attackRoll + attackBonus);
        defendingACText.text = "Defender AC: " + defendingAC;
        ToggleAttackUI(true);
        yield return new WaitForSeconds(attackRollAppearanceTime);
        ToggleAttackUI(false);
    }

    private IEnumerator TemporarilyDisplaySpellUI(
        int spellSave,
        int defendingUnitSavingThrowRoll,
        int defendingUnitSavingThrowBonus
    )
    {
        attackRollText.text = "Spell Save DC: " + spellSave;
        defendingACText.text =
            "Defender Saving Throw: ("
            + defendingUnitSavingThrowRoll
            + " + "
            + defendingUnitSavingThrowBonus
            + ") = "
            + (defendingUnitSavingThrowRoll + defendingUnitSavingThrowBonus);
        ToggleAttackUI(true);
        yield return new WaitForSeconds(attackRollAppearanceTime);
        ToggleAttackUI(false);
    }

    private void CombatSystem_OnAttackRoll(object sender, AttackInteraction attackInteraction)
    {
        StartCoroutine(
            TemporarilyDisplayAttackUI(
                attackInteraction.attackRoll,
                attackInteraction.attackBonus,
                attackInteraction.defenseRoll
            )
        );
    }

    private void CombatSystem_OnSpellSave(object sender, AttackInteraction attackInteraction)
    {
        StartCoroutine(
            TemporarilyDisplaySpellUI(
                attackInteraction.attackRoll,
                attackInteraction.defenseRoll,
                attackInteraction.defenseBonus
            )
        );
    }
}
