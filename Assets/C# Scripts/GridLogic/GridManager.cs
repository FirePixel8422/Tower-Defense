using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    private void Awake()
    {
        Instance = this;
    }


    public Renderer folow;
    public SpriteRenderer rangeRenderer;
    public LayerMask floor;
    public LayerMask path;

    public bool drawTileGizmos;

    public Vector3 gridPosition;
    public Vector3Int gridSize;
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
                if(Physics.Raycast(_worldPos + Vector3.up, Vector3.down, 20, path))
                {
                    _type = 1;
                }

                grid[x, z] = new GridObjectData()
                {
                    gridPos = new Vector2Int(x, z),
                    worldPos = _worldPos,
                    type = _type,
                };
            }
        }
        Destroy(WaveManager.Instance.GetComponent<MeshCollider>());
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100, floor))
        {
            Vector3 worldPos = hitInfo.point;
            GridObjectData gridData = GridObjectFromWorldPoint(worldPos);
            if (gridData.type != 1)
            {
                folow.material.color = Color.green;
                rangeRenderer.color = new Color(0.7619722f, 0.8740168f, 0.9547169f);
            }
            else
            {
                folow.material.color = Color.red;
                rangeRenderer.color = new Color(0.8943396f, 0.2309691f, 0.09955848f);
            }
            folow.transform.position = gridData.worldPos;
        }
    }
    public GridObjectData GridObjectFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = Mathf.Clamp01((worldPosition.x - gridPosition.x + gridSize.x / 2) / gridSize.x);
        float percentZ = Mathf.Clamp01((worldPosition.z - gridPosition.z + gridSize.z / 2) / gridSize.z);

        int x = Mathf.FloorToInt(percentX * gridSizeX);
        int z = Mathf.FloorToInt(percentZ * gridSizeZ);

        x = Mathf.Clamp(x, 0, gridSizeX - 1);
        z = Mathf.Clamp(z, 0, gridSizeZ - 1);

        return grid[x, z];
    }


    public GridObjectData GetGridData(Vector2Int gridPos)
    {
        gridPos.Clamp(new Vector2Int(0, 0), new Vector2Int(gridSizeX -1, gridSizeZ -1));
        return grid[gridPos.x, gridPos.y];
    }



    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(gridPosition, new Vector3(gridSize.x * tileSize, gridSize.y, gridSize.z * tileSize));

        if (drawTileGizmos)
        {
            gridSizeX = Mathf.RoundToInt(gridSize.x / tileSize);
            gridSizeZ = Mathf.RoundToInt(gridSize.z / tileSize);

            Vector3 worldBottomLeft = gridPosition - Vector3.right * gridSize.x / 2 - Vector3.forward * gridSize.z / 2;
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    Gizmos.DrawWireCube(worldBottomLeft + Vector3.right * (x * tileSize + tileSize / 2) + Vector3.forward * (z * tileSize + tileSize / 2), new Vector3(tileSize, gridSize.y, tileSize));
                }
            }
        }
    }
}