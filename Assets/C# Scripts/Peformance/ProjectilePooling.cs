using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePooling : MonoBehaviour
{
    public static ProjectilePooling Instance;
    private void Awake()
    {
        Instance = this;
    }


    public ProjectilePool[] pooledPrefabs;
    public bool dynamicRefillOnEmpty;

    public List<List<Projectile>> pooledList = new List<List<Projectile>>();


    private void Start()
    {
        pooledList = new List<List<Projectile>>();
        for (int i = 0; i < pooledPrefabs.Length; i++)
        {
            pooledList.Add(new List<Projectile>(1000));
            AddEnemyToPool(i, pooledPrefabs[i].amount);
        }
    }

    private void AddEnemyToPool(int index, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject obj = Instantiate(pooledPrefabs[index].projectilePrefab, Vector3.zero, Quaternion.identity, transform);
            pooledList[index].Add(obj.GetComponent<Projectile>());
            obj.SetActive(false);
        }
    }

    public GameObject GetPulledObj(int index, Vector3 pos, Quaternion rot)
    {
        foreach (Projectile obj in pooledList[index])
        {
            if (obj.readyForSpawn)
            {
                obj.gameObject.SetActive(true);
                obj.readyForSpawn = false;
                obj.transform.SetPositionAndRotation(pos, rot);
                return obj.gameObject;
            }
        }
        if (dynamicRefillOnEmpty == false)
        {
            return null;
        }
        GameObject spawnedObj = Instantiate(pooledPrefabs[index].projectilePrefab, pos, rot, transform);
        Projectile projectile = spawnedObj.GetComponent<Projectile>();
        pooledList[index].Add(projectile);
        projectile.readyForSpawn = false;
        pooledPrefabs[index].amount += 1;
        return spawnedObj;
    }


    [System.Serializable]
    public class ProjectilePool
    {
        public GameObject projectilePrefab;
        public int amount;
    }
}
