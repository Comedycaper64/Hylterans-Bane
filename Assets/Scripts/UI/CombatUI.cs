using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    }

    private void OnDisable()
    {
        CombatSystem.Instance.OnAttackRoll -= CombatSystem_OnAttackRoll;
    }

    private void ToggleAttackUI(bool toggle)
    {
        attackRollText.enabled = toggle;
        defendingACText.enabled = toggle;
    }

    private IEnumerator TemporarilyDisplayAttackUI(int attackRoll, int defendingAC)
    {
        attackRollText.text = "Attack Roll: " + attackRoll;
        defendingACText.text = "Defender AC: " + defendingAC;
        ToggleAttackUI(true);
        yield return new WaitForSeconds(attackRollAppearanceTime);
        ToggleAttackUI(false);
    }

    private void CombatSystem_OnAttackRoll(object sender, AttackInteraction attackInteraction)
    {
        StartCoroutine(
            TemporarilyDisplayAttackUI(attackInteraction.attackRoll, attackInteraction.defendingAC)
        );
    }
}
