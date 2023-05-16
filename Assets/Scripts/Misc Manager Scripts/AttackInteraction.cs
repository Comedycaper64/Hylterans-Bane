using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackInteraction
{
    public AttackInteraction(int attack, int armourClass)
    {
        attackRoll = attack;
        defendingAC = armourClass;
    }

    public int attackRoll;
    public int defendingAC;
}
