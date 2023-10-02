using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance;

    public event EventHandler<object[]> OnAttackRoll;
    public event EventHandler<object[]> OnSpellSave;

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
                                attackingUnit.GetUnitStats().GetAbilitySaveDC()
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

    public bool TryAttack(Unit attackingUnit, Unit defendingUnit)
    {
        //Attack role = d20 role + attacking stat + proficiency
        int attackingUnitAttackRoll = attackingUnit
            .GetUnitStats()
            .GetAttackRoll(out RollAugment rollAugment);
        int attackingUnitAttackBonus = attackingUnit.GetUnitStats().GetToHit();
        int defendingUnitAC = defendingUnit.GetUnitStats().GetArmourClass();
        //Debug.Log("Attack roll: " + attackingUnitAttackRoll);
        string attackRoll;
        if (rollAugment == RollAugment.Advantage)
        {
            attackRoll = "(Adv.) " + attackingUnitAttackRoll;
        }
        else if (rollAugment == RollAugment.Disadvantage)
        {
            attackRoll = "(Dis.) " + attackingUnitAttackRoll;
        }
        else
        {
            attackRoll = attackingUnitAttackRoll.ToString();
        }

        OnAttackRoll?.Invoke(
            this,
            new object[]
            {
                attackingUnit,
                defendingUnit,
                attackRoll,
                attackingUnitAttackBonus.ToString(),
                defendingUnitAC.ToString()
            }
        );
        return (attackingUnitAttackRoll + attackingUnitAttackBonus) >= defendingUnitAC;
    }

    public bool TryAttack(
        UnitStats attackingUnit,
        UnitStats defendingUnit,
        out AttackInteraction attackInteraction
    )
    {
        //Attack role = d20 role + attacking stat + proficiency
        int attackingUnitAttackRoll = attackingUnit.GetAttackRoll(out RollAugment rollAugment);
        int attackingUnitAttackBonus = attackingUnit.GetToHit();
        int defendingUnitAC = defendingUnit.GetArmourClass();

        attackInteraction = new AttackInteraction(
            attackingUnitAttackRoll,
            attackingUnitAttackBonus,
            defendingUnitAC,
            0,
            false
        );
        //OnAttackRoll?.Invoke(this, attackInteraction);
        return (attackingUnitAttackRoll + attackingUnitAttackBonus) >= defendingUnitAC;
    }

    public bool TrySpell(Unit attackingUnit, Unit defendingUnit, StatType spellSave)
    {
        int attackingUnitSpellDC = attackingUnit.GetUnitStats().GetAbilitySaveDC();
        int defendingUnitSavingThrowRoll = defendingUnit
            .GetUnitStats()
            .GetSavingThrowRoll(out RollAugment rollAugment);
        int defendingUnitSavingThrowBonus = defendingUnit.GetUnitStats().GetSavingThrow(spellSave);

        string defenseRoll;
        if (rollAugment == RollAugment.Advantage)
        {
            defenseRoll = "(Adv.) " + defendingUnitSavingThrowRoll;
        }
        else if (rollAugment == RollAugment.Disadvantage)
        {
            defenseRoll = "(Dis.) " + defendingUnitSavingThrowRoll;
        }
        else
        {
            defenseRoll = defendingUnitSavingThrowRoll.ToString();
        }

        OnSpellSave?.Invoke(
            this,
            new object[]
            {
                attackingUnit,
                defendingUnit,
                attackingUnitSpellDC.ToString(),
                defenseRoll,
                defendingUnitSavingThrowBonus.ToString()
            }
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
        int attackingUnitSpellDC = attackingUnit.GetAbilitySaveDC();
        int defendingUnitSavingThrowRoll = defendingUnit.GetSavingThrowRoll(
            out RollAugment rollAugment
        );
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
