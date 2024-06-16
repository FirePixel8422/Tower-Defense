using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerUIController : MonoBehaviour
{
    public static TowerUIController Instance;
    private void Awake()
    {
        Instance = this;
    }


    public GameObject towerUI;
    public TextMeshProUGUI targetModeTextObj;

    public UIPop[] uiPopupControllers;

    public Image towerImage;
    public TextMeshProUGUI towerName;
    public TextMeshProUGUI towerPopup;

    //public Image[] towerUpgradeImages;
    public TextMeshProUGUI[] towerUpgradeNames;
    public TextMeshProUGUI[] towerUpgradePopups;
    public TextMeshProUGUI[] towerUpgradeCosts;

    public GameObject[] upgradeButtonObjs;
    


    public void SelectTower(TowerCore tower)
    {
        if (tower.towerUIData == null)
        {
            return;
        }
        TowerUIDataSO towerUiData = tower.towerUIData;
        foreach(UIPop uiPop in uiPopupControllers)
        {
            uiPop.OnPointerExit(null);
        }

        for (int i = 0; i < 3; i++)
        {
            bool state = tower.upgradePrefabs.Length > i && tower.upgradePrefabs[i] != null;
            upgradeButtonObjs[i].SetActive(state);
        }

        //select tower and open and configure UI equal to that towers SO info
        towerUI.SetActive(true);

        targetModeTextObj.text = tower.targetModeString;

        towerImage.sprite = towerUiData.towerImage;
        towerName.text = towerUiData.towerName;
        towerPopup.text = towerUiData.towerPopup;
        //towerCost.text = towerUiData.essenceCost.ToString();

        for (int i = 0; i < 3; i++)
        {
            if (i == towerUiData.upgrades.Length)
            {
                break;
            }

            towerUpgradeNames[i].text = towerUiData.upgrades[i].towerName;
            towerUpgradePopups[i].text = towerUiData.upgrades[i].towerPopup;
            towerUpgradeCosts[i].text = towerUiData.upgrades[i].buildCost.ToString();
        }
        

        tower.SelectOrDeselectTower(true);
    }

    public void DeSelectTower(TowerCore tower)
    {
        towerUI.SetActive(false);
        tower.SelectOrDeselectTower(false);
    }
}
