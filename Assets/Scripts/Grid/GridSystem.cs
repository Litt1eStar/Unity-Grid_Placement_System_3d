using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem
{
    public int width {  get; private set; }
    public int depth { get; private set; }
    public float cellSize { get; private set; }
    private Dictionary<Vector2Int, GridObject> grids;
    public GridSystem(int width, int depth, float cellSize)
    {
        this.width = width;
        this.depth = depth;
        this.cellSize = cellSize;

        grids = new Dictionary<Vector2Int, GridObject>();

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Vector2Int gridPosition = new Vector2Int(x, z);
                grids[gridPosition] = new GridObject(0, gridPosition);                
            }
        }
    }

    public void PlaceBuilding(Vector2Int clickedPosition, BuildingSO buildingData)
    {
        Vector2Int buildingSize = buildingData.buildingSize;

        if(!CanBuild(clickedPosition, buildingSize))
        {
            Debug.LogWarning("These grid can't build");
            return;
        }

        GridObject headGrid = GetGridObject(clickedPosition.x, clickedPosition.y);
        headGrid.buildingData = buildingData;
        
        HandleCreatePrefab(clickedPosition, buildingData.prefab);

        for (int x = clickedPosition.x; x < clickedPosition.x + buildingSize.x; x++)
        {
            for (int z = clickedPosition.y; z < clickedPosition.y + buildingSize.y; z++)
            {
                GridObject gridObject = GetGridObject(x, z);
                gridObject.ref_grid = headGrid;

                GridVisualizer.Instance.SetCellInfo(new Vector2Int(x, z), buildingData, true);
            }
        }
    }

    private void HandleCreatePrefab(Vector2Int gridPos, GameObject buildingPrefab)
    {
        GameObject prefab = GameObject.Instantiate(buildingPrefab, new Vector3(gridPos.x, 0, gridPos.y) * cellSize, Quaternion.identity);
        float prefabSize = 0.1f * cellSize;
        prefab.transform.localScale = new Vector3(prefabSize, prefabSize, prefabSize);
    }

    public void DeleteBuilding(Vector2Int clickedGridPosition, GameObject buildingOnScene)
    {
        GridObject gridObject = GetGridObject(clickedGridPosition.x, clickedGridPosition.y);

        if(gridObject == null)
        {
            Debug.LogWarning("GridObject is null");
            return;
        }
        GridObject headGrid = gridObject.ref_grid;
        Vector2Int startPos = headGrid.gridPosition;

        BuildingSO buildingData = headGrid.buildingData;
        Vector2Int buildingSize = buildingData.buildingSize;

        headGrid.buildingData = null;
        GameObject.Destroy(buildingOnScene);

        for (int x = startPos.x; x < startPos.x + buildingSize.x; x++)
        {
            for (int z = startPos.y; z < startPos.y + buildingSize.y; z++)
            {
                GridObject gridData = GetGridObject(x, z);
                gridData.ref_grid = null;

                GridVisualizer.Instance.SetCellInfo(new Vector2Int(x, z), buildingData, false);
            }
        }

        
    }

    private bool CanBuild(Vector2Int startPos, Vector2Int buildingSize)
    {
        if(startPos.x < 0 || startPos.y < 0 || startPos.x >= width || startPos.y >= depth) return false;

        for (int x = startPos.x; x < startPos.x + buildingSize.x; x++)
        {
            for (int z = startPos.y; z < startPos.y + buildingSize.y; z++)
            {
                GridObject gridObject = grids[new Vector2Int(x, z)];
                if(gridObject.ref_grid != null)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public GridObject GetGridObject(int x, int z)
    {
        Vector2Int gridPosition = new Vector2Int(x, z);
        if (!grids.ContainsKey(gridPosition))
        {
            return null;
        }
        return grids[gridPosition];
    }

    public Vector2Int GetGridPositionFromWorldPosition(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / cellSize);
        int z = Mathf.FloorToInt(worldPosition.z / cellSize);

        if (x < 0 || z < 0 || x > width || z > depth)
        {
            return new Vector2Int(-1, -1);
        }

        return new Vector2Int(x, z);
    }
    public Vector3 GetWorldPositionFromGridPosition(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.y) * cellSize;
    }
}
