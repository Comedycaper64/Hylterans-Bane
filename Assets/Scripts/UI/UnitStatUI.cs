using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitStatUI : MonoBehaviour
{
    [SerializeField]
    private LayerMask unitLayerMask;

    [SerializeField]
    private GameObject unitDetails;

    [SerializeField]
    private Transform unitActionContainer;

    [SerializeField]
    private Transform unitPassiveContainer;

    [SerializeField]
    private Transform unitRallyingCryContainer;

    [SerializeField]
    private GameObject unitAbilityPrefab;

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
        unitDetails.SetActive(toggle);
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
            ClearContainers();
            foreach (BaseAction action in unit.GetBaseActionList())
            {
                string actionName = action.GetActionName();
                if ((actionName == "Move") || (actionName == "Attack") || (actionName == "Wait"))
                {
                    continue;
                }
                AbilityDetailUI actionUI = Instantiate(unitAbilityPrefab, unitActionContainer)
                    .GetComponent<AbilityDetailUI>();
                actionUI.SetupAbilityUI(actionName, action.GetActionDescription());
            }
            foreach (PassiveAbility passiveAbility in unit.GetComponents<PassiveAbility>())
            {
                AbilityDetailUI passiveUI = Instantiate(unitAbilityPrefab, unitPassiveContainer)
                    .GetComponent<AbilityDetailUI>();
                passiveUI.SetupAbilityUI(
                    passiveAbility.GetAbilityName(),
                    passiveAbility.GetAbilityDescription()
                );
            }
            if (unit.TryGetComponent(out RallyingCry rallyingCry))
            {
                AbilityDetailUI detailUI = Instantiate(unitAbilityPrefab, unitRallyingCryContainer)
                    .GetComponent<AbilityDetailUI>();
                detailUI.SetupAbilityUI(
                    rallyingCry.GetAbilityName(),
                    rallyingCry.GetAbilityDescription()
                );
            }
        }
    }

    private void ClearContainers()
    {
        foreach (Transform action in unitActionContainer)
        {
            Destroy(action.gameObject);
        }
        foreach (Transform passive in unitPassiveContainer)
        {
            Destroy(passive.gameObject);
        }
        foreach (Transform rally in unitRallyingCryContainer)
        {
            Destroy(rally.gameObject);
        }
    }

    private void InputManage_OnUnitDetailsEvent()
    {
        if (unitDetails.activeInHierarchy)
        {
            ToggleUnitDetailsUI(false, null);
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask))
            {
                if (raycastHit.transform.TryGetComponent(out Unit unit))
                {
                    ToggleUnitDetailsUI(true, unit);
                }
            }
            else if (UnitActionSystem.Instance.GetSelectedUnit())
            {
                ToggleUnitDetailsUI(true, UnitActionSystem.Instance.GetSelectedUnit());
            }
        }
    }
}
