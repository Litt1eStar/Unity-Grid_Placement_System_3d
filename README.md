# Unity - Grid Placement System 3D

## Overview
This project implements a Grid Placement System in 3D, allowing for the placement and deletion of buildings on a grid. It serves as an experimental project to develop a grid system from scratch. While the logic for the grid placement system is straightforward, the visualization aspect of the grid is somewhat tricky. The grid visualization feature is particularly useful during the development stage, enabling developers to see the grid and ensure the accuracy of their actions. It is also helpful for testing purposes.


https://github.com/Litt1eStar/Unity-Grid_Placement_System_3d/assets/90139527/be4fe716-8cad-4bf3-8e2d-466a34733a4e


## Key Features
- **Grid Placement and Deletion**: Place and delete buildings on a grid with intuitive logic.
- **Grid Visualization**: Visualize the grid using LineRenderer for development and testing purposes.
- **Performance Considerations**: Includes performance comparisons for grid visualization, as real-time rendering can be resource-intensive.

## Main Classes

### GridObject.cs
**File**: `Assets/Scripts/Grid/GridObject.cs`

**Description**:
Represents the data for each grid cell.

**Fields**:
- `Vector2Int gridPosition`: Position of the grid cell.
- `GridObject ref_grid`: Reference to the head grid object.
- `BuildingSO buildingData`: Data related to the building on this grid.

### GridSystem.cs
**File**: `Assets/Scripts/Grid/GridSystem.cs`

**Description**:
Contains the logic for the grid system, including methods for placing and deleting buildings.

**Fields**:
- `Dictionary<Vector2Int, GridObject> grids`: Stores grid data with grid position as the key and GridObject as the value.

**Key Methods**:
- `PlaceBuilding(Vector2Int clickedPosition, BuildingSO buildingData)`: Places a building on the grid.
- `DeleteBuilding(Vector2Int clickedGridPosition, GameObject buildingOnScene)`: Deletes a building from the grid.
- `CanBuild(Vector2Int startPos, Vector2Int buildingSize)`: Checks if a building can be placed at the specified position.
- `GetGridObject(int x, int z)`: Retrieves the grid object at the specified position.
- `GetGridPositionFromWorldPosition(Vector3 worldPosition)`: Converts world position to grid position.
- `GetWorldPositionFromGridPosition(Vector2Int gridPosition)`: Converts grid position to world position.

### GridVisualizer.cs
**File**: `Assets/Scripts/Grid/GridVisualizer.cs`

**Description**:
Uses LineRenderer to visualize the grid. This class provides methods to draw the grid, highlight cells, and display overlay information.

**Key Fields**:
- `Dictionary<Vector2Int, LineRenderer[]> cellLineRenderers`: Stores line renderers for each grid cell.
- `Dictionary<Vector2Int, TextMeshPro> textMeshes`: Stores text objects for each grid cell.
- `Dictionary<Vector2Int, GameObject> overlayGameObjects`: Stores overlay objects for each grid cell.

**Key Methods**:
- `DrawGrid()`: Draws the entire grid.
- `CreateCellLines(Vector2Int gridPos, Vector3 cellStart)`: Creates lines for a single grid cell.
- `HighlightGridCell(Vector2Int gridPos, Color color)`: Highlights a specified grid cell.
- `ResetAllGridHighlights()`: Resets highlights for all grid cells.
- `SetCellInfo(Vector2Int gridPos, BuildingSO buildingData, bool isBuildingPlaced)`: Updates the cell information based on building data.

## Usage

### Placing a Building
1. The user clicks on a grid cell to place a building.
2. The `PlaceBuilding` method is called with the clicked position and building data.
3. The method assigns the building data to the head grid and updates the reference for all relevant grid cells.

### Deleting a Building
1. The user clicks on a grid cell that contains a building to delete it.
2. The `DeleteBuilding` method is called with the clicked position and the building game object.
3. The method clears the building data from the grid and destroys the building game object.

### Visualizing the Grid
1. The `DrawGrid` method is called to draw the grid using LineRenderer.
2. The `HighlightGridCell` method is used to highlight cells, which is useful for overlay visualization when placing buildings.

## Performance Considerations
The grid visualization feature is resource-intensive, especially with real-time rendering. It's intended primarily for development and testing rather than actual gameplay.

## Grid Visualization
To visualize a grid cell, lines are drawn to create a square. The lines are drawn in this order: bottom line, top line, left line, and right line.

#### Here is method used to create visualize cell:
#### GridVisualizer.cs
```csharp
private void CreateCellLines(Vector2Int gridPos, Vector3 cellStart)
{
    LineRenderer[] lines = new LineRenderer[4];

    lines[0] = CreateLineRenderer(cellStart, cellStart + new Vector3(cellSize, 0, 0)); // Bottom
    lines[1] = CreateLineRenderer(cellStart + new Vector3(0, 0, cellSize), cellStart + new Vector3(cellSize, 0, cellSize)); // Top
    lines[2] = CreateLineRenderer(cellStart, cellStart + new Vector3(0, 0, cellSize)); // Left
    lines[3] = CreateLineRenderer(cellStart + new Vector3(cellSize, 0, 0), cellStart + new Vector3(cellSize, 0, cellSize)); // Right

    cellLineRenderers[gridPos] = lines;
}

private LineRenderer CreateLineRenderer(Vector3 start, Vector3 end)
{
    GameObject lineObject = new GameObject("GridLine");
    lineObject.transform.parent = lineRendererParent;
    LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

    lineRenderer.startWidth = 0.1f;
    lineRenderer.endWidth = 0.1f;
    lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    lineRenderer.useWorldSpace = true;
    lineRenderer.positionCount = 2;

    lineRenderer.SetPosition(0, start);
    lineRenderer.SetPosition(1, end);

    lineRenderer.startColor = defaultColor;
    lineRenderer.endColor = defaultColor;

    return lineRenderer;
}
```

if you want to highlihgt grid cell then you can change color of lineRenderer by use gridPosition to access to lineRenderer.
#### Example
```csharp
if (cellLineRenderers.ContainsKey(gridPos))
{
    LineRenderer[] lines = cellLineRenderers[gridPos];
    foreach (var line in lines)
    {
        line.startColor = color;
        line.endColor = color;
        line.material.color = color;
    }
}
```
![image](https://github.com/Litt1eStar/Unity-Grid_Placement_System_3d/assets/90139527/8625335c-e9b1-4ada-adfc-f5d1a8e243f8)

