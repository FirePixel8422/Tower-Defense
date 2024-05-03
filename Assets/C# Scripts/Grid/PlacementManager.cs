using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    [HideInInspector]
    public GridManager gridManager;
    private Camera mainCam;

    public LayerMask floor;
    public LayerMask path;

    public GameObject preSpawnedTowerHolder;
    public GameObject towerUIHolder;

    public TowerCore tower;

    public bool isPlacingTower;


    private void Start()
    {
        gridManager = GridManager.Instance;
        mainCam = Camera.main;

        Button[] buttons = towerUIHolder.GetComponentsInChildren<Button>();
        TowerCore[] towers = preSpawnedTowerHolder.GetComponentsInChildren<TowerCore>();
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[index].onClick.AddListener(() => StartTowerPreview(towers[index]));
        }
    }

    public void OnConfirm(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (isPlacingTower)
            {
                TryPlaceTower();
            }
        }
    }

    public void TryPlaceTower()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100, floor))
        {
            GridObjectData gridData = gridManager.GridObjectFromWorldPoint(hitInfo.point);
            if (gridData.type == 0)
            {
                tower.transform.localPosition = Vector3.zero;
                tower = Instantiate(tower.gameObject, gridData.worldPos, Quaternion.identity).GetComponent<TowerCore>();
                tower.Init();

                gridManager.UpdateGridDataFieldType(gridData.gridPos, 2);
                isPlacingTower = false;
            }
        }
    }

    public void StartTowerPreview(TowerCore _tower)
    {
        tower = _tower;
        isPlacingTower = true;
    }


    private void Update()
    {
        //give player preview of tower
        if (isPlacingTower)
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100, floor))
            {
                GridObjectData gridData = gridManager.GridObjectFromWorldPoint(hitInfo.point);
                if (gridData.type == 0)
                {
                    tower.towerPreviewRenderer.color = new Color(0.7619722f, 0.8740168f, 0.9547169f);
                }
                else
                {
                    tower.towerPreviewRenderer.color = new Color(0.8943396f, 0.2309691f, 0.09955848f);
                }
                tower.transform.position = gridData.worldPos;
            }
        }
    }
}
