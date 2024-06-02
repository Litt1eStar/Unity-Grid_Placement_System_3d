using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{
    public Vector2Int gridPosition;
    public GridObject ref_grid;
    public BuildingSO buildingData;

    public GridObject(Vector2Int gridPosition)
    {        
        this.gridPosition = gridPosition;

        ref_grid = null;
        buildingData = null;
    }
}
