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

    private IEnumerator TemporarilyDisplayAttackUI(
        string attackRoll,
        string attackBonus,
        string defendingAC
    )
    {
        attackRollText.text = "Attack Roll: " + attackRoll + " + " + attackBonus;
        defendingACText.text = "Defender AC: " + defendingAC;
        ToggleAttackUI(true);
        yield return new WaitForSeconds(attackRollAppearanceTime);
        ToggleAttackUI(false);
    }

    private IEnumerator TemporarilyDisplaySpellUI(
        string spellSave,
        string defendingUnitSavingThrowRoll,
        string defendingUnitSavingThrowBonus
    )
    {
        attackRollText.text = "Spell Save DC: " + spellSave;
        defendingACText.text =
            "Defender Saving Throw: "
            + defendingUnitSavingThrowRoll
            + " + "
            + defendingUnitSavingThrowBonus;
        ToggleAttackUI(true);
        yield return new WaitForSeconds(attackRollAppearanceTime);
        ToggleAttackUI(false);
    }

    private void CombatSystem_OnAttackRoll(object sender, string[] attackInteraction)
    {
        StartCoroutine(
            TemporarilyDisplayAttackUI(
                attackInteraction[0],
                attackInteraction[1],
                attackInteraction[2]
            )
        );
    }

    private void CombatSystem_OnSpellSave(object sender, string[] spellInteraction)
    {
        StartCoroutine(
            TemporarilyDisplaySpellUI(spellInteraction[0], spellInteraction[1], spellInteraction[2])
        );
    }
}
