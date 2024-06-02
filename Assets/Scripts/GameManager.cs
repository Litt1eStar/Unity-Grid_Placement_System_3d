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
            if (Input.GetKeyDown(KeyCode.F2) && GridVisualizer.Instance.shownGrid)
            {
                GridVisualizer.Instance.ToggleGridVisualization();
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity))
            {
                Vector2Int gridPosition = gridSystem.GetGridPositionFromWorldPosition(hitInfo.point);

                if (operation == UserOperation.Placement)
                {
                    if (buildingData == null)
                        return;

                    GridVisualizer.Instance.StartVisualizeOverlayGrid(hitInfo.point, buildingData.buildingSize);
                }

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


        private void HandlePlacementOperation(RaycastHit hitInfo)
        {
            if (buildingData == null) return;

            Vector2Int clickedPosition = gridSystem.GetGridPositionFromWorldPosition(hitInfo.point);

            if (Input.GetMouseButtonDown(1))
            {
                gridSystem.PlaceBuilding(clickedPosition, buildingData);
            }
        }

        private void HandleDeleteOperation(RaycastHit hitInfo)
        {
            Vector2Int clickedPosition = gridSystem.GetGridPositionFromWorldPosition(hitInfo.point);

            if (Input.GetMouseButtonDown(1))
            {
                if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    return;

                GameObject buildingOnScene = hitInfo.collider.gameObject;
                gridSystem.DeleteBuilding(clickedPosition, buildingOnScene);
            }
        }

        public void SetBuildingData(BuildingSO buildingData)
        {
            this.buildingData = buildingData;
        }
    }
}
