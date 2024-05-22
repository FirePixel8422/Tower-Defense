using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public float towerUpdateInterval;


    private WaveManager waveManager;
    public List<TowerCore> spawnedTowerObj;

    private void Start()
    {
        waveManager = WaveManager.Instance;
        StartCoroutine(UpdateTowersLoop());
    }

    public MagicType HighestMagicType()
    {
        int[] magicValues = new int[4];
        foreach (TowerCore tower in spawnedTowerObj)
        {
            /*if (tower.magicType.Contains(MagicType.Neutral))
            {
                magicValues[0] += 1;
            }
            else */if (tower.magicType.Contains(MagicType.Arcane))
            {
                magicValues[1] += 1;
            }
            else if (tower.magicType.Contains(MagicType.Ember))
            {
                magicValues[2] += 1;
            }
            else if (tower.magicType.Contains(MagicType.Life))
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


    private IEnumerator UpdateTowersLoop()
    {
        while (true)
        {
            UpdateTowers();
            yield return new WaitForSeconds(towerUpdateInterval);
        }
    }



    #region Stashed Data
    private int spawnedEnemyObjCount;

    private float bestProgression;
    private float leastProgression;
    private float mostDangerous;
    private float mostMaxHealth;
    private int id;

    private float towerRangeSquared;
    private Vector3 towerpos;
    private bool towerTargetModeIsFirst;
    private bool towerTargetModeIsLast;
    private bool towerTargetModeIsDangerous;
    private bool towerTargetModeIsTanky;

    private float enemyProgression;
    private float enemyDamage;
    private float enemyMaxHealth;
    #endregion
    private void UpdateTowers()
    {
        spawnedEnemyObjCount = waveManager.spawnedObj.Count;
        for (int i = 0; i < spawnedTowerObj.Count; i++)
        {
            if (spawnedTowerObj[i].towerCompleted == false || spawnedTowerObj[i].attackSpeed == 0)
            {
                continue;
            }

            //all values stashed so acceses to references (enemyCore and TowerCore) is limited.
            towerRangeSquared = spawnedTowerObj[i].range * spawnedTowerObj[i].range;
            towerpos = spawnedTowerObj[i].transform.position;

            towerTargetModeIsFirst = spawnedTowerObj[i].targetMode == TargetMode.First;
            towerTargetModeIsLast = spawnedTowerObj[i].targetMode == TargetMode.Last;
            towerTargetModeIsDangerous = spawnedTowerObj[i].targetMode == TargetMode.Dangerous;
            towerTargetModeIsTanky = spawnedTowerObj[i].targetMode == TargetMode.Tanky;

            //learn to use the "Switch" later


            bestProgression = -1;
            leastProgression = float.MaxValue;
            mostDangerous = -1;
            mostMaxHealth = -1;


            id = -1;
            for (int i2 = 0; i2 < spawnedEnemyObjCount; i2++)
            {
                //stash even more data in private floats
                enemyProgression = waveManager.spawnedObj[i2].progression;
                enemyDamage = waveManager.spawnedObj[i2].damage;
                enemyMaxHealth = waveManager.spawnedObj[i2].maxHealth;

                if ((towerpos - waveManager.spawnedObj[i2].transform.position).sqrMagnitude < towerRangeSquared)
                {
                    //check if enemy isnt going to die in next few frames from an already active dmg source > this way this tower wont shoot air.
                    if (waveManager.spawnedObj[i2].IsNotAboutToDie)
                    {
                        //furthest progressed enemy
                        if (towerTargetModeIsFirst && enemyProgression > bestProgression)
                        {
                            bestProgression = enemyProgression;
                            id = i2;
                        }
                        //least far progressed enemy
                        else if (towerTargetModeIsLast && enemyProgression < leastProgression)
                        {
                            leastProgression = enemyProgression;
                            id = i2;
                        }
                        //most dmg dealing furthest progressed enemy
                        else if (towerTargetModeIsDangerous && enemyDamage >= mostDangerous)
                        {
                            if (enemyDamage > mostDangerous)
                            {   
                                mostDangerous = enemyDamage;
                                id = i2;
                            }
                            if (enemyProgression > bestProgression)
                            {
                                mostDangerous = enemyDamage;
                                bestProgression = enemyProgression;
                                id = i2;
                            }
                        }
                        //tankiest furthest prgressed enemy
                        else if (towerTargetModeIsTanky && enemyMaxHealth >= mostMaxHealth)
                        {
                            if (enemyMaxHealth > mostMaxHealth)
                            {
                                mostMaxHealth = enemyMaxHealth;
                                id = i2;
                            }
                            if (enemyProgression > bestProgression)
                            {
                                mostMaxHealth = enemyMaxHealth;
                                bestProgression = enemyProgression;
                                id = i2;
                            }
                        }
                    }
                }
            }

            //if id is "-1" (no target found with specified rules like inrange etc), return.
            if (id == -1)
            {
                spawnedTowerObj[i].target = null;
                continue;
            }
            else
            {
                spawnedTowerObj[i].target = waveManager.spawnedObj[id];

                if (spawnedTowerObj[i].rotSpeed != 0)
                {
                    //update tower rotation
                    Vector3 dir = spawnedTowerObj[i].rotPoint.position - waveManager.spawnedObj[id].transform.position;
                    float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

                    //apply rotation
                    spawnedTowerObj[i].rotPoint.rotation =
                        Quaternion.RotateTowards(
                            spawnedTowerObj[i].rotPoint.rotation,
                            Quaternion.Euler(0, angle, 0) * spawnedTowerObj[i].rotOffset,
                            spawnedTowerObj[i].rotSpeed * Time.deltaTime);
                }
            }
        }
    }
}
