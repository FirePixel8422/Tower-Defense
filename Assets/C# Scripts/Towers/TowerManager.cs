using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance;
    private void Awake()
    {
        Instance = this;
    }


    private WaveManager waveManager;
    public List<TowerCore> spawnedTowerObj;

    private void Start()
    {
        waveManager = WaveManager.Instance;
    }

    public MagicType HighestMagicType()
    {
        int[] magicValues = new int[4];
        foreach (TowerCore tower in spawnedTowerObj)
        {
            if (tower.magicType == MagicType.Neutral)
            {
                magicValues[0] += 1;
            }
            else if (tower.magicType == MagicType.Arcane)
            {
                magicValues[1] += 1;
            }
            else if (tower.magicType == MagicType.Ember)
            {
                magicValues[2] += 1;
            }
            else if (tower.magicType == MagicType.Life)
            {
                magicValues[3] += 1;
            }
        }

        int highest = -1;
        int id = -1;
        for (int i = 0; i < magicValues.Length; i++)
        {
            if (magicValues[i] > highest)
            {
                highest = magicValues[i];
                id = i;
            }
        }

        if (id == 1)
        {
            return MagicType.Arcane;
        }
        else if (id == 2)
        {
            return MagicType.Ember;
        }
        else if (id == 3)
        {
            return MagicType.Life;
        }
        //id == 0
        return MagicType.Neutral;
    }


    private void Update()
    {
        for (int i = 0; i < spawnedTowerObj.Count; i++)
        {
            float progression = -1;
            int id = -1;
            for (int i2 = 0; i2 < waveManager.spawnedObj.Count; i2++)
            {
                if (Vector3.Distance(spawnedTowerObj[i].transform.position, waveManager.spawnedObj[i2].transform.position) < spawnedTowerObj[i].range)
                {
                    if (waveManager.spawnedObj[i2].splineAnimator.ElapsedTime > progression)
                    {
                        progression = waveManager.spawnedObj[i2].splineAnimator.ElapsedTime;
                        id = i2;
                    }
                }
            }
            spawnedTowerObj[i].transform.LookAt(waveManager.spawnedObj[id].transform.position, Vector3.up);
        }
    }
}
