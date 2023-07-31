using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMouseVisual : MonoBehaviour
{
    [SerializeField]
    private GameObject mouseGridVisualPrefab;

    [SerializeField]
    private GameObject mouseGridArrowVisualPrefab;

    private Transform mouseGridVisual;
    private List<Transform> mouseGridVisualAOE = new List<Transform>();
    private Transform mouseGridArrowVisual;
    private GridSystemVisualSingle mouseGridVisualScript;
    private float mouseGridVisualYOffset = 0.1f;
    private float mouseGridArrowVisualYOffset = 2f;
    public static EventHandler<Unit> OnMouseOverEnemyUnit;

    private void Start()
    {
        mouseGridVisual = Instantiate(
            mouseGridVisualPrefab,
            new Vector3(0, 0, 0),
            Quaternion.identity
        ).transform;
        mouseGridArrowVisual = Instantiate(
            mouseGridArrowVisualPrefab,
            new Vector3(0, 5f, 0),
            Quaternion.identity
        ).transform;
        mouseGridVisualScript = mouseGridVisual.GetComponent<GridSystemVisualSingle>();
        mouseGridVisualScript.ToggleTransparencyOscillation(true);
        UnitActionSystem.Instance.OnSelectedActionChanged +=
            UnitActionSystem_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnUnitActionStarted += UnitActionSystem_OnUnitActionStarted;
        UpdateMouseVisual();
    }

    private void OnDisable()
    {
        UnitActionSystem.Instance.OnSelectedActionChanged -=
            UnitActionSystem_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnUnitActionStarted -= UnitActionSystem_OnUnitActionStarted;
    }

    // Update is called once per frame
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void Update()
    {
        GridPosition mouseGridPosition;
        if (MouseWorld.GetPosition() != Vector3.negativeInfinity)
        {
            mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
        }
        else
        {
            mouseGridVisual.gameObject.SetActive(false);
            mouseGridArrowVisual.gameObject.SetActive(false);
            return;
        }

        float cellSize = LevelGrid.Instance.GetCellSize();
        if (
            !TurnSystem.Instance.IsPlayerTurn()
            || UnitManager.Instance.GetFriendlyUnitList().Count < 1
            || !LevelGrid.Instance.IsValidGridPosition(mouseGridPosition)
        //|| !UnitActionSystem.Instance.GetSelectedUnit()
        )
        {
            mouseGridVisual.gameObject.SetActive(false);
            mouseGridArrowVisual.gameObject.SetActive(false);
            return;
        }
        else
        {
            mouseGridArrowVisual.gameObject.SetActive(true);
            mouseGridArrowVisual.position = new Vector3(
                mouseGridPosition.x * cellSize,
                mouseGridArrowVisualYOffset,
                mouseGridPosition.z * cellSize
            );
        }

        if (
            UnitActionSystem.Instance.GetSelectedAction()
            && UnitActionSystem.Instance
                .GetSelectedAction()
                .IsValidActionGridPosition(mouseGridPosition)
        )
        {
            mouseGridVisual.gameObject.SetActive(true);

            mouseGridVisual.position = new Vector3(
                mouseGridPosition.x * cellSize,
                mouseGridVisualYOffset,
                mouseGridPosition.z * cellSize
            );
            if (LevelGrid.Instance.HasAnyUnitOnGridPosition(mouseGridPosition))
            {
                Unit unitAtMouseGrid = LevelGrid.Instance.GetUnitAtGridPosition(mouseGridPosition);
                if (unitAtMouseGrid.IsEnemy())
                {
                    OnMouseOverEnemyUnit?.Invoke(this, unitAtMouseGrid);
                }
            }
        }
        else
        {
            mouseGridVisual.gameObject.SetActive(false);
        }
    }

    private void SetAOEVisual(bool enable, int range, GridSystemVisual.GridVisualType visualType)
    {
        if (enable)
        {
            range = (range - 1) / 2;
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(
                mouseGridVisual.position
            );
            for (int x = mouseGridPosition.x - range; x <= mouseGridPosition.x + range; x++)
            {
                for (int z = mouseGridPosition.z - range; z <= mouseGridPosition.z + range; z++)
                {
                    Vector3 newMouseVisualSpawn =
                        LevelGrid.Instance.GetWorldPosition(new GridPosition(x, z))
                        + new Vector3(0, mouseGridVisualYOffset, 0);
                    Transform newMouseVisual = Instantiate(
                        mouseGridVisualPrefab,
                        newMouseVisualSpawn,
                        Quaternion.identity
                    ).transform;
                    newMouseVisual
                        .GetComponent<GridSystemVisualSingle>()
                        .Show(GridSystemVisual.Instance.GetGridVisualTypeMaterial(visualType));
                    newMouseVisual
                        .GetComponent<GridSystemVisualSingle>()
                        .ToggleTransparencyOscillation(true);
                    mouseGridVisualAOE.Add(newMouseVisual);
                    newMouseVisual.parent = mouseGridVisual;
                }
            }
        }
        else if (mouseGridVisualAOE.Count > 0)
        {
            foreach (Transform mouseVisual in mouseGridVisualAOE)
            {
                Destroy(mouseVisual.gameObject);
            }
            mouseGridVisualAOE.Clear();
        }
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void UpdateMouseVisual()
    {
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        SetAOEVisual(false, 0, GridSystemVisual.GridVisualType.White);
        if (!selectedAction)
        {
            mouseGridVisualScript.Hide();
            return;
        }

        GridSystemVisual.GridVisualType gridVisualType;
        switch (selectedAction)
        {
            default:
            case MoveAction moveAction:
                gridVisualType = GridSystemVisual.GridVisualType.White;
                break;
            case ShootAction shootAction:
                gridVisualType = GridSystemVisual.GridVisualType.Red;
                break;
            case FireballAction grenadeAction:
                gridVisualType = GridSystemVisual.GridVisualType.Yellow;
                SetAOEVisual(true, 3, gridVisualType);
                break;
            case SwordAction swordAction:
                gridVisualType = GridSystemVisual.GridVisualType.Red;
                break;
            case CleaveAction slashAction:
                gridVisualType = GridSystemVisual.GridVisualType.Red;
                SetAOEVisual(true, 3, gridVisualType);
                break;
            case InteractAction interactAction:
                gridVisualType = GridSystemVisual.GridVisualType.Blue;
                break;
        }
        mouseGridVisualScript.Show(
            GridSystemVisual.Instance.GetGridVisualTypeMaterial(gridVisualType)
        );
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, BaseAction e)
    {
        UpdateMouseVisual();
    }

    private void UnitActionSystem_OnUnitActionStarted()
    {
        UpdateMouseVisual();
    }
}
