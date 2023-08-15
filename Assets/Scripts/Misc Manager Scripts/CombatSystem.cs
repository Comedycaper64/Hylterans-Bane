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

    public BattleForecast GetBattleForecast(
        Unit attackingUnit,
        Unit defendingUnit,
        BaseAction currentAction
    )
    {
        BattleForecast currentBattleForecast = new BattleForecast();
        if (currentAction.IsSpell())
        {
            currentBattleForecast.attackingUnitChanceToHit = Mathf.Min(
                100,
                Mathf.RoundToInt(
                    100
                        * (
                            (
                                attackingUnit.GetUnitStats().GetSpellDC()
                                - defendingUnit
                                    .GetUnitStats()
                                    .GetSavingThrow(currentAction.SpellSave())
                            ) / 20f
                        )
                )
            );
        }
        else
        {
            currentBattleForecast.attackingUnitChanceToHit = Mathf.Min(
                100,
                Mathf.RoundToInt(
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
                )
            );
        }

        currentBattleForecast.attackingUnitDamage = attackingUnit.GetUnitStats().GetDamage();

        return currentBattleForecast;
    }

    public bool TryAttack(UnitStats attackingUnit, UnitStats defendingUnit)
    {
        //Attack role = d20 role + attacking stat + proficiency
        int attackingUnitAttackRoll = attackingUnit.GetRoll();
        int attackingUnitAttackBonus = attackingUnit.GetToHit();
        int defendingUnitAC = defendingUnit.GetArmourClass();
        //Debug.Log("Attack roll: " + attackingUnitAttackRoll);
        OnAttackRoll?.Invoke(
            this,
            new AttackInteraction(
                attackingUnitAttackRoll,
                attackingUnitAttackBonus,
                defendingUnitAC,
                0,
                false
            )
        );
        return ((attackingUnitAttackRoll + attackingUnitAttackBonus) >= defendingUnitAC);
    }

    public bool TryAttack(
        UnitStats attackingUnit,
        UnitStats defendingUnit,
        out AttackInteraction attackInteraction
    )
    {
        //Attack role = d20 role + attacking stat + proficiency
        int attackingUnitAttackRoll = attackingUnit.GetRoll();
        int attackingUnitAttackBonus = attackingUnit.GetToHit();
        int defendingUnitAC = defendingUnit.GetArmourClass();
        //Debug.Log("Attack roll: " + attackingUnitAttackRoll);
        attackInteraction = new AttackInteraction(
            attackingUnitAttackRoll,
            attackingUnitAttackBonus,
            defendingUnitAC,
            0,
            false
        );
        //OnAttackRoll?.Invoke(this, attackInteraction);
        return ((attackingUnitAttackRoll + attackingUnitAttackBonus) >= defendingUnitAC);
    }

    public bool TrySpell(UnitStats attackingUnit, UnitStats defendingUnit, StatType spellSave)
    {
        int attackingUnitSpellDC = attackingUnit.GetSpellDC();
        int defendingUnitSavingThrowRoll = defendingUnit.GetRoll();
        int defendingUnitSavingThrowBonus = defendingUnit.GetSavingThrow(spellSave);

        OnSpellSave?.Invoke(
            this,
            new AttackInteraction(
                attackingUnitSpellDC,
                0,
                defendingUnitSavingThrowRoll,
                defendingUnitSavingThrowBonus,
                true
            )
        );
        return (
            attackingUnitSpellDC > (defendingUnitSavingThrowRoll + defendingUnitSavingThrowBonus)
        );
    }

    public bool TrySpell(
        UnitStats attackingUnit,
        UnitStats defendingUnit,
        StatType spellSave,
        out AttackInteraction attackInteraction
    )
    {
        int attackingUnitSpellDC = attackingUnit.GetSpellDC();
        int defendingUnitSavingThrowRoll = defendingUnit.GetRoll();
        int defendingUnitSavingThrowBonus = defendingUnit.GetSavingThrow(spellSave);
        attackInteraction = new AttackInteraction(
            attackingUnitSpellDC,
            0,
            defendingUnitSavingThrowRoll,
            defendingUnitSavingThrowBonus,
            true
        );
        //OnSpellSave?.Invoke(this, attackInteraction);
        return (
            attackingUnitSpellDC > (defendingUnitSavingThrowRoll + defendingUnitSavingThrowBonus)
        );
    }
}
