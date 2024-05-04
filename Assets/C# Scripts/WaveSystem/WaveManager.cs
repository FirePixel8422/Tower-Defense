using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;
    private void Awake()
    {
        Instance = this;
    }


    public Transform startPoint;
    public Transform endPoint;

    [HideInInspector]
    public SplineContainer splineContainer;


    public float preparationTime;

    public WaveDataSO[] waves;

    public List<EnemyCore> spawnedObj;



    private void Start()
    {
        splineContainer = GetComponent<SplineContainer>();
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(preparationTime);

        for (int i = 0; i < waves.Length; i++)
        {
            for (int i2 = 0; i2 < waves[i].waveParts.Length; i2++)
            {
                yield return new WaitForSeconds(waves[i].waveParts[i2].startDelay);

                for (int i3 = 0; i3 < waves[i].waveParts[i2].amount; i3++)
                {
                    EnemyCore enemyCore = Instantiate(waves[i].waveParts[i2].enemy.gameObject, startPoint.position, Quaternion.identity).GetComponent<EnemyCore>();

                    spawnedObj.Add(enemyCore);
                    enemyCore.Init(splineContainer, waves[i].waveParts[i2].immunityBarrier);
                    yield return new WaitForSeconds(waves[i].waveParts[i2].spawnDelay);
                }
            }

            yield return new WaitForSeconds(waves[i].waveEndDelay);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            spawnedObj[0].splineAnimator.ElapsedTime *= 0.5f;
            spawnedObj[0].splineAnimator.Duration *= 0.5f;
        }
        for (int i = 0; i < spawnedObj.Count; i++)
        {
            if (Vector3.Distance(spawnedObj[i].transform.position, endPoint.position) < 0.1f)
            {
                Destroy(spawnedObj[i].gameObject);
                spawnedObj.RemoveAt(i);
                i -= 1;
            }
        }
    }
}
