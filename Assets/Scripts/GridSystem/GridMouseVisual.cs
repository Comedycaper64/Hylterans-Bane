using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMouseVisual : MonoBehaviour
{    
    [SerializeField] private GameObject mouseGridVisual;
    [SerializeField] private GameObject mouseGridGrenadeVisual;
    private Transform mouseGridVisualPosition;
    private Transform mouseGridGrenadeVisualPosition;
    private GridSystemVisualSingle mouseGridVisualScript;
    //[SerializeField] private Material mouseGridVisualMaterial;
    private float mouseGridVisualYOffset = 0.1f;

    private void Start() 
    {
        mouseGridVisualPosition = Instantiate(mouseGridVisual, new Vector3(0, 0, 0), Quaternion.identity).transform;
        mouseGridGrenadeVisualPosition = Instantiate(mouseGridGrenadeVisual, mouseGridVisualPosition).transform;
        mouseGridGrenadeVisualPosition.gameObject.SetActive(false);
        mouseGridVisualScript = mouseGridVisualPosition.GetComponent<GridSystemVisualSingle>();
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;    
        UpdateMouseVisual();
    }

    // Update is called once per frame
    void Update()
    {
        if (!TurnSystem.Instance.IsPlayerTurn() || UnitManager.Instance.GetFriendlyUnitList().Count < 1 || !UnitActionSystem.Instance.GetSelectedUnit()) 
        {
            mouseGridVisualPosition.gameObject.SetActive(false);
            mouseGridGrenadeVisualPosition.gameObject.SetActive(false);
            return;
        }

        if (UnitActionSystem.Instance.GetSelectedAction().GetActionPointsCost() > UnitActionSystem.Instance.GetSelectedUnit().GetActionPoints()) 
        {
            mouseGridVisualPosition.gameObject.SetActive(false);
            mouseGridGrenadeVisualPosition.gameObject.SetActive(false);
            return;
        }
        
        GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
        if (UnitActionSystem.Instance.GetSelectedAction().IsValidActionGridPosition(mouseGridPosition))
        {
            mouseGridVisualPosition.gameObject.SetActive(true);
            float cellSize = LevelGrid.Instance.GetCellSize();
            mouseGridVisualPosition.position = new Vector3(mouseGridPosition.x * cellSize, mouseGridVisualYOffset, mouseGridPosition.z * cellSize);
        }
        else
        {
            mouseGridVisualPosition.gameObject.SetActive(false);
        }
    }

    private void UpdateMouseVisual()
    {
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        GridSystemVisual.GridVisualType gridVisualType;
        mouseGridVisualPosition.localScale = new Vector3(1, 1, 1);
        mouseGridGrenadeVisualPosition.gameObject.SetActive(false);
        switch (selectedAction)
        {
            default:
            case MoveAction moveAction:
                gridVisualType = GridSystemVisual.GridVisualType.White;
                break;
            case ShootAction shootAction:
                gridVisualType = GridSystemVisual.GridVisualType.Red;
                break;
            case TauntAction tauntAction:
                gridVisualType = GridSystemVisual.GridVisualType.Blue;
                break;
            case GrenadeAction grenadeAction:
                gridVisualType = GridSystemVisual.GridVisualType.Yellow;
                mouseGridGrenadeVisualPosition.gameObject.SetActive(true);
                break;
            case SwordAction swordAction:
                gridVisualType = GridSystemVisual.GridVisualType.Red;
                break;
            case WideSlashAction slashAction:
                gridVisualType = GridSystemVisual.GridVisualType.Red;
                mouseGridVisualPosition.localScale = new Vector3(3.2f, 3.2f, 3.2f);
                break;
            case InteractAction interactAction:
                gridVisualType = GridSystemVisual.GridVisualType.Blue;
                break;
            case SummonSkeletonAction summonSkeleton:
                gridVisualType = GridSystemVisual.GridVisualType.Brown;
                break;
            case UnitSwitchAction unitSwitchAction:
                gridVisualType = GridSystemVisual.GridVisualType.Green;
                break;

        }
        mouseGridVisualScript.Show(GridSystemVisual.Instance.GetGridVisualTypeMaterial(gridVisualType));
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, BaseAction e)
    {
        UpdateMouseVisual();
    }
}
