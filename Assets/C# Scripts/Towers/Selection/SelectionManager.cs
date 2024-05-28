using System.Collections.Generic;
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

    private GridManager gridManager;
    private TowerManager towerManager;
    private Camera mainCam;

    public LayerMask floor;
    public LayerMask path;

    public GameObject preSpawnedTowerHolder;
    public GameObject towerUIHolder;

    public TowerCore selectedTower;
    public TowerPreview selectedPreviewTower;

    public bool isPlacingTower;
    public bool towerSelected;


    private void Start()
    {
        gridManager = GridManager.Instance;
        towerManager = TowerManager.Instance;
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
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100, floor + path))
        {
            GridObjectData gridData = gridManager.GridObjectFromWorldPoint(hitInfo.point);

            if ((gridData.type == 0 && selectedPreviewTower.placeOntrack == false) || (gridData.type == 1 && selectedPreviewTower.placeOntrack))
            {
                selectedPreviewTower.towerPreviewRenderer.color = new Color(0.7619722f, 0.8740168f, 0.9547169f);
                selectedPreviewTower.UpdateTowerPreviewColor(Color.white);

                selectedPreviewTower.transform.localPosition = Vector3.zero;
                selectedTower = Instantiate(selectedPreviewTower.towerPrefab, gridData.worldPos, selectedPreviewTower.transform.rotation).GetComponent<TowerCore>();

                selectedTower.CoreInit();

                if (selectedTower.excludeTargetUpdates == false) 
                {
                    towerManager.spawnedTowerObj.Add(selectedTower);
                }

                gridManager.UpdateGridDataFieldType(gridData.gridPos, 2, selectedTower);
                isPlacingTower = false;
            }
        }
    }
    public void TrySelectTower()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100, floor + path))
        {
            GridObjectData gridData = gridManager.GridObjectFromWorldPoint(hitInfo.point);
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

    public void UpdateSelectedTowerTargetMode(bool prev)
    {
        string targetModeString;
        if (prev)
        {
            targetModeString = selectedTower.PreviousTargetMode();
        }
        else
        {
            targetModeString = selectedTower.NextTargetMode();
        }
        TowerUIController.Instance.targetModeTextObj.text = targetModeString;
    }

    public void TryUpgradeTower(int path)
    {
        MagicType chosenType = MagicType.Ember;
        for (int i = 0; i < 3; i++)
        {
            if (path != i)
            {
                continue;
            }
            float cost = selectedTower.towerUIData.upgrades[i].essenseCost;
            if (EssenceManager.Instance.UpgradePossibleWithType(out bool[] options, cost, selectedTower.towerUIData.upgrades[i].essenseType))
            {
                if (selectedTower.towerUIData.upgrades[i].essenseType == MagicType.Neutral)
                {
                    if (options[0] == true && chosenType == MagicType.Life)
                    {
                        EssenceManager.Instance.AddRemoveEssence(-cost, chosenType);
                    }
                    else if (options[1] == true && chosenType == MagicType.Arcane)
                    {
                        EssenceManager.Instance.AddRemoveEssence(-cost, chosenType);
                    }
                    else if (options[2] == true && chosenType == MagicType.Ember)
                    {
                        EssenceManager.Instance.AddRemoveEssence(-cost, chosenType);
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    EssenceManager.Instance.AddRemoveEssence(-cost, selectedTower.towerUIData.upgrades[i].essenseType);
                }

                towerManager.spawnedTowerObj.Remove(selectedTower);
                selectedTower.UpgradeTower(path);

                TowerUIController.Instance.DeSelectTower(selectedTower);
                towerSelected = false;
                selectedTower = null;
            }
        }
    }

    public void SellTower()
    {
        EssenceManager.Instance.AddRemoveEssence(selectedTower.towerUIData.essenseCost, selectedTower.towerUIData.essenseType);

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
        if (isPlacingTower)
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100, floor + path))
            {
                GridObjectData gridData = gridManager.GridObjectFromWorldPoint(hitInfo.point);

                int onTrack = gridData.type;
                if ((onTrack == 1 && selectedPreviewTower.placeOntrack) || (onTrack == 0 && selectedPreviewTower.placeOntrack == false))
                {
                    selectedPreviewTower.towerPreviewRenderer.color = new Color(0.7619722f, 0.8740168f, 0.9547169f);
                    selectedPreviewTower.UpdateTowerPreviewColor(Color.white);
                }
                else
                {
                    selectedPreviewTower.towerPreviewRenderer.color = new Color(0.8943396f, 0.2309691f, 0.09955848f);
                    selectedPreviewTower.UpdateTowerPreviewColor(Color.red);
                }
                selectedPreviewTower.transform.position = gridData.worldPos;
            }
        }
    }
}
