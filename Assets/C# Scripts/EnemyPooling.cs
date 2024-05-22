using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPooling : MonoBehaviour
{
    public static EnemyPooling Instance;
    private void Awake()
    {
        Instance = this;
    }


    public EnemyPool[] pooledPrefabs;
    public bool dynamicRefillOnEmpty;

    public List<List<EnemyCore>> pooledList = new List<List<EnemyCore>>();


    private void Start()
    {
        pooledList = new List<List<EnemyCore>>();
        for (int i = 0; i < pooledPrefabs.Length; i++)
        {
            pooledList.Add(new List<EnemyCore>(1000));
            AddEnemyToPool(i, pooledPrefabs[i].amount);
        }
    }

    private void AddEnemyToPool(int index, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject obj = Instantiate(pooledPrefabs[index].enemyPrefab, Vector3.zero, Quaternion.identity);
            pooledList[index].Add(obj.GetComponent<EnemyCore>());
            obj.SetActive(false);
            obj.transform.SetParent(transform, true);
        }
    }

    public GameObject GetPulledObj(int index, Vector3 pos, Quaternion rot)
    {
        foreach (EnemyCore obj in pooledList[index])
        {
            if (obj.gameObject.activeInHierarchy == false)
            {
                obj.gameObject.SetActive(true);
                obj.transform.SetPositionAndRotation(pos, rot);
                return obj.gameObject;
            }
        }
        if(dynamicRefillOnEmpty == false)
        {
            return null;
        }
        GameObject spawnedObj = Instantiate(pooledPrefabs[index].enemyPrefab, pos, rot);
        EnemyCore enemy = spawnedObj.GetComponent<EnemyCore>();
        pooledList[index].Add(enemy);
        pooledPrefabs[index].amount += 1;
        return spawnedObj;
    }


    [System.Serializable]
    public class EnemyPool
    {
        public GameObject enemyPrefab;
        public int amount;
    }
}
