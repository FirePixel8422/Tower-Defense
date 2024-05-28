using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class OnHitVFXPooling : MonoBehaviour
{
    public static OnHitVFXPooling Instance;
    private void Awake()
    {
        Instance = this;
    }


    public VisualEffectPool[] pooledPrefabs;
    public bool dynamicRefillOnEmpty;

    public List<List<VisualEffect>> pooledList = new List<List<VisualEffect>>();


    private void Start()
    {
        pooledList = new List<List<VisualEffect>>();
        for (int i = 0; i < pooledPrefabs.Length; i++)
        {
            pooledList.Add(new List<VisualEffect>(1000));
            AddVisualEffectToPool(i, pooledPrefabs[i].amount);
        }
    }

    private void AddVisualEffectToPool(int index, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject obj = Instantiate(pooledPrefabs[index].visualEffectPrefab, Vector3.zero, Quaternion.identity, transform);
            pooledList[index].Add(obj.GetComponent<VisualEffect>());
        }
    }

    public GameObject GetPulledObj(int index, Vector3 pos, Quaternion rot)
    {
        foreach (VisualEffect obj in pooledList[index])
        {
            if (obj.HasAnySystemAwake() == false)
            {
                obj.Play();
                obj.transform.SetPositionAndRotation(pos, rot);
                return obj.gameObject;
            }
        }
        if (dynamicRefillOnEmpty == false)
        {
            return null;
        }
        GameObject spawnedObj = Instantiate(pooledPrefabs[index].visualEffectPrefab, pos, rot, transform);
        VisualEffect visualEffect = spawnedObj.GetComponent<VisualEffect>();

        pooledList[index].Add(visualEffect);
        pooledPrefabs[index].amount += 1;

        visualEffect.Play();
        
        return spawnedObj;
    }


    [System.Serializable]
    public class VisualEffectPool
    {
        public GameObject visualEffectPrefab;
        public int amount;
    }
}