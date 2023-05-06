using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitStats", menuName = "The Seventy Year War/UnitStats", order = 0)]
public class UnitStats : ScriptableObject
{
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
