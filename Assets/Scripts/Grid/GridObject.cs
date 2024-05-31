using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{    
    public Vector2Int gridPosition {  get; private set; }
    public GridObject ref_grid;
    public BuildingSO buildingData;

    public GridObject(int value, Vector2Int gridPosition)
    {        
        this.gridPosition = gridPosition;

        ref_grid = null;
        buildingData = null;
    }

    public override string ToString()
    {
        if(buildingData == null && ref_grid == null)
            return $"GridPosition: {gridPosition}, Building: None, ref_grid: None";
        else
            return $"GridPosition: {gridPosition}, Building: {buildingData}, ref_grid: {ref_grid.gridPosition}";
    }
}
