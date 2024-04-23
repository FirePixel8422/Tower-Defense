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
    public GameObject prefab;

    public float speed;

    public List<Transform> spawnedObj;
    public List<int> cPoint;

    public float spawnDelay;
    public int amountOfSpawns;
    public float waveDelay;


    private void Start()
    {
        spawnedObj = new List<Transform>();
        cPoint = new List<int>();
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(waveDelay);
            for (int i = 0; i < amountOfSpawns; i++)
            {
                Instantiate(prefab, points[0].position, Quaternion.identity);
                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }

    private void Update()
    {
        for (int i = 0;i < spawnedObj.Count ;i++)
        {
            Vector3.MoveTowards(spawnedObj[i].position, points[cPoint[i]], speed * Time.deltaTime)
        }
    }
}
