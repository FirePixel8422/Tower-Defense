using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public int[] magicValues = new int[3];

    public float towerTargetUpdateInterval;
    public float towerRotationUpdateInterval;
    public float projectileTrackTargetUpdateInterval;


    private WaveManager waveManager;
    public List<TowerCore> spawnedTowerObj;

    private void Start()
    {
        waveManager = WaveManager.Instance;
        StartCoroutine(UpdateTowerTargetsLoop());
        StartCoroutine(UpdateTowerRotationsLoop());
    }

    public MagicType HighestMagicType()
    {
        //test which of three types has the most
        float highest = -1;
        List<int> targetTypeIndex = new List<int>();
        for (int i = 0; i < magicValues.Length; i++)
        {
            if (magicValues[i] >= highest)
            {
                if (magicValues[i] > highest)
                {
                    highest = magicValues[i];
                    
                    targetTypeIndex.Clear();
                    targetTypeIndex.Add(i);
                }
                else
                {
                    targetTypeIndex.Add(i);
                }
            }
        }
        //return early of all towers have 0 magicTypes active
        if (highest == 0)
        {
            return MagicType.Neutral;
        }

        //if more then 1 type have the most then randomize it
        int chosenId = -1;
        if (targetTypeIndex.Count > 1)
        {
            highest = -1;
            for (int i = 0; i < targetTypeIndex.Count; i++)
            {
                float r = Random.Range(0, 100f);
                if (r > highest)
                {
                    chosenId = targetTypeIndex[i];
                    highest = r;
                }
            }
        }
        else
        {
            chosenId = targetTypeIndex[0];
        }

        print(chosenId);
        if (chosenId == 0)
        {
            return MagicType.Life;
        }
        else if (chosenId == 1)
        {
            return MagicType.Arcane;
        }
        else if (chosenId == 2)
        {
            return MagicType.Ember;
        }
        return MagicType.Neutral;
    }


    private IEnumerator UpdateTowerTargetsLoop()
    {
        while (true)
        {
            UpdateTowerTargets();
            yield return new WaitForSeconds(towerTargetUpdateInterval);
        }
    }
    private IEnumerator UpdateTowerRotationsLoop()
    {
        while (true)
        {
            UpdateTowerRotations();
            yield return new WaitForSeconds(towerRotationUpdateInterval);
        }
    }



    #region Stashed Data

    private float bestProgression;
    private float leastProgression;
    private float mostDangerous;
    private float mostMaxHealth;
    private int targetEnemyIndex;

    private float towerRangeSquared;
    private Vector3 towerpos;
    private bool towerTargetModeIsFirst;
    private bool towerTargetModeIsLast;
    private bool towerTargetModeIsDangerous;
    private bool towerTargetModeIsTanky;

    private int enemyDataIndex;

    #endregion

    public EnemyData[] enemyData;
    public struct EnemyData
    {
        public float enemyProgression;
        public float enemyDamage;
        public float enemyMaxHealth;
        public Vector3 position;
        public bool IsNotAboutToDie;
    }
    

    private void UpdateTowerTargets()
    {
        //stash all enemyCore required data to struct and read that later.
        enemyData = new EnemyData[waveManager.spawnedObj.Count];

        enemyDataIndex = 0;
        foreach (EnemyCore enemy in waveManager.spawnedObj)
        {
            enemyData[enemyDataIndex].enemyProgression = enemy.progression;
            enemyData[enemyDataIndex].enemyDamage = enemy.damage;
            enemyData[enemyDataIndex].enemyMaxHealth = enemy.maxHealth;
            enemyData[enemyDataIndex].position = enemy.transform.position;
            enemyData[enemyDataIndex].IsNotAboutToDie = enemy.IsNotAboutToDie;
            enemyDataIndex += 1;
        }


        for (int i = spawnedTowerObj.Count - 1; i >= 0; i--)
        {
            if (spawnedTowerObj[i].towerCompleted == false)
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


            targetEnemyIndex = -1;
            enemyDataIndex = 0;
            foreach (EnemyData enemyData in enemyData)
            {
                if ((towerpos - enemyData.position).sqrMagnitude < towerRangeSquared)
                {
                    //check if enemy isnt going to die in next few frames from an already active dmg source > this way this tower wont shoot air.
                    if (enemyData.IsNotAboutToDie)
                    {
                        //furthest progressed enemy
                        if (towerTargetModeIsFirst && enemyData.enemyProgression > bestProgression)
                        {
                            bestProgression = enemyData.enemyProgression;
                            targetEnemyIndex = enemyDataIndex;
                        }
                        //least far progressed enemy
                        else if (towerTargetModeIsLast && enemyData.enemyProgression < leastProgression)
                        {
                            leastProgression = enemyData.enemyProgression;
                            targetEnemyIndex = enemyDataIndex;
                        }
                        //most dmg dealing furthest progressed enemy
                        else if (towerTargetModeIsDangerous && enemyData.enemyDamage >= mostDangerous)
                        {
                            if (enemyData.enemyDamage > mostDangerous)
                            {
                                mostDangerous = enemyData.enemyDamage;
                                targetEnemyIndex = enemyDataIndex;
                            }
                            if (enemyData.enemyProgression > bestProgression)
                            {
                                mostDangerous = enemyData.enemyDamage;
                                bestProgression = enemyData.enemyProgression;
                                targetEnemyIndex = enemyDataIndex;
                            }
                        }
                        //tankiest furthest prgressed enemy
                        else if (towerTargetModeIsTanky && enemyData.enemyMaxHealth >= mostMaxHealth)
                        {
                            if (enemyData.enemyMaxHealth > mostMaxHealth)
                            {
                                mostMaxHealth = enemyData.enemyMaxHealth;
                                targetEnemyIndex = enemyDataIndex;
                            }
                            if (enemyData.enemyProgression > bestProgression)
                            {
                                mostMaxHealth = enemyData.enemyMaxHealth;
                                bestProgression = enemyData.enemyProgression;
                                targetEnemyIndex = enemyDataIndex;
                            }
                        }
                    }
                }
                enemyDataIndex += 1;
            }

            //if targetEnemyIndex is "-1" (no target found with specified rules like inrange etc), return.
            if (targetEnemyIndex == -1)
            {
                spawnedTowerObj[i].target = null;
                continue;
            }
            else
            {
                spawnedTowerObj[i].target = waveManager.spawnedObj[targetEnemyIndex];
            }
        }
    }

    private void UpdateTowerRotations()
    {
        foreach (TowerCore tower in spawnedTowerObj)
        {
            if (tower.rotSpeed == 0 || tower.target == null || tower.target.IsNotAboutToDie == false)
            {
                continue;
            }
            
            //update tower rotation
            Vector3 dir = tower.rotPoint.position - tower.target.transform.position;
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

            //apply rotation
            tower.rotPoint.rotation =
                Quaternion.RotateTowards(
                    tower.rotPoint.rotation,
                    Quaternion.Euler(0, angle, 0) * tower.rotOffset,
                    tower.rotSpeed * towerRotationUpdateInterval);
        }
    }
}
