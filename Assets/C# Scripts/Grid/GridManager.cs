using UnityEngine;


public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    private void Awake()
    {
        Instance = this;
    }


    public bool drawMasterGizmos;
    public bool drawTileGizmos;
    public bool drawTileDataGizmos;

    public Vector3 gridPosition;
    public Vector3 gridSize;
    public float tileSize;

    private int gridSizeX, gridSizeZ;

    [SerializeField] private GridObjectData[,] grid;



    private void Start()
    {
        CreateGrid();
    }
    private void CreateGrid()
    {
        gridSizeX = Mathf.RoundToInt(gridSize.x / tileSize);
        gridSizeZ = Mathf.RoundToInt(gridSize.z / tileSize);

        grid = new GridObjectData[gridSizeX, gridSizeZ];

        Vector3 worldBottomLeft = gridPosition - Vector3.right * gridSize.x / 2 - Vector3.forward * gridSize.z / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                Vector3 _worldPos = worldBottomLeft + Vector3.right * (x * tileSize + tileSize / 2) + Vector3.forward * (z * tileSize + tileSize / 2);

                int _type = 0;
                if (Physics.Raycast(_worldPos + Vector3.up, Vector3.down, 20, SelectionManager.Instance.path))
                {
                    _type = 1;
                }

                grid[x, z] = new GridObjectData()
                {
                    gridPos = new Vector2Int(x, z),
                    worldPos = _worldPos,
                    coreType = _type,
                    type = _type,
                };
            }
        }
        Destroy(WaveManager.Instance.GetComponent<MeshCollider>());
    }


    public GridObjectData GridObjectFromWorldPoint(Vector3 worldPosition)
    {
        worldPosition -= gridPosition;
        float percentX = Mathf.Clamp01((worldPosition.x + gridSize.x / 2) / gridSize.x);
        float percentZ = Mathf.Clamp01((worldPosition.z + gridSize.z / 2) / gridSize.z);

        int x = Mathf.FloorToInt(percentX * gridSizeX);
        int z = Mathf.FloorToInt(percentZ * gridSizeZ);

        x = Mathf.Clamp(x, 0, gridSizeX - 1);
        z = Mathf.Clamp(z, 0, gridSizeZ - 1);

        return grid[x, z];
    }


    public GridObjectData GetGridData(Vector2Int gridPos)
    {
        gridPos.Clamp(new Vector2Int(0, 0), new Vector2Int(gridSizeX - 1, gridSizeZ - 1));
        return grid[gridPos.x, gridPos.y];
    }

    public void UpdateGridDataFieldType(Vector2Int gridPos, int newType)
    {
        grid[gridPos.x, gridPos.y].type = newType;
    }
    public void UpdateGridDataFieldType(Vector2Int gridPos, int newType, TowerCore tower)
    {
        grid[gridPos.x, gridPos.y].type = newType;
        grid[gridPos.x, gridPos.y].tower = tower;
    }



    public void OnDrawGizmos()
    {
        if (drawMasterGizmos == false)
        {
            return;
        }

        Gizmos.DrawWireCube(gridPosition, new Vector3(gridSize.x, gridSize.y, gridSize.z));

        Vector3 worldBottomLeft = gridPosition - Vector3.right * gridSize.x / 2 - Vector3.forward * gridSize.z / 2;
        if (drawTileGizmos)
        {
            gridSizeX = Mathf.RoundToInt(gridSize.x / tileSize);
            gridSizeZ = Mathf.RoundToInt(gridSize.z / tileSize);
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireCube(worldBottomLeft + Vector3.right * (x * tileSize + tileSize / 2) + Vector3.forward * (z * tileSize + tileSize / 2), new Vector3(tileSize, gridSize.y, tileSize));
                }
            }
        }
        if (drawTileDataGizmos && Application.isPlaying)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    if (grid[x, z].type == 1)
                    {
                        Gizmos.color = Color.red;
                    }
                    else
                    {
                        Gizmos.color = Color.green;
                    }
                    Gizmos.DrawCube(worldBottomLeft + Vector3.right * (x * tileSize + tileSize / 2) + Vector3.forward * (z * tileSize + tileSize / 2), new Vector3(tileSize / 2, tileSize / 2, tileSize / 2));
                }
            }
        }
    }
}