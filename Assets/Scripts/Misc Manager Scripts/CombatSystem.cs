using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one CombatSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public Queue<CombatInteraction> SimulateCombat(Unit attackingUnit, Unit defendingUnit)
    {
        CombatInteraction newInteraction = new CombatInteraction();
        newInteraction.attackHit = TryAttack(attackingUnit, defendingUnit);
        newInteraction.attackDamage = CalculateUnitDamage(attackingUnit);
        //Check if defending unit is dead, then add interaction into queue

        return new Queue<CombatInteraction>();
    }

    private bool TryAttack(Unit attackingUnit, Unit defendingUnit)
    {
        //Attack role = d20 role + attacking stat + proficiency
        int attackingUnitAttackRole =
            Random.Range(0, 21)
            + Mathf.FloorToInt((attackingUnit.GetUnitStats().GetStrength() - 10) / 2);
        int defendingUnitAC =
            10 + Mathf.FloorToInt((defendingUnit.GetUnitStats().GetDexterity() - 10) / 2);
        return (attackingUnitAttackRole >= defendingUnitAC);
    }

    private int CalculateUnitDamage(Unit attackingUnit)
    {
        throw new System.NotImplementedException();
    }
}
