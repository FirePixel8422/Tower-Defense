using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;
    private void Awake()
    {
        Instance = this;
    }


    public Transform[] points;


    public float preparationTime;

    public WaveDataSO[] waves;

    public List<EnemyCore> spawnedObj;



    private void Start()
    {
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
                    EnemyCore enemyCore = Instantiate(waves[i].waveParts[i2].enemy.gameObject, points[0].position, Quaternion.identity).GetComponent<EnemyCore>();

                    spawnedObj.Add(enemyCore);
                    enemyCore.Init(waves[i].waveParts[i2].immunityBarrier);
                    yield return new WaitForSeconds(waves[i].waveParts[i2].spawnDelay);
                }
            }

            yield return new WaitForSeconds(waves[i].waveEndDelay);
        }
    }

    private void Update()
    {
        //loop through all enemies and move them forward at all times.
        //then rotate them towards the next points, making them turn.
        for (int i = 0; i < spawnedObj.Count; i++)
        {
            //make a wa to get rotation between current point and next one, and calculate rotation speed with it
            spawnedObj[i].transform.rotation = Quaternion.RotateTowards(spawnedObj[i].transform.rotation,
                points[spawnedObj[i].pointIndex].rotation, spawnedObj[i].rotSpeed * Time.deltaTime);
            spawnedObj[i].transform.position -= spawnedObj[i].moveSpeed * Time.deltaTime * spawnedObj[i].transform.forward;

            spawnedObj[i].progression += Time.deltaTime * spawnedObj[i].moveSpeed;

            if (Vector3.Distance(spawnedObj[i].transform.position, points[spawnedObj[i].pointIndex].position) < 0.1f)
            {
                 spawnedObj[i].pointIndex += 1;
            }
        }

        for (int i = 0; i < spawnedObj.Count; i++)
        {
            if (Vector3.Distance(spawnedObj[i].transform.position, points[points.Length - 1].position) < 0.1f)
            {
                Destroy(spawnedObj[i].gameObject);
                spawnedObj.RemoveAt(i);
                i -= 1;
            }
        }
    }
}
