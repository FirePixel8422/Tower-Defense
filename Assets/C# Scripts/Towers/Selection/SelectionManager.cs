using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
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


    public static GraphicRaycaster gfxRayCaster;

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

    public bool canAffordTower;


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
            if (towerSelected)
            {
                towerSelected = false;
                TowerUIController.Instance.DeSelectTower(selectedTower); 
                selectedTower = null;
            }

            if (isPlacingTower)
            {
                selectedPreviewTower.transform.localPosition = Vector3.zero;
                selectedPreviewTower.locked = false;
                isPlacingTower = false;
            }
        }
    }

    public void TryPlaceTower()
    {
        //place tower system
        if (selectedGridTileData.type == selectedPreviewTower.placementIndex)
        {
            if (ResourceManager.Instance.TryBuildTower(selectedPreviewTower.scrapCost))
            {
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

        AudioController towerAudio = selectedTower.GetComponent<AudioController>();
        towerAudio.UpdateVolume(AudioManager.Instance.menuData.audioMaster, AudioManager.Instance.menuData.audioSFX, 0);
        AudioManager.Instance.audioControllers.Add(towerAudio);

        selectedTower.CoreInit();

        if (selectedTower.excludeTargetUpdates == false)
        {
            TowerManager.Instance.spawnedTowerObj.Add(selectedTower);
        }

        GridManager.Instance.UpdateGridDataFieldType(selectedGridTileData.gridPos, 3, selectedTower);
        GridManager.Instance.UpdateGridDataFieldType(selectedGridTileData.gridPos, selectedTower.towerUIData.buildCost);
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



    public GameObject[] essenceTypes;
    public void TryUpgradeTower(int chosenType)
    {
        if (selectedTower == null || selectedTower.upgradePrefabs.Length <= chosenType && selectedTower.upgradePrefabs[chosenType] != null)
        {
            return;
        }

        if (ResourceManager.Instance.TryUpgradeTower(selectedTower.towerUIData.upgrades[chosenType].buildCost, selectedTower.towerUIData.upgrades[chosenType].essenceType, (MagicType)chosenType))
        {
            GridObjectData gridData = GridManager.Instance.GridObjectFromWorldPoint(selectedTower.transform.position);

            TowerManager.Instance.spawnedTowerObj.Remove(selectedTower);
            selectedTower.UpgradeTower(chosenType, gridData.gridPos);

            TowerUIController.Instance.DeSelectTower(selectedTower);
            towerSelected = false;
            selectedTower = null;
        }
    }

    public void SellTower()
    {
        GridObjectData gridData = GridManager.Instance.GridObjectFromWorldPoint(selectedTower.transform.position);

        ResourceManager.Instance.AddScrap(gridData.scrap);
        GridManager.Instance.ResetGridDataFieldType(gridData.gridPos);

        TowerManager.Instance.spawnedTowerObj.Remove(selectedTower);

        TowerUIController.Instance.DeSelectTower(selectedTower);

        AudioManager.Instance.audioControllers.Remove(selectedTower.audioController);
        Destroy(selectedTower.gameObject);

        towerSelected = false;
        selectedTower = null;

        towerSelected = false;
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

        canAffordTower = ResourceManager.Instance.Scrap >= selectedPreviewTower.scrapCost;
    }


    private void Update()
    {
        //give player preview of selectedTower
        if (Input.mousePosition != mousePos)
        {
            UpdateTowerPlacementPreview();
        }
    }

    public void UpdateTowerPlacementPreview()
    {
        mousePos = Input.mousePosition;
        if (isPlacingTower == false || selectedPreviewTower.locked == true)
        {
            return;
        }

        Ray ray = mainCam.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100, floor + path))
        {
            selectedGridTileData = GridManager.Instance.GridObjectFromWorldPoint(hitInfo.point);

            int onTrack = selectedGridTileData.type;
            if (onTrack == selectedPreviewTower.placementIndex && canAffordTower)
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
