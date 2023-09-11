using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitStatUI : MonoBehaviour
{
    [SerializeField]
    private GameObject unitDetailsContainer;

    [SerializeField]
    private TextMeshProUGUI unitNameText;

    [SerializeField]
    private TextMeshProUGUI unitClassText;

    [SerializeField]
    private TextMeshProUGUI unitLevelText;

    [SerializeField]
    private TextMeshProUGUI unitSTRText;

    [SerializeField]
    private TextMeshProUGUI unitDEXText;

    [SerializeField]
    private TextMeshProUGUI unitCONText;

    [SerializeField]
    private TextMeshProUGUI unitINTText;

    [SerializeField]
    private TextMeshProUGUI unitWISText;

    [SerializeField]
    private TextMeshProUGUI unitCHAText;

    private void Awake()
    {
        ToggleUnitDetailsUI(false, null);
    }

    void Start()
    {
        InputManager.Instance.OnUnitDetailsEvent += InputManage_OnUnitDetailsEvent;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnUnitDetailsEvent -= InputManage_OnUnitDetailsEvent;
    }

    private void ToggleUnitDetailsUI(bool toggle, Unit unit)
    {
        unitDetailsContainer.SetActive(toggle);
        if (toggle)
        {
            UnitStats unitStats = unit.GetUnitStats();
            unitNameText.text = unit.GetUnitName();
            unitClassText.text = unit.GetUnitClass();
            unitLevelText.text = "Level " + unitStats.GetLevel();
            unitSTRText.text = "STR: " + unitStats.GetStatValue(StatType.STR);
            unitDEXText.text = "DEX: " + unitStats.GetStatValue(StatType.DEX);
            unitCONText.text = "CON: " + unitStats.GetStatValue(StatType.CON);
            unitINTText.text = "INT: " + unitStats.GetStatValue(StatType.INT);
            unitWISText.text = "WIS: " + unitStats.GetStatValue(StatType.WIS);
            unitCHAText.text = "CHA: " + unitStats.GetStatValue(StatType.CHA);
        }
    }

    private void InputManage_OnUnitDetailsEvent()
    {
        if (unitDetailsContainer.activeInHierarchy)
        {
            ToggleUnitDetailsUI(false, null);
        }
        else
        {
            if (UnitActionSystem.Instance.GetSelectedUnit())
            {
                ToggleUnitDetailsUI(true, UnitActionSystem.Instance.GetSelectedUnit());
            }
        }
    }
}
