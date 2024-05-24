using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformanceManager : MonoBehaviour
{
    public float enemyMovementUpdateFPS;
    public float towerTargetUpdateFPS;
    public float towerRotationUpdateFPS;
    public float projectileTrackTargetUpdateFPS;


    private void Start()
    {
        WaveManager.Instance.enemyMovementUpdateInterval = 1 / enemyMovementUpdateFPS;
        TowerManager.Instance.towerTargetUpdateInterval = 1 / towerTargetUpdateFPS;
        TowerManager.Instance.towerRotationUpdateInterval = 1 / towerRotationUpdateFPS;
        TowerManager.Instance.projectileTrackTargetUpdateInterval = 1 / projectileTrackTargetUpdateFPS;
    }
    private void OnValidate()
    {
        if(WaveManager.Instance != null)
        {
            WaveManager.Instance.enemyMovementUpdateInterval = 1 / enemyMovementUpdateFPS;
        }
        if (TowerManager.Instance != null)
        {
            TowerManager.Instance.towerTargetUpdateInterval = 1 / towerTargetUpdateFPS;
            TowerManager.Instance.towerRotationUpdateInterval = 1 / towerRotationUpdateFPS;
            TowerManager.Instance.projectileTrackTargetUpdateInterval = 1 / projectileTrackTargetUpdateFPS;
        }
    }
}
