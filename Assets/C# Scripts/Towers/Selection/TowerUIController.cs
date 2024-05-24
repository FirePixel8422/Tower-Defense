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

    public Image towerImage;
    public TextMeshProUGUI towerName;
    public TextMeshProUGUI towerPopup;

    public Image LU_towerImage;
    public TextMeshProUGUI LU_towerCost;
    public TextMeshProUGUI LU_towerName;
    public TextMeshProUGUI LU_towerPopup;

    public Image RU_towerImage;
    public TextMeshProUGUI RU_towerCost;
    public TextMeshProUGUI RU_towerName;
    public TextMeshProUGUI RU_towerPopup;


    public void SelectTower(TowerCore tower)
    {
        if(tower.towerUIData == null)
        {
            return;
        }
        //select tower and open and configure UI equal to that towers SO info
        towerUI.SetActive(true);

        targetModeTextObj.text = tower.targetModeString;

        towerImage.sprite = tower.towerUIData.towerImage;
        towerName.text = tower.towerUIData.towerName;
        towerPopup.text = tower.towerUIData.towerPopup;

        if (tower.towerUIData.leftUpgrade != null)
        {
            //LU_towerImage.sprite = tower.towerUIData.leftUpgrade.towerImage;
            //LU_towerCost.text = tower.towerUIData.LU_essenseCost.ToString();
            LU_towerName.text = tower.towerUIData.leftUpgrade.towerName;
            LU_towerPopup.text = tower.towerUIData.leftUpgrade.towerPopup;
        }
        else
        {
            LU_towerImage.sprite = null;
            LU_towerCost.text = "";
            LU_towerName.text = "";
            LU_towerPopup.text = "";
        }
        if (tower.towerUIData.rightUpgrade != null)
        {
            //RU_towerImage.sprite = tower.towerUIData.rightUpgrade.towerImage;
            //RU_towerCost.text = tower.towerUIData.RU_essenseCost.ToString();
            RU_towerName.text = tower.towerUIData.rightUpgrade.towerName;
            RU_towerPopup.text = tower.towerUIData.rightUpgrade.towerPopup;
        }
        else
        {
            RU_towerImage.sprite = null;
            RU_towerCost.text = "";
            RU_towerName.text = "";
            RU_towerPopup.text = "";
        }

        tower.SelectOrDeselectTower(true);
    }
    public void DeSelectTower(TowerCore tower)
    {
        towerUI.SetActive(false);
        tower.SelectOrDeselectTower(false);
    }
}
