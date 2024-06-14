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

    public int waveScaleHealthDelay;
    public float healthScaleMultiplier;
    public float cHealthScale = 1;

    public float enemyMovementUpdateInterval;

    public List<Transform> points;
    private Vector3 startPointPos;
    public float smoothValue;
    public float rotMultiplier;

    public float preparationTime;
    
    public WaveDataSO[] waves;

    public List<EnemyCore> spawnedObj;


    private void Start()
    {
        points = GetComponentsInChildren<Transform>().ToList();
        points.Remove(transform);
        startPointPos = points[0].position;

        StartCoroutine(SpawnLoop());
        StartCoroutine(UpdateEnemyMovementsLoop());
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
                        target = EnemyPooling.Instance.GetPulledObj(waves[i].waveParts[i2].enemy.enemyId, startPointPos, Quaternion.identity).GetComponent<EnemyCore>();
                        target.Init(cHealthScale);
                        spawnedObj.Add(target);
                        UpdateTargetDir(out target.dir, target.transform.position, startPointPos);

                        yield return new WaitForSeconds(waves[i].waveParts[i2].spawnDelay);
                    }
                }
                yield return new WaitForSeconds(waves[i].waveEndDelay);
                cHealthScale += healthScaleMultiplier - 1;
                ResourceManager.Instance.AddScrap(waves[i].scrapForThisWave);
            }
        }
    }


    private EnemyCore target;
    private IEnumerator UpdateEnemyMovementsLoop()
    {
        float elapsedTime = 0;
        while (true)
        {
            yield return null;
            elapsedTime += Time.deltaTime;

            if (elapsedTime > enemyMovementUpdateInterval)
            {
                UpdateEnemyMovements(elapsedTime);

                elapsedTime = 0;
            }        
        }
    }


    private Transform targetTransform;
    private float targetMoveSpeed;
    private Vector3 pos;
    private int pointIndex;
    private Vector3 pointPos;
    private Quaternion pointRot;
    private float targetRotSpeed;
    private Vector3 movement;
    private float dist;


    private void UpdateEnemyMovements(float enemyMovementUpdateInterval)
    {
        //loop through all enemies and move them forward at all times.
        //then rotate them towards the next points, making them turn.
        for (int i = spawnedObj.Count - 1; i >= 0; i--)
        {
            targetMoveSpeed = spawnedObj[i].MoveSpeed;
            if (targetMoveSpeed == 0)
            {
                continue;
            }


            //get rotation between current point and next one, and calculate rotation speed.
            target = spawnedObj[i];
            targetTransform = target.transform;
            pointIndex = target.pointIndex;

            pointPos = points[pointIndex].position;
            pointRot = points[pointIndex].rotation;

            targetRotSpeed = targetMoveSpeed * 90 * rotMultiplier * enemyMovementUpdateInterval;


            //calculate movement and check distance to point.
            pos = targetTransform.position;
            movement = targetMoveSpeed * enemyMovementUpdateInterval * targetTransform.forward;
            dist = movement.x + movement.y + movement.z;

            if (Vector3.Distance(pos, pointPos) < dist)
            {
                targetTransform.position = pointPos;
            }
            else
            {
                targetTransform.position -= movement;
            }

            //rotate towards the next point 
            if (targetTransform.rotation.y != pointRot.y)
            {
                targetTransform.rotation = Quaternion.RotateTowards(targetTransform.rotation, pointRot, targetRotSpeed);
            }

            #region retrieve int2 direction and lock targets position in those 2 angles (x-1,x1,z-1,z1)
            pos = targetTransform.position;
            int2 dir = target.dir;
            if (dir.x == 1 && pos.x < pointPos.x)
            {
                pos.x = pointPos.x;
            }
            if (dir.x == -1 && pos.x > pointPos.x)
            {
                pos.x = pointPos.x;
            }
            if (dir.y == 1 && pos.z < pointPos.z)
            {
                pos.z = pointPos.z;
            }
            if (dir.y == -1 && pos.z > pointPos.z)
            {
                pos.z = pointPos.z;
            }
            targetTransform.position = pos;
            #endregion

            if (target.confusionTime == 0)
            {
                target.progression += enemyMovementUpdateInterval * targetMoveSpeed;
            }


            //update targetPoint to target and give new dir with UpdateTargetDir
            if (Vector3.Distance(pos, pointPos) < 0.035f)
            {
                target.pointIndex += 1;
                pointIndex += 1;
                if (pointIndex == points.Count)
                {
                    spawnedObj[i].gameObject.SetActive(false);
                    spawnedObj.RemoveAt(i);
                }
                else
                {
                    UpdateTargetDir(out target.dir, pos, points[pointIndex].position);
                }
            }
        }
    }

    public void UpdateTargetDir(out int2 dir, Vector3 targetPos, Vector3 pointPos)
    {
        dir = int2.zero;
        if (targetPos.x > pointPos.x)
        {
            dir.x = 1;
        }
        if (targetPos.x < pointPos.x)
        {
            dir.x = -1;
        }
        if (targetPos.z > pointPos.z)
        {
            dir.y = 1;
        }
        if (targetPos.z < pointPos.z)
        {
            dir.y = -1;
        }
    }
}
