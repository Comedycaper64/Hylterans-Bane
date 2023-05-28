using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackInteraction
{
    public AttackInteraction(int attack, int defense)
    {
        attackingValue = attack;
        defendingValue = defense;
    }

    public int attackingValue;
    public int defendingValue;
}
