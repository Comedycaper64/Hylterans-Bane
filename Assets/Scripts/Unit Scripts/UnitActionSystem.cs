using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }
    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler<BaseAction> OnSelectedActionChanged;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnActionStarted;

    private Unit selectedUnit;
    [SerializeField] private LayerMask unitLayerMask;
    [SerializeField] private AudioClip selectAction;

    private BaseAction selectedAction;
    private bool isBusy;


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one UnitActionSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        //SetSelectedUnit(GameObject.FindGameObjectWithTag("FriendlyUnit").GetComponent<Unit>());
        //TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        //FuseButtonUI.OnAnySummonChosen += FuseButtonUI_OnAnySummonChosen;
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
        if (InputManager.Instance.IsMouseButtonDownThisFrame())
        {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

            if (selectedAction.IsValidActionGridPosition(mouseGridPosition))
            {
                //Action only goes through is Unit has enough AP
                // if (selectedUnit.TrySpendActionPointsToTakeAction(selectedAction))
                // {
                    
                // }
                SetBusy();
                selectedAction.TakeAction(mouseGridPosition, ClearBusy);
                OnActionStarted?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    //isBusy is changed and the event is invoked if an action is being done
        //Other actions cannot be peformed while the actionsystem is busy
    private void SetBusy()
    {  
        isBusy = true;
        OnBusyChanged?.Invoke(this, isBusy);
    }

    private void ClearBusyForSummon()
    {
        isBusy = false;
    }

    private void ClearBusy()
    {
        isBusy = false;
        OnBusyChanged?.Invoke(this, isBusy);
    }

    //Unit that's clicked on is new selected unit
    private bool TryHandleUnitSelection()
    {
        if (InputManager.Instance.IsMouseButtonDownThisFrame())
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

        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetSelectedAction(BaseAction baseAction)
    {
        selectedAction = baseAction;
        AudioSource.PlayClipAtPoint(selectAction, Camera.main.transform.position, SoundManager.Instance.GetSoundEffectVolume());
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

    // private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    // {
    //     if (TurnSystem.Instance.IsPlayerTurn())
    //     {
    //         List<Unit> unitList = UnitManager.Instance.GetFriendlyUnitList();
    //         if (unitList.Count > 0)
    //         {
    //             SetSelectedUnit(unitList[0]);
    //         }
    //     }
    //     else
    //     {
    //         SetSelectedUnit(null);
    //     }
    //     OnSelectedUnitChanged.Invoke(this, EventArgs.Empty);
    // }

    // private void FuseButtonUI_OnAnySummonChosen(object sender, FuseButtonUI.OnSummonChosenArgs e)
    // {
    //     ClearBusyForSummon();
    // }

}
