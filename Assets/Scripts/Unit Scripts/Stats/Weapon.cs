using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "The Seventy Year War/Weapon", order = 1)]
public class Weapon : ScriptableObject
{
    [SerializeField]
    private string weaponName;

    [SerializeField]
    private int weaponDamage;

    [SerializeField]
    private int weaponMinRange;

    [SerializeField]
    private int weaponMaxRange;

    public string GetWeaponName()
    {
        return weaponName;
    }

    public int GetWeaponDamage()
    {
        return weaponDamage;
    }

    public int GetWeaponMinRange()
    {
        return weaponMinRange;
    }

    public int GetWeaponMaxRange()
    {
        return weaponMaxRange;
    }
}
