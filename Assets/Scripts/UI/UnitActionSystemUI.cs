using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainerTransform;
    [SerializeField] private TextMeshProUGUI actionPointsText;
    [SerializeField] private TextMeshProUGUI actionPointCostText;

    private List<ActionButtonUI> actionButtonUIList;
    //Creates list for actionbuttons
    private void Awake() 
    {
        actionButtonUIList = new List<ActionButtonUI>();    
    }

    //Subscribes to a *fuckton* of events
    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnActionStarted;
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;

        UpdateActionPoints();
        UpdateActionPointCost();
        CreateUnitActionButtons();
        UpdateSelectedVisuals();
    }

    //Instantiates a button for each action a unit can do. They're stored in the actionButtonContainer and are automatically formatted there
        //This is done each time a new unit is selected
    private void CreateUnitActionButtons()
    {
        if (!TurnSystem.Instance.IsPlayerTurn()) {return;}
        if (!actionButtonContainerTransform) {return;}
        
        foreach (Transform buttonTransform in actionButtonContainerTransform)
        {
            Destroy(buttonTransform.gameObject);
        }

        actionButtonUIList.Clear();

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
            Transform actionButtonTransform = Instantiate(actionButtonPrefab, actionButtonContainerTransform);
            ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
            actionButtonUI.SetBaseAction(baseAction);

            actionButtonUIList.Add(actionButtonUI);
        }
    }

    //Updates the actionPoint UI whenever an action is started, when the turn is changed, when the selected unit is changed and when any action point is changed
    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        CreateUnitActionButtons();
        UpdateSelectedVisuals();   
        UpdateActionPointCost();
        UpdateActionPoints();
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, BaseAction e)
    {
        UpdateSelectedVisuals();
        UpdateActionPointCost();
    }
    
    private void UnitActionSystem_OnActionStarted(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    //Puts a green outline around the action that was selected
    private void UpdateSelectedVisuals()
    {
        foreach (ActionButtonUI actionButton in actionButtonUIList)
        {
            actionButton.UpdateSelectedVisual();
        }
    }

    private void UpdateActionPoints()
    {
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        if (!selectedUnit) {return;}

        actionPointsText.text = "Action Points: " + selectedUnit.GetActionPoints();
    }

    //Shows how many actionpoints a unit has left above their actins
    private void UpdateActionPointCost()
    {
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        if (!selectedAction) {return;}

        actionPointCostText.text = "Action Point Cost: " + selectedAction.GetActionPointsCost();
    }
}
