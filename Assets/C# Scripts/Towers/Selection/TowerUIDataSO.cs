using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DefTowerUIData", menuName = "TowerUI")]
public class TowerUIDataSO : ScriptableObject
{
    public float essenseCost;
    public MagicType essenseType;

    public Sprite towerImage;
    public string towerName;
    public string towerPopup;

    [Header("")]

    public TowerUIDataSO[] upgrades;
}
