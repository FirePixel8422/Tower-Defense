using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    private GridManager gridManager;
    private TowerManager towerManager;
    private Camera mainCam;

    public LayerMask floor;
    public LayerMask path;

    public GameObject preSpawnedTowerHolder;
    public GameObject towerUIHolder;

    public TowerCore selectedTower;

    public bool isPlacingTower;
    public bool towerSelected;


    private void Start()
    {
        gridManager = GridManager.Instance;
        towerManager = TowerManager.Instance;
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
            else
            {
                TrySelectTower();
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
                selectedTower.transform.localPosition = Vector3.zero;
                selectedTower = Instantiate(selectedTower.gameObject, gridData.worldPos, Quaternion.identity).GetComponent<TowerCore>();

                towerManager.spawnedTowerObj.Add(selectedTower);
                selectedTower.Init();

                gridManager.UpdateGridDataFieldType(gridData.gridPos, 2, selectedTower);
                isPlacingTower = false;
            }
        }
    }
    public void TrySelectTower()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100, floor))
        {
            GridObjectData gridData = gridManager.GridObjectFromWorldPoint(hitInfo.point);
            if (gridData.tower != null)
            {
                //deselect older selected tower
                if (towerSelected)
                {
                    selectedTower.towerPreviewRenderer.enabled = false;
                    selectedTower.SelectOrDeselectTower(false);
                }

                //select tower
                selectedTower = gridData.tower;
                selectedTower.towerPreviewRenderer.enabled = true;

                towerSelected = true;
                selectedTower.SelectOrDeselectTower(true);
                return;
            }
        }

        //deselect tower if clicked on empty space
        if (towerSelected)
        {
            selectedTower.towerPreviewRenderer.enabled = false;
            selectedTower.SelectOrDeselectTower(false);
        }
        towerSelected = false;
    }


    public void StartTowerPreview(TowerCore _selectedTower)
    {
        if (isPlacingTower)
        {
            selectedTower.transform.localPosition = Vector3.zero;
        }
        if (towerSelected)
        {
            selectedTower.towerPreviewRenderer.enabled = false;
            towerSelected = false;
            selectedTower.SelectOrDeselectTower(false);
        }
        selectedTower = _selectedTower;
        isPlacingTower = true;
    }


    private void Update()
    {
        //give player preview of selectedTower
        if (isPlacingTower)
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100, floor))
            {
                GridObjectData gridData = gridManager.GridObjectFromWorldPoint(hitInfo.point);
                if (gridData.type == 0)
                {
                    selectedTower.towerPreviewRenderer.color = new Color(0.7619722f, 0.8740168f, 0.9547169f);
                }
                else
                {
                    selectedTower.towerPreviewRenderer.color = new Color(0.8943396f, 0.2309691f, 0.09955848f);
                }
                selectedTower.transform.position = gridData.worldPos;
            }
        }
    }
}
