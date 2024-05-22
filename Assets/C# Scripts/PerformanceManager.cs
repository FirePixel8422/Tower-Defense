using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformanceManager : MonoBehaviour
{
    public int enemyMovementFPS;
    public int towerUpdateFPS;


    private void Start()
    {
        if (TowerManager.Instance != null)
        {
            TowerManager.Instance.towerUpdateInterval = 1 / towerUpdateFPS;
        }
    }
    private void OnValidate()
    {
        if(TowerManager.Instance != null)
        {
            TowerManager.Instance.towerUpdateInterval = 1 / towerUpdateFPS;
        }
    }
}
