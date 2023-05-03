using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField]
    private Transform actionButtonPrefab;

    [SerializeField]
    private Transform actionButtonContainerTransform;

    private List<ActionButtonUI> actionButtonUIList;

    //Creates list for actionbuttons
    private void Awake()
    {
        actionButtonUIList = new List<ActionButtonUI>();
    }

    private void Start()
    {
        UnitActionSystem.Instance.OnUnitMoved += UnitActionSystem_OnUnitMoved;
        UnitActionSystem.Instance.OnUnitActionFinished += UnitActionSystem_OnUnitActionFinished;

        CreateUnitActionButtons();
        UpdateSelectedVisuals();
    }

    //Instantiates a button for each action a unit can do. They're stored in the actionButtonContainer and are automatically formatted there
    //This is done each time a new unit is selected
    private void CreateUnitActionButtons()
    {
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }
        if (!actionButtonContainerTransform)
        {
            return;
        }

        ClearUnitActionButtons();

        Unit selectedUnit;
        if (UnitActionSystem.Instance.GetSelectedUnit())
        {
            selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        }
        else
        {
            return;
        }

        foreach (BaseAction baseAction in selectedUnit.GetBaseActionArray())
        {
            if (baseAction.GetActionName() == "Move")
            {
                continue;
            }
            Transform actionButtonTransform = Instantiate(
                actionButtonPrefab,
                actionButtonContainerTransform
            );
            ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
            actionButtonUI.SetBaseAction(baseAction);

            actionButtonUIList.Add(actionButtonUI);
        }
    }

    private void ClearUnitActionButtons()
    {
        foreach (Transform buttonTransform in actionButtonContainerTransform)
        {
            Destroy(buttonTransform.gameObject);
        }

        actionButtonUIList.Clear();
    }

    //Puts a green outline around the action that was selected
    private void UpdateSelectedVisuals()
    {
        foreach (ActionButtonUI actionButton in actionButtonUIList)
        {
            actionButton.UpdateSelectedVisual();
        }
    }

    private void UnitActionSystem_OnUnitMoved()
    {
        CreateUnitActionButtons();
        UpdateSelectedVisuals();
    }

    private void UnitActionSystem_OnUnitActionFinished()
    {
        ClearUnitActionButtons();
    }

    private void OnDisable()
    {
        UnitActionSystem.Instance.OnUnitMoved -= UnitActionSystem_OnUnitMoved;
        UnitActionSystem.Instance.OnUnitActionFinished -= UnitActionSystem_OnUnitActionFinished;
    }
}
