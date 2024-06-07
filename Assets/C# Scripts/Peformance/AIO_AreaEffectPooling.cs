using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AIO_AreaEffectPooling : MonoBehaviour
{
    public static AIO_AreaEffectPooling Instance;
    private void Awake()
    {
        Instance = this;
    }


    public AIO_AreaEffectPool[] pooledPrefabs;
    public bool dynamicRefillOnEmpty;

    public List<List<AIO_AreaEffect>> pooledList = new List<List<AIO_AreaEffect>>();


    private void Start()
    {
        pooledList = new List<List<AIO_AreaEffect>>();
        for (int i = 0; i < pooledPrefabs.Length; i++)
        {
            pooledList.Add(new List<AIO_AreaEffect>(1000));
            AddAreaEffectToPool(i, pooledPrefabs[i].amount);
        }
    }

    private void AddAreaEffectToPool(int index, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject obj = Instantiate(pooledPrefabs[index].areaEffectPrefab, Vector3.zero, Quaternion.identity, transform);
            pooledList[index].Add(obj.GetComponent<AIO_AreaEffect>());
        }
    }

    public GameObject GetPulledObj(int index, Vector3 pos, Quaternion rot, ProjectileStats stats)
    {
        foreach (AIO_AreaEffect obj in pooledList[index])
        {
            if (obj.gameObject.activeInHierarchy == false)
            {
                obj.gameObject.SetActive(true);
                obj.transform.SetPositionAndRotation(pos, rot);
                obj.Init(stats);
                return obj.gameObject;
            }
        }
        if (dynamicRefillOnEmpty == false)
        {
            return null;
        }
        GameObject spawnedObj = Instantiate(pooledPrefabs[index].areaEffectPrefab, pos, rot, transform);

        AIO_AreaEffect areaEffect = spawnedObj.GetComponent<AIO_AreaEffect>();
        areaEffect.Init(stats);

        pooledList[index].Add(areaEffect);
        pooledPrefabs[index].amount += 1;

        return spawnedObj;
    }


    [System.Serializable]
    public class AIO_AreaEffectPool
    {
        public GameObject areaEffectPrefab;
        public int amount;
    }
}