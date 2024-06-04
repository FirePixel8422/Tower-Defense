using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance;
    private void Awake()
    {
        Instance = this;
        gfxRayCaster = FindObjectOfType<GraphicRaycaster>();
    }


    public GraphicRaycaster gfxRayCaster;

    private Camera mainCam;

    public LayerMask floor;
    public LayerMask path;
    public LayerMask water;

    public GameObject preSpawnedTowerHolder;
    public GameObject towerUIHolder;

    public TowerCore selectedTower;
    public TowerPreview selectedPreviewTower;

    public bool isPlacingTower;
    public bool towerSelected;

    private GridObjectData selectedGridTileData;
    private Vector3 mousePos;


    private void Start()
    {
        mainCam = Camera.main;

        Button[] buttons = towerUIHolder.GetComponentsInChildren<Button>();
        TowerPreview[] towers = preSpawnedTowerHolder.GetComponentsInChildren<TowerPreview>();
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
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;

            var results = new List<RaycastResult>();
            gfxRayCaster.Raycast(pointerEventData, results);

            if (results.Count > 0)
            {
                return;
            }

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
    public void OnCancel(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            towerSelected = false;
            selectedTower = null;
            TowerUIController.Instance.DeSelectTower(selectedTower);
        }
    }

    public void TryPlaceTower()
    {
        //place tower system
        if (selectedGridTileData.type == selectedPreviewTower.placementIndex)
        {
            if (ResourceManager.Instance.TryBuildTower(selectedPreviewTower.scrapCost))
            {
                essenceChooseMenuObj.SetActive(false);
                PlaceTower();
            }
            else
            {
                //say that there is not enough essence
                CancelTowerPlacement();
            }
        }
    }
    public void CancelTowerPlacement()
    {
        essenceChooseMenuObj.SetActive(false);

        selectedPreviewTower.transform.localPosition = Vector3.zero;
        selectedPreviewTower.locked = false;
        isPlacingTower = false;
    }
    private void PlaceTower()
    {
        selectedPreviewTower.towerPreviewRenderer.color = new Color(0.7619722f, 0.8740168f, 0.9547169f);
        selectedPreviewTower.UpdateTowerPreviewColor(Color.white);

        selectedPreviewTower.transform.localPosition = Vector3.zero;
        selectedTower = Instantiate(selectedPreviewTower.towerPrefab, selectedGridTileData.worldPos, selectedPreviewTower.transform.rotation).GetComponent<TowerCore>();

        selectedTower.CoreInit();

        if (selectedTower.excludeTargetUpdates == false)
        {
            TowerManager.Instance.spawnedTowerObj.Add(selectedTower);
        }

        GridManager.Instance.UpdateGridDataFieldType(selectedGridTileData.gridPos, 3, selectedTower);
        GridManager.Instance.UpdateGridDataFieldType(selectedGridTileData.gridPos, selectedTower.towerUIData.buildCost, selectedTower.towerUIData.essenceType);
        isPlacingTower = false;
    }

    public void TrySelectTower()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100, floor + path))
        {
            GridObjectData gridData = GridManager.Instance.GridObjectFromWorldPoint(hitInfo.point);
            if (gridData.tower != null && gridData.tower.towerCompleted)
            {
                //deselect older selected tower
                if (towerSelected)
                {
                    selectedTower.SelectOrDeselectTower(false);
                }

                //select tower
                selectedTower = gridData.tower;

                towerSelected = true;

                TowerUIController.Instance.SelectTower(selectedTower);

                //refresh targetMode text
                UpdateSelectedTowerTargetMode(0);
                return;
            }
        }

        //deselect tower if clicked on empty space
        if (towerSelected)
        {
            TowerUIController.Instance.DeSelectTower(selectedTower);
        }
        towerSelected = false;
    }

    public void UpdateSelectedTowerTargetMode(int direction)
    {
        TowerUIController.Instance.targetModeTextObj.text = selectedTower.UpdateTargetMode(direction);
    }



    public int pathId;
    public GameObject essenceChooseMenuObj;
    public GameObject[] essenceTypes;
    public void SetPathId(int id)
    {
        pathId = id;
        if (selectedTower.towerUIData.upgrades[id].essenceType == MagicType.Neutral)
        {
            essenceChooseMenuObj.SetActive(true);
        }
        else
        {
            TryUpgradeTower(0);
        }
    }
    public void TryUpgradeTower(int chosenType)
    {
        for (int i = 0; i < 3; i++)
        {
            if (pathId != i || selectedTower == null || selectedTower.upgradePrefabs.Length <= pathId)
            {
                continue;
            }

            if (ResourceManager.Instance.TryUpgradeTower(selectedTower.towerUIData.upgrades[i].buildCost, selectedTower.towerUIData.upgrades[i].essenceType, (MagicType)chosenType))
            {
                GridObjectData gridData = GridManager.Instance.GridObjectFromWorldPoint(selectedTower.transform.position);
                GridManager.Instance.UpdateGridDataFieldType(gridData.gridPos, selectedTower.towerUIData.upgrades[pathId].buildCost, selectedTower.towerUIData.upgrades[pathId].essenceType);

                TowerManager.Instance.spawnedTowerObj.Remove(selectedTower);
                selectedTower.UpgradeTower(pathId);

                TowerUIController.Instance.DeSelectTower(selectedTower);
                towerSelected = false;
                selectedTower = null;
            }
        }
    }

    public void SellTower()
    {
        ResourceManager.Instance.AddRemoveEssence(selectedTower.towerUIData.buildCost, selectedTower.towerUIData.essenceType);

        GridObjectData gridData = GridManager.Instance.GridObjectFromWorldPoint(selectedTower.transform.position);
        GridManager.Instance.UpdateGridDataFieldType(gridData.gridPos, gridData.coreType, null);

        TowerManager.Instance.spawnedTowerObj.Remove(selectedTower);
        Destroy(selectedTower.gameObject);
        towerSelected = false;
        selectedTower = null;
    }


    public void StartTowerPreview(TowerPreview _selectedTower)
    {
        if (isPlacingTower)
        {
            selectedPreviewTower.transform.localPosition = Vector3.zero;
            selectedPreviewTower.locked = false;
        }
        if (towerSelected)
        {
            towerSelected = false;
            TowerUIController.Instance.DeSelectTower(selectedTower);
        }
        selectedTower = null;
        selectedPreviewTower = _selectedTower;
        isPlacingTower = true;
    }


    private void Update()
    {
        //give player preview of selectedTower
        if (isPlacingTower && Input.mousePosition != mousePos && selectedPreviewTower.locked == false)
        {
            mousePos = Input.mousePosition;
            Ray ray = mainCam.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100, floor + path))
            {
                selectedGridTileData = GridManager.Instance.GridObjectFromWorldPoint(hitInfo.point);

                int onTrack = selectedGridTileData.type;
                if (onTrack == selectedPreviewTower.placementIndex)
                {
                    selectedPreviewTower.towerPreviewRenderer.color = new Color(0.7619722f, 0.8740168f, 0.9547169f);
                    selectedPreviewTower.UpdateTowerPreviewColor(Color.white);
                }
                else
                {
                    selectedPreviewTower.towerPreviewRenderer.color = new Color(0.8943396f, 0.2309691f, 0.09955848f);
                    selectedPreviewTower.UpdateTowerPreviewColor(Color.red);
                }
                selectedPreviewTower.transform.position = selectedGridTileData.worldPos;
            }
        }
    }
}
