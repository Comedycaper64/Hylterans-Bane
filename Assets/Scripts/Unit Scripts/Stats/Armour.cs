using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewArmour", menuName = "The Seventy Year War/Armour", order = 2)]
public class Armour : ScriptableObject
{
    [SerializeField]
    private string armourName;

    [SerializeField]
    private int armourBonus;

    [SerializeField]
    private int dexBonusLimit;

    public string GetArmourName()
    {
        return armourName;
    }

    public int GetArmourBonus()
    {
        return armourBonus;
    }

    public int GetDexBonusLimit()
    {
        return dexBonusLimit;
    }
}
