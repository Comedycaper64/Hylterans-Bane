using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem<TGridObject>
{
    //**The Grid**
    private int width;  //How wide it is
    private int height; //How high it is
    private float cellSize; //How large each tile is
    private TGridObject[,] gridObjectArray;  //All of the tiles on this grid, stored in a 2D array based on what their gridposition is

    public GridSystem(int width, int height, float cellSize, Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObject)
    //Constructor
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridObjectArray = new TGridObject[width, height];
        for(int x = 0; x < width; x++)
        {
            for(int z = 0; z < height; z++)
            {     
                GridPosition gridPosition = new GridPosition(x, z);
                gridObjectArray[x, z] = createGridObject(this, gridPosition);    
            }
        }
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.z) * cellSize;
    }

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return new GridPosition(
            Mathf.RoundToInt(worldPosition.x / cellSize),
            Mathf.RoundToInt(worldPosition.z / cellSize)
        );  
    }

    //Makes all the GridDebugObjects to show the GridPosition + current units of all tiles
    public void CreateDebugObjects(Transform debugPrefab)
    {
        for(int x = 0; x < width; x++)
        {
            for(int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);

                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
                GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                gridDebugObject.SetGridObject(GetGridObject(gridPosition));
            }
        }

    }

    //You can get a specific tile based on its GridPosition :ooo what a good 2D array that is
    public TGridObject GetGridObject(GridPosition gridPosition)
    {
        return gridObjectArray[gridPosition.x, gridPosition.z];
    }

    private bool CheckIfTileExists(GridPosition gridPosition)
    {
        float tileCheckRadius = .5f;
        Collider[] colliderArray = Physics.OverlapSphere(new Vector3(gridPosition.x * cellSize, 0, gridPosition.z * cellSize), tileCheckRadius);
        foreach (Collider collider in colliderArray)
        {
            if (collider.gameObject.layer == 6)
            {
                return true;
            }
        }
        return false;
    }

    //Tests if the tile is within the Grid
    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        return  gridPosition.x >= 0 && 
                gridPosition.z >= 0 && 
                gridPosition.x < width && 
                gridPosition.z < height &&
                CheckIfTileExists(gridPosition);
    }

    public int GetWidth() //of the Grid
    {
        return width;
    }

    public int GetHeight()//^
    {
        return height;
    }

    public float GetCellSize()
    {
        return cellSize;
    }

}
