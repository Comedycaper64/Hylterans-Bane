using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStats : MonoBehaviour
{
    [SerializeField]
    private BaseStats baseStats;
    private Dictionary<StatType, int> statDictionary = new Dictionary<StatType, int>();

    [SerializeField]
    private StatType attackingStat;

    private int weaponDamage = 8;

    private void Awake()
    {
        SetupDictionary();
    }

    public void SetupDictionary()
    {
        statDictionary.Add(StatType.STR, baseStats.GetStrength());
        statDictionary.Add(StatType.DEX, baseStats.GetDexterity());
        statDictionary.Add(StatType.CON, baseStats.GetConstitution());
        statDictionary.Add(StatType.INT, baseStats.GetIntelligence());
        statDictionary.Add(StatType.WIS, baseStats.GetWisdom());
        statDictionary.Add(StatType.CHA, baseStats.GetCharisma());
    }

    public int GetMaxHealth()
    {
        return baseStats.GetMaxHealth();
    }

    public int GetToHit()
    {
        int toHitStat = statDictionary[attackingStat];
        return GetModifier(toHitStat) + baseStats.GetProficiencyBonus();
    }

    public int GetAttackRoll()
    {
        return Random.Range(1, 21) + GetToHit();
    }

    public int GetArmourClass()
    {
        int unitArmour = 0;
        return 10 + GetModifier(baseStats.GetDexterity()) + unitArmour;
    }

    public int GetDamage()
    {
        int damageStat = statDictionary[attackingStat];
        return GetModifier(damageStat) + weaponDamage;
    }

    public int GetSavingThrow(StatType savingThrowType)
    {
        return Random.Range(1, 21) + GetModifier(statDictionary[savingThrowType]);
    }

    private int GetModifier(int score)
    {
        return Mathf.FloorToInt(score - 10) / 2;
    }

    public int GetSpellDC()
    {
        return 8 + baseStats.GetProficiencyBonus() + GetModifier(statDictionary[attackingStat]);
    }
}
