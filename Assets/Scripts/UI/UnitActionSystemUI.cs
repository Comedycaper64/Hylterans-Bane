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

    [SerializeField]
    private GameObject actionDescription;

    [SerializeField]
    private TextMeshProUGUI actionDescriptionText;

    private List<ActionButtonUI> actionButtonUIList;

    //Creates list for actionbuttons
    private void Awake()
    {
        actionButtonUIList = new List<ActionButtonUI>();
    }

    private void Start()
    {
        UnitActionSystem.Instance.OnUnitMoved += UnitActionSystem_OnUnitMoved;
        UnitActionSystem.Instance.OnUnitActionStarted += UnitActionSystem_OnUnitActionStarted;
        UnitActionSystem.Instance.OnSelectedActionChanged +=
            UnitActionSystem_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;

        CreateUnitActionButtons();
        UpdateSelectedVisuals(null);
    }

    private void OnDisable()
    {
        UnitActionSystem.Instance.OnUnitMoved -= UnitActionSystem_OnUnitMoved;
        UnitActionSystem.Instance.OnUnitActionStarted -= UnitActionSystem_OnUnitActionStarted;
        UnitActionSystem.Instance.OnSelectedActionChanged -=
            UnitActionSystem_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged;
    }

    //Instantiates a button for each action a unit can do. They're stored in the actionButtonContainer and are automatically formatted there
    //This is done each time a new unit is selected
    private void CreateUnitActionButtons()
    {
        ClearUnitActionButtons();
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }
        if (!actionButtonContainerTransform)
        {
            return;
        }

        Unit selectedUnit;
        if (UnitActionSystem.Instance.GetSelectedUnit())
        {
            selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        }
        else
        {
            return;
        }

        foreach (BaseAction baseAction in selectedUnit.GetBaseActionList())
        {
            if (baseAction.GetActionName() == "Move")
            {
                continue;
            }
            if (baseAction.IsDisabled())
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
    private void UpdateSelectedVisuals(BaseAction selectedBaseAction)
    {
        foreach (ActionButtonUI actionButton in actionButtonUIList)
        {
            actionButton.UpdateSelectedVisual();
        }

        if (selectedBaseAction && (selectedBaseAction.GetActionDescription() != null))
        {
            actionDescription.SetActive(true);
            actionDescriptionText.text = selectedBaseAction.GetActionDescription();
        }
        else
        {
            actionDescription.SetActive(false);
        }
    }

    private void UnitActionSystem_OnUnitMoved()
    {
        CreateUnitActionButtons();
        UpdateSelectedVisuals(null);
    }

    private void UnitActionSystem_OnUnitActionStarted()
    {
        ClearUnitActionButtons();
        UpdateSelectedVisuals(null);
    }

    private void UnitActionSystem_OnSelectedActionChanged(
        object sender,
        BaseAction selectedBaseAction
    )
    {
        UpdateSelectedVisuals(selectedBaseAction);
    }

    private void UnitActionSystem_OnSelectedUnitChanged()
    {
        ClearUnitActionButtons();
    }
}
