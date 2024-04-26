using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance;
    private void Awake()
    {
        Instance = this;
    }


    public GridManager gridManager;
    public Camera mainCam;

    public LayerMask floor;
    public LayerMask path;


    public Renderer folow;
    public SpriteRenderer rangeRenderer;

    public bool isPlacingTower;



    private void Start()
    {
        gridManager = GridManager.Instance;
        mainCam = Camera.main;
    }


    void Update()
    {
        if (isPlacingTower)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100, floor))
            {
                Vector3 worldPos = hitInfo.point;
                GridObjectData gridData = gridManager.GridObjectFromWorldPoint(worldPos);
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
    }
}
