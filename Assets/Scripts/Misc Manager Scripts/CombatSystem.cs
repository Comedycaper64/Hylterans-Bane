using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance;

    public event EventHandler<AttackInteraction> OnAttackRoll;
    public event EventHandler<AttackInteraction> OnSpellSave;

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

    // public Queue<CombatInteraction> SimulateCombat(Unit attackingUnit, Unit defendingUnit)
    // {
    //     CombatInteraction newInteraction = new CombatInteraction();
    //     newInteraction.attackHit = TryAttack(attackingUnit, defendingUnit);
    //     newInteraction.attackDamage = CalculateUnitDamage(attackingUnit);
    //     //Check if defending unit is dead, then add interaction into queue

    //     return new Queue<CombatInteraction>();
    // }

    public BattleForecast GetBattleForecast(Unit attackingUnit, Unit defendingUnit)
    {
        BattleForecast currentBattleForecast = new BattleForecast();
        currentBattleForecast.attackingUnitChanceToHit = Mathf.RoundToInt(
            100
                * (
                    1
                    - (
                        (
                            defendingUnit.GetUnitStats().GetArmourClass()
                            - attackingUnit.GetUnitStats().GetToHit()
                        ) / 20f
                    )
                )
        );
        currentBattleForecast.attackingUnitDamage = attackingUnit.GetUnitStats().GetDamage();

        return currentBattleForecast;
    }

    public bool TryAttack(UnitStats attackingUnit, UnitStats defendingUnit)
    {
        //Attack role = d20 role + attacking stat + proficiency
        int attackingUnitAttackRoll = attackingUnit.GetAttackRoll();
        int defendingUnitAC = defendingUnit.GetArmourClass();
        //Debug.Log("Attack roll: " + attackingUnitAttackRoll);
        OnAttackRoll?.Invoke(this, new AttackInteraction(attackingUnitAttackRoll, defendingUnitAC));
        return (attackingUnitAttackRoll >= defendingUnitAC);
    }

    public bool TrySpell(UnitStats attackingUnit, UnitStats defendingUnit)
    {
        int attackingUnitSpellDC = attackingUnit.GetSpellDC();
        int defendingUnitSavingThrow = defendingUnit.GetSavingThrow(StatType.DEX);
        OnSpellSave?.Invoke(
            this,
            new AttackInteraction(attackingUnitSpellDC, defendingUnitSavingThrow)
        );
        return (attackingUnitSpellDC > defendingUnitSavingThrow);
    }

    private int CalculateUnitDamage(Unit attackingUnit)
    {
        throw new System.NotImplementedException();
    }
}
