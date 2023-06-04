using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackInteraction
{
    public AttackInteraction(int attackRoll, int attackBonus, int defenseRoll, int defenseBonus)
    {
        this.attackRoll = attackRoll;
        this.attackBonus = attackBonus;
        this.defenseRoll = defenseRoll;
        this.defenseBonus = defenseBonus;
    }

    public int attackRoll;
    public int attackBonus;
    public int defenseRoll;
    public int defenseBonus;
}
