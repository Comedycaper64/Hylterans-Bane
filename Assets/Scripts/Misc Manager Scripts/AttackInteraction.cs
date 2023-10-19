using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackInteraction
{
    public Unit attacker;
    public Unit defender;
    public bool attackHit;
    public bool attackCrit;
    public int attackDamage;

    public AttackInteraction(
        Unit attacker,
        Unit defender,
        bool attackHit = false,
        bool attackCrit = false,
        int attackDamage = 0
    )
    {
        this.attacker = attacker;
        this.defender = defender;
        this.attackHit = attackHit;
        this.attackCrit = attackCrit;
        this.attackDamage = attackDamage;
    }

    // public AttackInteraction(
    //     int attackRoll,
    //     int attackBonus,
    //     int defenseRoll,
    //     int defenseBonus,
    //     bool spellAttack
    // )
    // {
    //     this.attackRoll = attackRoll;
    //     this.attackBonus = attackBonus;
    //     this.defenseRoll = defenseRoll;
    //     this.defenseBonus = defenseBonus;
    //     this.spellAttack = spellAttack;
    // }

    // public int attackRoll;
    // public int attackBonus;
    // public int defenseRoll;
    // public int defenseBonus;
    // public bool spellAttack = false;
}
