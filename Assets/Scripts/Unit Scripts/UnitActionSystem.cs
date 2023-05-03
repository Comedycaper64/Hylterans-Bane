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
    private ActionState currentState;
    private GridPosition unitStartPosition;

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
        if (InputManager.Instance.IsLeftClickDownThisFrame())
        {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(
                MouseWorld.GetPosition()
            );

            if (selectedAction.IsValidActionGridPosition(mouseGridPosition))
            {
                BaseAction actionToHandle = selectedAction; //Set to this so that the SetBusy method doesn't cause a NullReference
                StartAction();
                actionToHandle.TakeAction(mouseGridPosition, FinishAction);

                if (currentState == ActionState.selectingAction)
                {
                    currentState = ActionState.noSelectedUnit;
                    selectedUnit.SetActionCompleted(true);
                    SetSelectedUnit(null);
                    OnUnitActionFinished?.Invoke();
                }
            }
        }
        else if (InputManager.Instance.IsRightClickDownThisFrame()) //Resets unit to starting position if player right clicks
        {
            currentState = ActionState.noSelectedUnit;
            BaseAction actionToHandle = selectedUnit.GetComponent<MoveAction>();
            StartAction();
            actionToHandle.TakeAction(unitStartPosition, FinishAction);
            SetSelectedUnit(null);
            OnUnitActionFinished?.Invoke();
        }
    }

    //isBusy is changed and the event is invoked if an action is being done
    //Other actions cannot be peformed while the actionsystem is busy
    private void StartAction()
    {
        isBusy = true;
        selectedAction = null;
        OnUnitActionStarted?.Invoke();
    }

    private void FinishAction()
    {
        isBusy = false;
        if (currentState == ActionState.movingUnit)
        {
            currentState = ActionState.selectingAction;
            OnUnitMoved?.Invoke();
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
        if (!selectedUnit)
        {
            return;
        }
        //Default selected action for new selected unit
        SetSelectedAction(unit.GetAction<MoveAction>());
        unitStartPosition = unit.GetGridPosition();
        currentState = ActionState.movingUnit;
        OnSelectedUnitChanged?.Invoke();
    }

    public void SetSelectedAction(BaseAction baseAction)
    {
        selectedAction = baseAction;
        AudioSource.PlayClipAtPoint(
            selectActionSFX,
            Camera.main.transform.position,
            SoundManager.Instance.GetSoundEffectVolume()
        );
        OnSelectedActionChanged?.Invoke(this, baseAction);
    }

    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }

    public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }
}
