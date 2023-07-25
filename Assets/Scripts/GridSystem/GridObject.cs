using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{
    //Each tile on the Grid is a GridObject
    private List<Unit> gridUnitList; //All units currently on the tile
    private GridSystem<GridObject> gridSystem;

    private GridPosition gridPosition; //Position of this tile
    private IInteractable interactable;

    public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition) //Constructor
    {
        this.gridPosition = gridPosition;
        this.gridSystem = gridSystem;
        gridUnitList = new List<Unit>();
    }

    public GridPosition GetGridPosition()
    {
        return this.gridPosition;
    }

    public List<Unit> GetGridUnitList()
    {
        return gridUnitList;
    }

    public void AddGridUnit(Unit gridUnit)
    {
        gridUnitList.Add(gridUnit);
    }

    public void RemoveGridUnit(Unit gridUnit)
    {
        gridUnitList.Remove(gridUnit);
    }

    public bool HasAnyUnit()
    {
        return gridUnitList.Count > 0;
    }

    //Returns list of Units on the tile
    public override string ToString()
    {
        string unitString = "";
        foreach (Unit unit in gridUnitList)
        {
            unitString += unit + "\n";
        }
        return "\n" + unitString;
    }

    public Unit GetUnit()
    {
        if (HasAnyUnit())
        {
            return gridUnitList[0];
        }
        else
        {
            return null;
        }
    }

    public IInteractable GetInteractable()
    {
        return interactable;
    }

    public void SetInteractable(IInteractable interactable)
    {
        this.interactable = interactable;
    }
}
