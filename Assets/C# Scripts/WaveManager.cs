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


    public Transform[] points;
    public GameObject prefab;

    public float speed;
    public float circularSpeed;

    public List<GameObject> spawnedObj;
    public List<int> cPoint;

    public float spawnDelay;
    public int amountOfSpawns;
    public float waveDelay;

    public float radius;



    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        GetComponent<SplineAnimate>().
        while (true)
        {
            yield return new WaitForSeconds(waveDelay);
            for (int i = 0; i < amountOfSpawns; i++)
            {
                spawnedObj.Add(Instantiate(prefab, points[0].position, Quaternion.identity));
                cPoint.Add(1);
                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }

    /*private void Update()
    {
    
        for (int i = 0; i < spawnedObj.Count; i++)
        {
            if (((float)cPoint[i] / 2) == Mathf.RoundToInt(cPoint[i] / 2))
            {

            }
            else
            {
                spawnedObj[i].transform.position = Vector3.MoveTowards(spawnedObj[i].transform.position, points[cPoint[i]].position, speed * Time.deltaTime);
                if (Vector3.Distance(spawnedObj[i].transform.position, points[cPoint[i]].position) < turnDist)
                {
                    cPoint[i] += 1;
                    if (cPoint[i] == points.Length)
                    {
                        Destroy(spawnedObj[i]);
                        spawnedObj.RemoveAt(i);
                        cPoint.RemoveAt(i);

                        i -= 1;
                    }
                }
            }
        }
    }*/
}
