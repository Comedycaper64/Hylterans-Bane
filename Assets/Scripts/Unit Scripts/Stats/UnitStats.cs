using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStats : MonoBehaviour
{
    [SerializeField]
    private BaseStats baseStats;

    [SerializeField]
    private Weapon unitWeapon;

    [SerializeField]
    private Armour unitArmour;

    private Dictionary<StatType, int> statDictionary = new Dictionary<StatType, int>();

    [SerializeField]
    private StatType attackingStat;

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

    public int GetRoll()
    {
        return Random.Range(1, 21);
    }

    public int GetAttackRoll()
    {
        return Random.Range(1, 21) + GetToHit();
    }

    public int GetArmourClass()
    {
        return 10
            + Mathf.Min(GetModifier(baseStats.GetDexterity()), unitArmour.GetDexBonusLimit())
            + unitArmour.GetArmourBonus();
    }

    public int GetDamage()
    {
        int damageStat = statDictionary[attackingStat];
        return GetModifier(damageStat) + unitWeapon.GetWeaponDamage();
    }

    public int GetAttackRange()
    {
        return unitWeapon.GetWeaponRange();
    }

    public int GetInitiative()
    {
        return GetRoll() + GetModifier(statDictionary[StatType.DEX]);
    }

    public int GetSavingThrow(StatType savingThrowType)
    {
        return GetModifier(statDictionary[savingThrowType]);
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
