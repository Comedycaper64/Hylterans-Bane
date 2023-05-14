using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMouseVisual : MonoBehaviour
{
    [SerializeField]
    private GameObject mouseGridVisual;
    private Transform mouseGridVisualPosition;
    private GridSystemVisualSingle mouseGridVisualScript;
    private float mouseGridVisualYOffset = 0.1f;
    public static EventHandler<Unit> OnMouseOverEnemyUnit;

    private void Start()
    {
        mouseGridVisualPosition = Instantiate(
            mouseGridVisual,
            new Vector3(0, 0, 0),
            Quaternion.identity
        ).transform;
        mouseGridVisualScript = mouseGridVisualPosition.GetComponent<GridSystemVisualSingle>();
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
        if (
            !TurnSystem.Instance.IsPlayerTurn()
            || UnitManager.Instance.GetFriendlyUnitList().Count < 1
            || !UnitActionSystem.Instance.GetSelectedUnit()
        )
        {
            mouseGridVisualPosition.gameObject.SetActive(false);
            return;
        }

        GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(
            MouseWorld.GetPosition()
        );

        if (
            UnitActionSystem.Instance.GetSelectedAction()
            && UnitActionSystem.Instance
                .GetSelectedAction()
                .IsValidActionGridPosition(mouseGridPosition)
        )
        {
            mouseGridVisualPosition.gameObject.SetActive(true);
            float cellSize = LevelGrid.Instance.GetCellSize();
            mouseGridVisualPosition.position = new Vector3(
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
            mouseGridVisualPosition.gameObject.SetActive(false);
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
