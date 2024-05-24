using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DefTowerUIData", menuName = "TowerUI")]
public class TowerUIDataSO : ScriptableObject
{
    public Sprite towerImage;
    public string towerName;
    public string towerPopup;

    [Header("")]

    public TowerUIDataSO leftUpgrade;
    public int LU_essenseCost;
    public MagicType LU_essenseType;
    [Header("1 for first upgrade, 2 for second")]
    public int LU_upgradeNumber;

    [Header("")]
    
    public TowerUIDataSO rightUpgrade;
    public int RU_essenseCost;
    public MagicType RU_essenseType;
    [Header("1 for first upgrade, 2 for second")]
    public int RU_upgradeNumber;
}
