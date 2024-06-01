using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Assets.Scripts
{
    public enum UserOperation
    {
        None,
        Placement,
        Delete
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public GridSystem gridSystem { get; private set; }
        public UserOperation operation { private get; set; }

        [SerializeField] private BuildingSO buildingData;
        [SerializeField] private LayerMask buildingLayerMask;
        [SerializeField] private int width = 20;
        [SerializeField] private int depth = 20;
        [SerializeField] private float cellSize = 10f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            DontDestroyOnLoad(this.gameObject);
            Instance = this;

            gridSystem = new GridSystem(width, depth, cellSize);
            operation = UserOperation.None;
        }

        private void Start()
        {
            buildingData = null;
            operation = UserOperation.None;
        }

        void Update()
        {

            if (Input.GetKeyDown(KeyCode.F2))
            {
                GridVisualizer.Instance.ToggleGridVisualization();
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity))
            {
                if (operation == UserOperation.None)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        //Selection Grid
                        Vector2Int gridPosition = gridSystem.GetGridPositionFromWorldPosition(hitInfo.point);
                        GridObject selectedGrid = gridSystem.GetGridObject(gridPosition.x, gridPosition.y);
                        Debug.Log(selectedGrid);
                    }
                }

                if (buildingData == null) return;

                if (operation == UserOperation.Placement)
                {
                    GridVisualizer.Instance.StartVisualizeOverlayGrid(hitInfo.point, buildingData.buildingSize);
                }

                if (Input.GetMouseButtonDown(1))
                {
                    switch (operation)
                    {
                        case UserOperation.Placement:
                            HandlePlacementOperation(hitInfo);
                            break;
                        case UserOperation.Delete:
                            HandleDeleteOperation(hitInfo);
                            break;

                    }
                }
            }

        }

        private void HandlePlacementOperation(RaycastHit hitInfo)
        {
            Vector2Int clickedPosition = gridSystem.GetGridPositionFromWorldPosition(hitInfo.point);
            gridSystem.PlaceBuilding(clickedPosition, buildingData);   
        }

        private void HandleDeleteOperation(RaycastHit hitInfo)
        {
            Vector2Int clickedPosition = gridSystem.GetGridPositionFromWorldPosition(hitInfo.point);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity,buildingLayerMask))
            {
                Debug.Log(hit.transform.gameObject.name);
                GameObject buildingOnScene = hit.transform.gameObject;
                gridSystem.DeleteBuilding(clickedPosition, buildingOnScene);
            }
        }


        public void SetBuildingData(BuildingSO buildingData)
        {
            this.buildingData = buildingData;
        }
    }


}