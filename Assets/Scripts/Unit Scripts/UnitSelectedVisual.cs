using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour
{
    [SerializeField]
    private Unit unit;

    //Logic for changing where the SelectedUnit visual is based on GetSelectedUnit and the OnSelectedUnitChanged event
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        UnitActionSystem.Instance.OnUnitActionFinished += UnitActionSystem_OnSelectedUnitChanged;
        UpdateVisual();
    }

    private void OnDisable()
    {
        UnitActionSystem.Instance.OnUnitActionFinished -= UnitActionSystem_OnSelectedUnitChanged;
    }

    private void UnitActionSystem_OnSelectedUnitChanged()
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (!meshRenderer)
        {
            return;
        }

        if (unit == UnitActionSystem.Instance.GetSelectedUnit())
        {
            meshRenderer.enabled = true;
        }
        else
        {
            meshRenderer.enabled = false;
        }
    }

    private void OnDestroy()
    {
        UnitActionSystem.Instance.OnUnitActionFinished -= UnitActionSystem_OnSelectedUnitChanged;
    }
}
