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

    private readonly float mouseGridVisualYOffset = 0.1f;
    private readonly float mouseGridArrowVisualYOffset = 2f;

    private bool aoeEnabled;
    private (int, int) currentRange;
    private AOEType currentAOEType;
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
            mouseGridArrowVisual.gameObject.SetActive(true);
            mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
            if (
                (mouseGridPosition != currentMousePosition)
                && LevelGrid.Instance.IsValidGridPosition(mouseGridPosition)
            )
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
                    DisableAOEVisual();
                    SetAOEVisual(true, currentRange, currentAOEType, currentVisualType);
                }
            }
        }
        else
        {
            ToggleMouseVisibility(false);
            return;
        }
    }

    private void DisableAOEVisual()
    {
        SetAOEVisual(false, (0, 0), AOEType.Cube, GridSystemVisual.GridVisualType.White);
    }

    private void SetAOEVisual(
        bool enable,
        (int, int) range,
        AOEType aOEType,
        GridSystemVisual.GridVisualType visualType
    )
    {
        aoeEnabled = enable;
        if (enable)
        {
            currentRange = range;
            currentAOEType = aOEType;
            currentVisualType = visualType;

            // range = (
            //     Mathf.RoundToInt((range.Item1 - 1) / 2),
            //     Mathf.RoundToInt((range.Item2 - 1) / 2)
            // );
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(
                mouseGridVisual.position
            );

            GridPosition mouseOffset =
                mouseGridPosition - UnitActionSystem.Instance.GetSelectedUnit().GetGridPosition();

            switch (aOEType)
            {
                default:
                case AOEType.Cube:

                    range = (
                        Mathf.RoundToInt((range.Item1 - 1) / 2),
                        Mathf.RoundToInt((range.Item2 - 1) / 2)
                    );

                    if (range.Item1 != range.Item2)
                    {
                        if (Mathf.Abs(mouseOffset.x) > Mathf.Abs(mouseOffset.z))
                        {
                            (range.Item2, range.Item1) = (range.Item1, range.Item2);
                        }
                    }

                    for (int x = -range.Item1; x <= range.Item1; x++)
                    {
                        for (int z = -range.Item2; z <= range.Item2; z++)
                        {
                            GridPosition testGridPosition =
                                mouseGridPosition + new GridPosition(x, z);
                            SpawnAOEVisual(testGridPosition, visualType);
                        }
                    }
                    break;
                case AOEType.Sphere:
                    for (int x = -range.Item1; x <= range.Item1; x++)
                    {
                        for (int z = -range.Item1; z <= range.Item1; z++)
                        {
                            int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                            if (testDistance > range.Item1)
                            {
                                continue;
                            }

                            GridPosition testGridPosition =
                                mouseGridPosition + new GridPosition(x, z);
                            SpawnAOEVisual(testGridPosition, visualType);
                        }
                    }
                    break;
                case AOEType.Line:

                    GridPosition aoeCentre =
                        mouseGridPosition - mouseOffset + (mouseOffset * ((range.Item2 + 1) / 2));
                    Debug.Log("AOE Centre = " + aoeCentre);

                    range = (
                        Mathf.FloorToInt((range.Item1 - 1) / 2),
                        Mathf.FloorToInt((range.Item2 - 1) / 2)
                    );

                    if (range.Item1 != range.Item2)
                    {
                        if (Mathf.Abs(mouseOffset.x) > Mathf.Abs(mouseOffset.z))
                        {
                            (range.Item2, range.Item1) = (range.Item1, range.Item2);
                        }
                    }

                    for (int x = -range.Item1; x <= range.Item1; x++)
                    {
                        for (int z = -range.Item2; z <= range.Item2; z++)
                        {
                            GridPosition offsetGridPosition = new GridPosition(x, z);
                            GridPosition testGridPosition = aoeCentre + offsetGridPosition;

                            SpawnAOEVisual(testGridPosition, visualType);
                        }
                    }
                    break;
                case AOEType.Cone:
                    Debug.Log("Not implemented yet");
                    break;
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

    private void SpawnAOEVisual(
        GridPosition spawnLocation,
        GridSystemVisual.GridVisualType visualType
    )
    {
        Vector3 newMouseVisualSpawn =
            LevelGrid.Instance.GetWorldPosition(spawnLocation)
            + new Vector3(0, mouseGridVisualYOffset, 0);
        Transform newMouseVisual = Instantiate(
            mouseGridVisualPrefab,
            newMouseVisualSpawn,
            Quaternion.identity
        ).transform;
        newMouseVisual
            .GetComponent<GridSystemVisualSingle>()
            .Show(GridSystemVisual.Instance.GetGridVisualTypeMaterial(visualType));
        newMouseVisual.GetComponent<GridSystemVisualSingle>().ToggleTransparencyOscillation(true);
        mouseGridVisualAOE.Add(newMouseVisual);
        newMouseVisual.parent = mouseGridVisual;
    }

    private void UpdateMouseVisual()
    {
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        DisableAOEVisual();
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
                selectedAction.GetAOEType(),
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
