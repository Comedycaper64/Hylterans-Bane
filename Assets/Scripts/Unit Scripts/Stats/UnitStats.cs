using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitStats", menuName = "The Seventy Year War/UnitStats", order = 0)]
public class UnitStats : ScriptableObject
{
    [Header("Main Stats")]
    [SerializeField]
    private int strength = 10;

    [SerializeField]
    private int dexterity = 10;

    [SerializeField]
    private int constitution = 10;

    [SerializeField]
    private int intelligence = 10;

    [SerializeField]
    private int wisdom = 10;

    [SerializeField]
    private int charisma = 10;

    [Header("Other")]
    [SerializeField]
    private int proficiencyBonus = 2;

    [SerializeField]
    private int weaponDamage = 5;

    [SerializeField]
    private int unitHealth = 10;

    public int GetToHit()
    {
        int attackingStat = GetStrength();
        return Mathf.FloorToInt((attackingStat - 10) / 2) + proficiencyBonus;
    }

    public int GetAttackRoll()
    {
        return Random.Range(1, 21) + GetToHit();
    }

    public int GetArmourClass()
    {
        int unitArmour = 0;
        return 10 + Mathf.FloorToInt((GetDexterity() - 10) / 2) + unitArmour;
    }

    public int GetDamage()
    {
        int attackingStat = GetStrength();
        return Mathf.FloorToInt((attackingStat - 10) / 2) + weaponDamage;
    }

    public void SetStrength(int newStrength)
    {
        strength = newStrength;
    }

    public int GetStrength()
    {
        return strength;
    }

    public int GetDexterity()
    {
        return dexterity;
    }

    public int GetConstitution()
    {
        return constitution;
    }

    public int GetIntelligence()
    {
        return intelligence;
    }

    public int GetWisdom()
    {
        return wisdom;
    }

    public int GetCharisma()
    {
        return charisma;
    }
}
