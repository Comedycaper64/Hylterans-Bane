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
    private bool aoeEnabled;
    private (int, int) currentRange;
    private GridSystemVisual.GridVisualType currentVisualType;

    //public static EventHandler<Unit> OnMouseOverEnemyUnit;
    private GridPosition currentMousePosition;

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
        ToggleMouseVisibility(true);
    }

    private void OnDisable()
    {
        UnitActionSystem.Instance.OnSelectedActionChanged -=
            UnitActionSystem_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnUnitActionStarted -= UnitActionSystem_OnUnitActionStarted;
    }

    void Update()
    {
        GridPosition mouseGridPosition;
        if (MouseWorld.GetPosition() != Vector3.negativeInfinity)
        {
            //ToggleMouseVisibility(true);
            mouseGridArrowVisual.gameObject.SetActive(true);
            mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
            if (mouseGridPosition != currentMousePosition)
            {
                currentMousePosition = mouseGridPosition;
                float cellSize = LevelGrid.Instance.GetCellSize();
                mouseGridArrowVisual.position = new Vector3(
                    mouseGridPosition.x * cellSize,
                    mouseGridArrowVisualYOffset,
                    mouseGridPosition.z * cellSize
                );

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
                }
                else
                {
                    mouseGridVisual.gameObject.SetActive(false);
                }

                if (aoeEnabled)
                {
                    SetAOEVisual(false, (0, 0), GridSystemVisual.GridVisualType.White);
                    SetAOEVisual(true, currentRange, currentVisualType);
                }
            }
        }
        else
        {
            ToggleMouseVisibility(false);
            return;
        }
    }

    private void SetAOEVisual(
        bool enable,
        (int, int) range,
        GridSystemVisual.GridVisualType visualType
    )
    {
        aoeEnabled = enable;
        if (enable)
        {
            currentRange = range;
            currentVisualType = visualType;

            range = (
                Mathf.RoundToInt((range.Item1 - 1) / 2),
                Mathf.RoundToInt((range.Item2 - 1) / 2)
            );
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(
                mouseGridVisual.position
            );

            if (range.Item1 != range.Item2)
            {
                GridPosition mouseOffset =
                    mouseGridPosition
                    - UnitActionSystem.Instance.GetSelectedUnit().GetGridPosition();
                //Debug.Log(mouseOffset);
                if (Mathf.Abs(mouseOffset.x) > Mathf.Abs(mouseOffset.z))
                {
                    (range.Item2, range.Item1) = (range.Item1, range.Item2);
                }
            }

            for (
                int x = mouseGridPosition.x - range.Item1;
                x <= mouseGridPosition.x + range.Item1;
                x++
            )
            {
                for (
                    int z = mouseGridPosition.z - range.Item2;
                    z <= mouseGridPosition.z + range.Item2;
                    z++
                )
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

    private void UpdateMouseVisual()
    {
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        SetAOEVisual(false, (0, 0), GridSystemVisual.GridVisualType.White);
        if (!selectedAction)
        {
            mouseGridVisualScript.Hide();
            return;
        }

        //GridSystemVisual.GridVisualType gridVisualType ;
        if (selectedAction.GetIsAOE())
        {
            SetAOEVisual(
                true,
                selectedAction.GetDamageArea(),
                GridSystemVisual.GridVisualType.White
            );
        }

        mouseGridVisualScript.Show(
            GridSystemVisual.Instance.GetGridVisualTypeMaterial(
                GridSystemVisual.GridVisualType.White
            )
        );
    }

    private void ToggleMouseVisibility(bool toggle)
    {
        mouseGridVisual.gameObject.SetActive(toggle);
        mouseGridArrowVisual.gameObject.SetActive(toggle);
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
