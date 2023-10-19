using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance;

    public event EventHandler<AttackInteraction> OnAttackInteraction;

    //public event EventHandler<AttackInteraction> OnSpellSave;
    private int critModifier = 2;

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

    // public bool TryAttack(Unit attackingUnit, Unit defendingUnit)
    // {
    //     //Attack role = d20 role + attacking stat + proficiency
    //     int attackingUnitAttackRoll = attackingUnit
    //         .GetUnitStats()
    //         .GetAttackRoll(out RollAugment rollAugment);
    //     int attackingUnitAttackBonus = attackingUnit.GetUnitStats().GetToHit();
    //     int defendingUnitAC = defendingUnit.GetUnitStats().GetArmourClass();
    //     //Debug.Log("Attack roll: " + attackingUnitAttackRoll);
    //     string attackRoll;
    //     if (rollAugment == RollAugment.Advantage)
    //     {
    //         attackRoll = "(Adv.) " + attackingUnitAttackRoll;
    //     }
    //     else if (rollAugment == RollAugment.Disadvantage)
    //     {
    //         attackRoll = "(Dis.) " + attackingUnitAttackRoll;
    //     }
    //     else
    //     {
    //         attackRoll = attackingUnitAttackRoll.ToString();
    //     }

    //     OnAttackRoll?.Invoke(
    //         this,
    //         new object[]
    //         {
    //             attackingUnit,
    //             defendingUnit,
    //             attackRoll,
    //             attackingUnitAttackBonus.ToString(),
    //             defendingUnitAC.ToString()
    //         }
    //     );
    //     return (attackingUnitAttackRoll + attackingUnitAttackBonus) >= defendingUnitAC;
    // }

    public AttackInteraction TryAttack(Unit attackingUnit, Unit defendingUnit)
    {
        UnitStats attackerStats = attackingUnit.GetUnitStats();
        UnitStats defenderStats = defendingUnit.GetUnitStats();
        int attackingUnitAttackRoll = attackerStats.GetAttackRoll(out RollAugment rollAugment);
        //Debug.Log("Attack Roll: " + attackingUnitAttackRoll);
        int attackingUnitAttackBonus = attackerStats.GetToHit();
        int defendingUnitAC = defenderStats.GetArmourClass();
        int damage = attackerStats.GetDamage();
        bool attackCrit = attackingUnitAttackRoll == 20;
        if (attackCrit)
        {
            damage *= critModifier;
        }

        AttackInteraction attackInteraction = new AttackInteraction(
            attackingUnit,
            defendingUnit,
            (attackingUnitAttackRoll + attackingUnitAttackBonus) >= defendingUnitAC,
            attackCrit,
            damage
        );
        OnAttackInteraction?.Invoke(this, attackInteraction);
        return attackInteraction;
    }

    // public bool TrySpell(Unit attackingUnit, Unit defendingUnit, StatType spellSave)
    // {
    //     int attackingUnitSpellDC = attackingUnit.GetUnitStats().GetAbilitySaveDC();
    //     int defendingUnitSavingThrowRoll = defendingUnit
    //         .GetUnitStats()
    //         .GetSavingThrowRoll(out RollAugment rollAugment);
    //     int defendingUnitSavingThrowBonus = defendingUnit.GetUnitStats().GetSavingThrow(spellSave);

    //     string defenseRoll;
    //     if (rollAugment == RollAugment.Advantage)
    //     {
    //         defenseRoll = "(Adv.) " + defendingUnitSavingThrowRoll;
    //     }
    //     else if (rollAugment == RollAugment.Disadvantage)
    //     {
    //         defenseRoll = "(Dis.) " + defendingUnitSavingThrowRoll;
    //     }
    //     else
    //     {
    //         defenseRoll = defendingUnitSavingThrowRoll.ToString();
    //     }

    //     OnSpellSave?.Invoke(
    //         this,
    //         new object[]
    //         {
    //             attackingUnit,
    //             defendingUnit,
    //             attackingUnitSpellDC.ToString(),
    //             defenseRoll,
    //             defendingUnitSavingThrowBonus.ToString()
    //         }
    //     );
    //     return (
    //         attackingUnitSpellDC > (defendingUnitSavingThrowRoll + defendingUnitSavingThrowBonus)
    //     );
    // }

    public AttackInteraction TrySpell(Unit attackingUnit, Unit defendingUnit, StatType spellSave)
    {
        UnitStats attackerStats = attackingUnit.GetUnitStats();
        UnitStats defenderStats = defendingUnit.GetUnitStats();
        int attackingUnitSpellDC = attackerStats.GetAbilitySaveDC();
        int defendingUnitSavingThrowRoll = defenderStats.GetSavingThrowRoll(
            out RollAugment rollAugment
        );
        int defendingUnitSavingThrowBonus = defenderStats.GetSavingThrow(spellSave);
        AttackInteraction attackInteraction = new AttackInteraction(
            attackingUnit,
            defendingUnit,
            attackingUnitSpellDC > (defendingUnitSavingThrowRoll + defendingUnitSavingThrowBonus),
            false,
            attackerStats.GetDamage()
        );
        OnAttackInteraction?.Invoke(this, attackInteraction);
        return attackInteraction;
    }
}
