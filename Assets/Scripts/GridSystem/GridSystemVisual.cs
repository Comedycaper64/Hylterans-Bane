using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    //Static, so that it can be referenced from any class
    public static GridSystemVisual Instance { get; private set; }

    [SerializeField]
    private Transform gridSystemVisualSinglePrefab;
    private GridSystemVisualSingle[,] gridSystemVisualSingleArray;

    //Struct for matching an enum type to a material
    [Serializable]
    public struct GridVisualTypeMaterial
    {
        public GridVisualType gridVisualType;
        public Material material;
    }

    public enum GridVisualType
    {
        White, //Movement
        Blue, //Taunt
        Red, //Attack Target
        RedSoft, //Attack Range
        Yellow, //Fireball
        Green, //Switch Unit
        Purple, //Fusion
        Brown, //Summon Skeleton
        SoftWhite, //Blocked Movement
        SoftBlue, //Taunt Range
    }

    [SerializeField]
    private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;

    private void Awake()
    {
        //Singleton-ed and Instance-d
        if (Instance != null)
        {
            Debug.LogError(
                "There's more than one GridSystemVisual! " + transform + " - " + Instance
            );
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    //At the start just makes the Visual squares for the entire grid
    private void Start()
    {
        gridSystemVisualSingleArray = new GridSystemVisualSingle[
            LevelGrid.Instance.GetWidth(),
            LevelGrid.Instance.GetHeight()
        ];

        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);

                Transform gridSystemVisualSingleTransform = Instantiate(
                    gridSystemVisualSinglePrefab,
                    LevelGrid.Instance.GetWorldPosition(gridPosition),
                    Quaternion.identity
                );

                gridSystemVisualSingleArray[x, z] =
                    gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            }
        }

        //Subscribes events
        UnitActionSystem.Instance.OnSelectedActionChanged +=
            UnitActionSystem_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnUnitActionStarted += UnitActionSystem_OnUnitAction;
        UnitActionSystem.Instance.OnUnitActionFinished += UnitActionSystem_OnUnitAction;
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;
        Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;

        //Changes visible tiles based on selected unit / selected action
        UpdateGridVisual();
    }

    //Meshenabled = false for all visual squares
    public void HideAllGridPosition()
    {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                gridSystemVisualSingleArray[x, z].Show(
                    GetGridVisualTypeMaterial(GridVisualType.SoftWhite)
                );
            }
        }
    }

    //Generic method for showing all tiles based on central gridposition and range
    private void ShowGridPositionRange(
        GridPosition gridPosition,
        int range,
        GridVisualType gridVisualType
    )
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();

        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                //Accounts for diagonal tiles by calculating absolute range away from the center
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > range)
                {
                    continue;
                }

                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType);
    }

    private void ShowGridPositionRangeSquare(
        GridPosition gridPosition,
        int range,
        GridVisualType gridVisualType
    )
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();

        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType);
    }

    //Meshenabled = true for all visual squares that are in the input list, also applies a material based on the gridVisualType enum
    public void ShowGridPositionList(
        List<GridPosition> gridPositionList,
        GridVisualType gridVisualType
    )
    {
        foreach (GridPosition gridPosition in gridPositionList)
        {
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].Show(
                GetGridVisualTypeMaterial(gridVisualType)
            );
        }
    }

    public GridSystemVisualSingle GetGridSystemVisualSingleAtGridPosition(GridPosition gridPosition)
    {
        return gridSystemVisualSingleArray[gridPosition.x, gridPosition.z];
    }

    //Shows the visual range of the selected action for the selected unit
    public void UpdateGridVisual()
    {
        HideAllGridPosition();
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        if (!selectedUnit)
        {
            return;
        }

        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        if (!selectedAction)
        {
            return;
        }

        GridVisualType gridVisualType;

        switch (selectedAction)
        {
            default:
            case InteractAction interactAction:
                gridVisualType = GridVisualType.Blue;
                break;
            case FireballAction fireballAction:
                gridVisualType = GridVisualType.Yellow;
                ShowGridPositionRange(
                    selectedUnit.GetGridPosition(),
                    fireballAction.GetMaxThrowDistance(),
                    GridVisualType.Yellow
                );
                break;
            case ShootAction shootAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRange(
                    selectedUnit.GetGridPosition(),
                    shootAction.GetMaxShootDistance(),
                    GridVisualType.RedSoft
                );
                break;
            case FireboltAction fireboltAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRange(
                    selectedUnit.GetGridPosition(),
                    fireboltAction.GetMaxShootDistance(),
                    GridVisualType.RedSoft
                );
                break;
            case MoveAction moveAction:
                gridVisualType = GridVisualType.White;
                ShowGridPositionRange(
                    selectedUnit.GetGridPosition(),
                    moveAction.GetMaxMoveDistance(),
                    GridVisualType.SoftWhite
                );
                break;
            case SwordAction swordAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRangeSquare(
                    selectedUnit.GetGridPosition(),
                    swordAction.GetMaxSwordDistance(),
                    GridVisualType.RedSoft
                );
                break;
            case CleaveAction slashAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRange(
                    selectedUnit.GetGridPosition(),
                    slashAction.GetMaxSlashDistance(),
                    GridVisualType.RedSoft
                );
                break;
        }

        ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType);
    }

    //When the selected action is changed in UnitActionSystem, UpdateGridVisual
    private void UnitActionSystem_OnSelectedActionChanged(object sender, BaseAction e)
    {
        UpdateGridVisual();
    }

    private void UnitActionSystem_OnSelectedUnitChanged()
    {
        UpdateGridVisual();
    }

    private void UnitActionSystem_OnUnitAction()
    {
        UpdateGridVisual();
    }

    //When a unit has moved on the grid, UpdateGridVisual
    private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void Unit_OnAnyUnitSpawned(object sender, GridPosition e)
    {
        UpdateGridVisual();
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            HideAllGridPosition();
        }
    }

    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void OnDisable()
    {
        UnitActionSystem.Instance.OnSelectedActionChanged -=
            UnitActionSystem_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnUnitActionStarted -= UnitActionSystem_OnUnitAction;
        UnitActionSystem.Instance.OnUnitActionFinished -= UnitActionSystem_OnUnitAction;
        UnitActionSystem.Instance.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged;
        LevelGrid.Instance.OnAnyUnitMovedGridPosition -= LevelGrid_OnAnyUnitMovedGridPosition;
        Unit.OnAnyUnitSpawned -= Unit_OnAnyUnitSpawned;
        TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChanged;
        BaseAction.OnAnyActionCompleted -= BaseAction_OnAnyActionCompleted;
    }

    //Gets the corresponding material of the gridVisualType enum
    public Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList)
        {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType)
            {
                return gridVisualTypeMaterial.material;
            }
        }

        Debug.LogError(
            "Could not find GridVisualTypeMaterial for GridVisualType " + gridVisualType
        );
        return null;
    }
}
