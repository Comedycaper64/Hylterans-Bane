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
        UpdateMouseVisual();
    }

    private void OnDisable()
    {
        UnitActionSystem.Instance.OnSelectedActionChanged -=
            UnitActionSystem_OnSelectedActionChanged;
    }

    // Update is called once per frame
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

    private void UpdateMouseVisual()
    {
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
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
            case GrenadeAction grenadeAction:
                gridVisualType = GridSystemVisual.GridVisualType.Yellow;
                break;
            case SwordAction swordAction:
                gridVisualType = GridSystemVisual.GridVisualType.Red;
                break;
            case WideSlashAction slashAction:
                gridVisualType = GridSystemVisual.GridVisualType.Red;
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
}
