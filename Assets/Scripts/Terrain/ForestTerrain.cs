using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestTerrain : MonoBehaviour, ITerrainEffect
{
    private readonly StatBonus forestStatBonus = new(0, 0, 0, 2);

    private void Start()
    {
        GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetTerrainEffectAtGridPosition(gridPosition, this);
    }

    public void ApplyEffect(Unit unit)
    {
        unit.GetUnitStats().currentStatBonus += forestStatBonus;
    }

    public bool GetIsDifficultTerrain()
    {
        return true;
    }

    public void RemoveEffect(Unit unit)
    {
        unit.GetUnitStats().currentStatBonus -= forestStatBonus;
    }
}
