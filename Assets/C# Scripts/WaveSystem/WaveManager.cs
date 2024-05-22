using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using System.Linq;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;
    private void Awake()
    {
        Instance = this;
    }


    public List<Transform> points;
    public float smoothValue;
    public float rotMultiplier;

    public float preparationTime;
    
    public WaveDataSO[] waves;

    public List<EnemyCore> spawnedObj;


    private void Start()
    {
        points = GetComponentsInChildren<Transform>().ToList();
        points.Remove(transform);

        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(preparationTime);

        while (true)
        {
            for (int i = 0; i < waves.Length; i++)
            {
                for (int i2 = 0; i2 < waves[i].waveParts.Length; i2++)
                {
                    yield return new WaitForSeconds(waves[i].waveParts[i2].startDelay);

                    for (int i3 = 0; i3 < waves[i].waveParts[i2].amount; i3++)
                    {
                        SpawnEnemy(waves[i].waveParts[i2].enemy.enemyId);
                        yield return new WaitForSeconds(waves[i].waveParts[i2].spawnDelay);
                    }
                }
                yield return new WaitForSeconds(waves[i].waveEndDelay);
            }
        }
    }
    private void SpawnEnemy(int id)
    {
        EnemyCore target = EnemyPooling.Instance.GetPulledObj(id, points[0].position, Quaternion.identity).GetComponent<EnemyCore>();

        spawnedObj.Add(target);

        UpdateTargetDir(target);

        target.Init();
    }




    private Vector3 pos;
    private int pointIndex;
    private int rotPointDiff;
    private float targetSpeed;
    private Vector3 movement;
    private float dist;


    private void Update()
    {
        //loop through all enemies and move them forward at all times.
        //then rotate them towards the next points, making them turn.
        for (int i = spawnedObj.Count - 1; i >= 0; i--)
        {
            //get rotation between current point and next one, and calculate rotation speed.
            EnemyCore target = spawnedObj[i];
            int pointIndex = target.pointIndex;

            int rotPointDiff = Mathf.RoundToInt(Quaternion.Angle(points[Mathf.Max(0, pointIndex - 1)].rotation, points[pointIndex].rotation) / 90);
            float targetSpeed = (target.moveSpeed * 90 + Mathf.Max(0, rotPointDiff - 1) * 45) * rotMultiplier * Time.deltaTime;

            //calculate movement and check distance to point.
            Vector3 movement = target.moveSpeed * Time.deltaTime * target.transform.forward;
            float dist = movement.x + movement.y + movement.z;
            if (Vector3.Distance(target.transform.position, points[pointIndex].position) < dist)
            {
                target.transform.position = points[pointIndex].position;
            }
            else
            {
                target.transform.position -= movement;
            }

            //rotate towards the next point.
            target.transform.rotation = Quaternion.RotateTowards(target.transform.rotation, points[pointIndex].rotation, targetSpeed);

            //retrieve int2 direction and lock its position in those 2 angles (x-1,x1,z-1,z1)
            Vector3 pos = target.transform.position;
            int2 dir = target.dir;
            if (dir.x == 1)
            {
                if (target.transform.position.x < points[pointIndex].position.x)
                {
                    pos.x = points[pointIndex].position.x;
                }
            }
            if (dir.x == -1)
            {
                if (target.transform.position.x > points[pointIndex].position.x)
                {
                    pos.x = points[pointIndex].position.x;
                }
            }
            if (dir.y == 1)
            {
                if(target.transform.position.z < points[pointIndex].position.z)
                {
                    pos.z = points[pointIndex].position.z;
                }
            }
            if (dir.y == -1)
            {
                if (target.transform.position.z > points[pointIndex].position.z)
                {
                    pos.z = points[pointIndex].position.z;
                }
            }
            target.transform.position = pos;

            target.progression += Time.deltaTime * target.moveSpeed;


            //update targetPoint to target and give new dir with UpdateTargetDir
            if (Vector3.Distance(target.transform.position, points[pointIndex].position) < 0.035f)
            {
                target.pointIndex += 1;
                
                if (target.pointIndex == points.Count)
                {
                    spawnedObj[i].gameObject.SetActive(false);
                    spawnedObj.RemoveAt(i);
                }
                else
                {
                    UpdateTargetDir(target);
                }
            }
        }
    }

    public void UpdateTargetDir(EnemyCore target)
    {
        int2 dir = int2.zero;
        if (target.transform.position.x > points[target.pointIndex].position.x)
        {
            dir.x = 1;
        }
        if (target.transform.position.x < points[target.pointIndex].position.x)
        {
            dir.x = -1;
        }
        if (target.transform.position.z > points[target.pointIndex].position.z)
        {
            dir.y = 1;
        }
        if (target.transform.position.z < points[target.pointIndex].position.z)
        {
            dir.y = -1;
        }
        target.dir = dir;
    }
}
