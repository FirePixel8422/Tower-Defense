using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DefTowerUIData", menuName = "TowerUI")]
public class TowerUIDataSO : ScriptableObject
{
    public float essenceCost;
    public MagicType essenceType;

    public Sprite towerImage;
    public string towerName;
    public string towerPopup;

    [Header("")]

    public TowerUIDataSO[] upgrades;
}
