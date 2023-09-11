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

    public StatBonus currentStatBonus;
    public int attackAugment = 0;
    public int savingThrowAugment = 0;

    [Min(1)]
    [SerializeField]
    private int unitLevel = 1;

    // [SerializeField]
    // private HitDiceType unitHitDice;

    private int totalRolledHealth;

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

    public int GetStatValue(StatType statType)
    {
        return statDictionary[statType];
    }

    public int GetMaxHealth()
    {
        return GetHitDiceValue(baseStats.GetHitDiceType())
            + totalRolledHealth
            + (GetModifier(statDictionary[StatType.CON]) * unitLevel);
    }

    private int GetHitDiceValue(HitDiceType hitDice)
    {
        switch (hitDice)
        {
            default:
            case HitDiceType.d6:
                return 6;
            case HitDiceType.d8:
                return 8;
            case HitDiceType.d10:
                return 10;
            case HitDiceType.d12:
                return 12;
        }
    }

    public int GetToHit()
    {
        int toHitStat = statDictionary[attackingStat];
        return GetModifier(toHitStat) + GetProficiencyBonus() + currentStatBonus.toHitBonus;
    }

    public int GetRoll()
    {
        return Random.Range(1, 21);
    }

    public int GetAttackRoll(out RollAugment rollAugment)
    {
        if (attackAugment > 0)
        {
            Debug.Log("Advantaged!");
            rollAugment = RollAugment.Advantage;
            return Mathf.Max(Random.Range(1, 21), Random.Range(1, 21));
        }
        else if (attackAugment < 0)
        {
            rollAugment = RollAugment.Disadvantage;
            return Mathf.Min(Random.Range(1, 21), Random.Range(1, 21));
        }
        else
        {
            rollAugment = RollAugment.Unchanged;
            return Random.Range(1, 21);
        }
    }

    public int GetSavingThrowRoll(out RollAugment rollAugment)
    {
        if (savingThrowAugment > 0)
        {
            rollAugment = RollAugment.Advantage;
            return Mathf.Max(Random.Range(1, 21), Random.Range(1, 21));
        }
        else if (savingThrowAugment < 0)
        {
            rollAugment = RollAugment.Disadvantage;
            return Mathf.Min(Random.Range(1, 21), Random.Range(1, 21));
        }
        else
        {
            rollAugment = RollAugment.Unchanged;
            return Random.Range(1, 21);
        }
    }

    public int GetArmourClass()
    {
        return 10
            + Mathf.Min(GetModifier(baseStats.GetDexterity()), unitArmour.GetDexBonusLimit())
            + unitArmour.GetArmourBonus()
            + currentStatBonus.acBonus;
    }

    public int GetDamage()
    {
        int damageStat = statDictionary[attackingStat];
        return GetModifier(damageStat)
            + unitWeapon.GetWeaponDamage()
            + currentStatBonus.damageBonus;
    }

    public int GetAttackRange()
    {
        return unitWeapon.GetWeaponRange() + currentStatBonus.rangeBonus;
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
        return 8 + GetProficiencyBonus() + GetModifier(statDictionary[attackingStat]);
    }

    private int GetProficiencyBonus()
    {
        return 1 + Mathf.CeilToInt(unitLevel / 4f);
    }

    public int GetLevel()
    {
        return unitLevel;
    }
}
