using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }
    public event Action OnUnitActionStarted;
    public event Action OnUnitActionFinished;
    public event Action OnSelectedUnitChanged;
    public event Action OnUnitMoved;
    public event EventHandler<BaseAction> OnSelectedActionChanged;

    private Unit selectedUnit;

    [SerializeField]
    private LayerMask unitLayerMask;

    [SerializeField]
    private AudioClip selectActionSFX;

    private BaseAction selectedAction;
    private bool isBusy;
    private bool unitTurnFinished = false;
    private ActionState currentState;
    private GridPosition unitStartPosition;

    private StatBonus actionStatBonus;

    public enum ActionState
    {
        noSelectedUnit,
        movingUnit,
        selectingAction,
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError(
                "There's more than one UnitActionSystem! " + transform + " - " + Instance
            );
            Destroy(gameObject);
            return;
        }
        Instance = this;
        currentState = ActionState.noSelectedUnit;
    }

    private void Update()
    {
        if (isBusy)
        {
            return;
        }

        //If enemy turn
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        //If pointer is over a GameObject (like the BusyUI)
        //Problematic as cancels/moves get caught on world UI. annoying --------------------------------------------------------------------------------------------------------------
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        //If the cursor is above the unit
        if (TryHandleUnitSelection())
        {
            return;
        }

        //Does a selected action if none of the above are true
        if (selectedUnit)
        {
            HandleSelectedAction();
        }
    }

    private void HandleSelectedAction()
    {
        if (InputManager.Instance.IsLeftClickDownThisFrame() && selectedAction)
        {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(
                MouseWorld.GetPosition()
            );

            if (selectedAction.IsValidActionGridPosition(mouseGridPosition))
            {
                PerformAction(selectedAction, mouseGridPosition);
            }
        }
        else if (InputManager.Instance.IsRightClickDownThisFrame()) //Resets unit to starting position if player right clicks
        {
            CancelAction();
        }
    }

    private void PerformAction(BaseAction actionToHandle, GridPosition gridPosition)
    {
        StartAction();

        selectedUnit.UseHeldActions(actionToHandle.GetRequiredHeldActions());

        if (currentState == ActionState.selectingAction)
        {
            currentState = ActionState.noSelectedUnit;
            selectedUnit.SetActionCompleted(true);
            SetSelectedUnit(null);
            unitTurnFinished = true;
        }

        actionToHandle.TakeAction(gridPosition, FinishAction);
    }

    //isBusy is changed and the event is invoked if an action is being done
    //Other actions cannot be peformed while the actionsystem is busy
    public void StartAction()
    {
        isBusy = true;
        selectedAction = null;
        OnUnitActionStarted?.Invoke();
    }

    public void CancelAction()
    {
        currentState = ActionState.noSelectedUnit;
        if (!selectedUnit.GetMovementCompleted())
        {
            BaseAction actionToHandle = selectedUnit.GetComponent<MoveAction>();
            SetSelectedAction(actionToHandle);
            StartAction();
            actionToHandle.TakeAction(unitStartPosition, FinishAction);
        }
        else
        {
            selectedAction = null;
            OnUnitActionStarted?.Invoke();
        }
        SetSelectedUnit(null);
    }

    public void FinishAction()
    {
        isBusy = false;
        OnUnitActionFinished?.Invoke();
        if (currentState == ActionState.movingUnit)
        {
            currentState = ActionState.selectingAction;
            OnUnitMoved?.Invoke();
        }
        else if (unitTurnFinished)
        {
            TurnSystem.Instance.NextInitiative(); //Should be decoupled later
        }
    }

    //Unit that's clicked on is new selected unit
    private bool TryHandleUnitSelection()
    {
        if (InputManager.Instance.IsLeftClickDownThisFrame())
        {
            Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask))
            {
                if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
                {
                    if (unit == selectedUnit)
                    {
                        // Unit is already selected
                        return false;
                    }

                    if (unit.IsEnemy())
                    {
                        return false;
                    }

                    if (unit.GetActionCompleted())
                    {
                        return false;
                    }

                    SetSelectedUnit(unit);
                    return true;
                }
            }
        }

        return false;
    }

    public void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        OnSelectedUnitChanged?.Invoke();
        if (!selectedUnit)
        {
            return;
        }

        unitStartPosition = unit.GetGridPosition();
        actionStatBonus = new StatBonus();

        if (selectedUnit.GetHeldActions() < 0)
        {
            currentState = ActionState.selectingAction;
            BaseAction waitAction = unit.GetAction<WaitAction>();
            PerformAction(waitAction, unitStartPosition);
        }
        else
        {
            //Default selected action for new selected unit
            unitTurnFinished = false;
            if (unit.GetMovementCompleted())
            {
                currentState = ActionState.selectingAction;
                OnUnitMoved?.Invoke();
            }
            else
            {
                SetSelectedAction(unit.GetAction<MoveAction>());
                currentState = ActionState.movingUnit;
            }
        }
    }

    public void SetSelectedAction(BaseAction baseAction)
    {
        RemoveStatBonuses(actionStatBonus);
        selectedAction = baseAction;
        actionStatBonus = selectedAction.GetStatBonus();
        AddStatBonuses(actionStatBonus);

        AudioSource.PlayClipAtPoint(
            selectActionSFX,
            Camera.main.transform.position,
            SoundManager.Instance.GetSoundEffectVolume()
        );
        OnSelectedActionChanged?.Invoke(this, baseAction);
    }

    private void AddStatBonuses(StatBonus statBonus)
    {
        selectedUnit.GetUnitStats().currentStatBonus += statBonus;
    }

    private void RemoveStatBonuses(StatBonus statBonus)
    {
        selectedUnit.GetUnitStats().currentStatBonus -= statBonus;
    }

    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }

    public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }

    public ActionState GetCurrentState()
    {
        return currentState;
    }

    public bool GetIsBusy()
    {
        return isBusy;
    }
}
