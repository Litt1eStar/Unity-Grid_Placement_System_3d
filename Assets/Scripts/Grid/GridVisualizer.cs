using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Assets.Scripts;
using System;

public class GridVisualizer : MonoBehaviour
{
    public static GridVisualizer Instance;

    public GridSystem gridSystem;
    public bool isVisualizing = false;
    private int width;
    private int depth;
    private float cellSize;

    [SerializeField] private Transform lineRendererParent;
    [SerializeField] private GameObject gridText;
    [SerializeField] private GameObject overlayObj;
    [SerializeField] private Color overlayColor = Color.green;
    [SerializeField] private Color errorColor = Color.red;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color usedColor = Color.yellow;

    //Testing Purpose
    public bool shownGrid;

    private Dictionary<Vector2Int, LineRenderer[]> cellLineRenderers = new Dictionary<Vector2Int, LineRenderer[]>();
    private Dictionary<Vector2Int, TextMeshPro> textMeshes = new Dictionary<Vector2Int, TextMeshPro>();
    private Dictionary<Vector2Int, GameObject> overlayGamObjects = new Dictionary<Vector2Int, GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
        Instance = this;
    }

    private void Start()
    {
        gridSystem = GameManager.Instance.gridSystem;
        width = gridSystem.width;
        depth = gridSystem.depth;
        cellSize = gridSystem.cellSize;

        lineRendererParent.gameObject.SetActive(isVisualizing);

        if(shownGrid)
            DrawGrid();
    }

    private void DrawGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Vector2Int gridPos = new Vector2Int(x, z);
                Vector3 cellStart = gridSystem.GetWorldPositionFromGridPosition(gridPos);
                CreateCellLines(gridPos, cellStart);

                Vector3 cellCenter = cellStart + new Vector3(cellSize / 2, 0, cellSize / 2);
                CreateTextObject(gridPos, $"({x}, {z})", cellCenter);
            }
        }
    }

    //Used to Generate Cell and 1 cell = 4 lines
    //NOTE: I have put picture to shown that how this method is working in README.md
    private void CreateCellLines(Vector2Int gridPos, Vector3 cellStart)
    {
        LineRenderer[] lines = new LineRenderer[4];

        lines[0] = CreateLineRenderer(cellStart, cellStart + new Vector3(cellSize, 0, 0)); // Bottom
        lines[1] = CreateLineRenderer(cellStart + new Vector3(0, 0, cellSize), cellStart + new Vector3(cellSize, 0, cellSize)); // Top
        lines[2] = CreateLineRenderer(cellStart, cellStart + new Vector3(0, 0, cellSize)); // Left
        lines[3] = CreateLineRenderer(cellStart + new Vector3(cellSize, 0, 0), cellStart + new Vector3(cellSize, 0, cellSize)); // Right

        cellLineRenderers[gridPos] = lines;
    }

    //Used to Generate single line
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

    //Used to generate textObject of Cell that will located at the center of cell
    private void CreateTextObject(Vector2Int gridPos, string text, Vector3 position)
    {
        GameObject textObj = Instantiate(gridText, lineRendererParent.position, Quaternion.identity, lineRendererParent);
        TextMeshPro textInfo = textObj.GetComponent<TextMeshPro>();

        textInfo.fontSize = CalculateFontSize(cellSize);
        textInfo.text = text;
        textInfo.alignment = TextAlignmentOptions.Center;
        textInfo.transform.position = position + new Vector3(5f, 0.1f, 0);
        textInfo.transform.eulerAngles = new Vector3(90, 0, 0);

        textMeshes[gridPos] = textInfo;
    }
    
    //This method is not my stuff | It's CHATGPT4 stuff :3
    private float CalculateFontSize(float cellSize)
    {        
        float baselineCellSize = 20f;
        float baselineFontSize = 45f;        
        float scaleFactor = baselineFontSize / baselineCellSize;
        float fontSize = cellSize * scaleFactor;

        fontSize = Mathf.Clamp(fontSize, 1, 30);
        return fontSize;
    }


    public void StartVisualizeOverlayGrid(Vector3 hitInfo, Vector2Int buildingSize)
    {
        ResetAllGridHighlights();

        Vector2Int clickedPosition = gridSystem.GetGridPositionFromWorldPosition(hitInfo);

        //If clickedPosition is out of grid range
        if (clickedPosition == new Vector2Int(-1, -1))
        {
            ResetAllGridHighlights();
            return;
        }

        for (int x = clickedPosition.x; x < clickedPosition.x + buildingSize.x; x++)
        {
            for (int z = clickedPosition.y; z < clickedPosition.y + buildingSize.y; z++)
            {
                if(x < 0 || z < 0 || x >= width || z >= depth)
                {
                    return;
                }
                GridObject gridObject = gridSystem.GetGridObject(x, z);
                Color color = gridObject.ref_grid == null ? overlayColor : errorColor;
               
                HighlightGridCell(new Vector2Int(x, z), color);
            }
        }
    }

    //Basically, This method just change color of line, text and create overlayObj that's used in real-game
    private void HighlightGridCell(Vector2Int gridPos, Color color)
    {
        if (cellLineRenderers.ContainsKey(gridPos))
        {
            LineRenderer[] lines = cellLineRenderers[gridPos];
            foreach (var line in lines)
            {
                line.startColor = color;
                line.endColor = color;
                line.material.color = color;
            }

            UpdateNeighboringCells(gridPos, color);
        }

        if (textMeshes.ContainsKey(gridPos))
        {
            TextMeshPro textInfo = textMeshes[gridPos];
            if (textInfo.color != usedColor) // Preserve used color
            {
                textInfo.color = color;
            }
        }

        if (!isVisualizing)
        {
            Vector3 position = gridSystem.GetWorldPositionFromGridPosition(gridPos) + new Vector3(cellSize / 2, 0, cellSize / 2);
            GameObject overlayGameobject = GameObject.Instantiate(overlayObj, position, Quaternion.identity);
            overlayGameobject.GetComponent<Renderer>().material.color = color;
            overlayGameobject.transform.localScale = new Vector3(cellSize / 10, 0, cellSize / 10);
            overlayGamObjects[gridPos] = overlayGameobject; 
        }
    }

    private void UpdateNeighboringCells(Vector2Int gridPos, Color color)
    {
        Vector2Int[] neighborOffsets = new Vector2Int[] {
            new Vector2Int(-1, 0), // Left
            new Vector2Int(1, 0),  // Right
            new Vector2Int(0, -1), // Bottom
            new Vector2Int(0, 1)   // Top
        };

        foreach (var offset in neighborOffsets)
        {
            Vector2Int neighborPos = gridPos + offset;
            if (cellLineRenderers.ContainsKey(neighborPos))
            {
                LineRenderer[] lines = cellLineRenderers[neighborPos];

                if (offset == new Vector2Int(-1, 0))
                {
                    // Left neighbor
                    lines[3].startColor = color; // Right line
                    lines[3].endColor = color;
                }
                else if (offset == new Vector2Int(1, 0))
                {
                    // Right neighbor
                    lines[2].startColor = color; // Left line
                    lines[2].endColor = color;
                }
                else if (offset == new Vector2Int(0, -1))
                {
                    // Bottom neighbor
                    lines[1].startColor = color; // Top line
                    lines[1].endColor = color;
                }
                else if (offset == new Vector2Int(0, 1))
                {
                    // Top neighbor
                    lines[0].startColor = color; // Bottom line
                    lines[0].endColor = color;
                }
            }
        }
    }

    private void ResetGridCellHighlight(Vector2Int gridPos)
    {
        if (cellLineRenderers.ContainsKey(gridPos))
        {
            LineRenderer[] lines = cellLineRenderers[gridPos];
            foreach (var line in lines)
            {
                line.startColor = defaultColor;
                line.endColor = defaultColor;
                line.material.color = defaultColor;
            }

            UpdateNeighboringCells(gridPos, defaultColor);
        }

        if (textMeshes.ContainsKey(gridPos))
        {
            TextMeshPro textInfo = textMeshes[gridPos];
            if (textInfo.color != usedColor) // Preserve used color
            {
                textInfo.color = defaultColor;
            }
        }
    }

    public void ResetAllGridHighlights()
    {
        foreach (var gridPos in cellLineRenderers.Keys)
        {
            ResetGridCellHighlight(gridPos);
        }

        foreach(var gridPos in overlayGamObjects.Keys)
        {
            ResetAllOverlayObj(gridPos);
        }
    }

    private void ResetAllOverlayObj(Vector2Int gridPos)
    {
        if (overlayGamObjects.ContainsKey(gridPos))
        {
            GameObject overlayObj = overlayGamObjects[gridPos];
            GameObject.Destroy(overlayObj);
        }
    }

    public void SetCellInfo(Vector2Int gridPos, BuildingSO buildingData, bool isBuildingPlaced)
    {
        if (textMeshes.ContainsKey(gridPos))
        {
            TextMeshPro textInfo = textMeshes[gridPos];
            if (textInfo == null)
                return;
            
            if (isBuildingPlaced)
            {
                textInfo.text = buildingData.buildingName;
                textInfo.color = usedColor;
            }
            else
            {
                textInfo.text = $"({gridPos.x}, {gridPos.y})";
                textInfo.color = defaultColor;
            }
        }
        else
        {
            Debug.LogWarning($"No text mesh found for grid position {gridPos}");
        }
    }

    public void ToggleGridVisualization()
    {
        isVisualizing = !isVisualizing;
        lineRendererParent.gameObject.SetActive(isVisualizing);
    }

}
