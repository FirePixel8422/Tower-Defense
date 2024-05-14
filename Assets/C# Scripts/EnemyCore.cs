using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyCore : MonoBehaviour
{
    public float health;
    public float damage;
    public float incomingDamage;

    public float moveSpeed;

    public int pointIndex;
    public int2 dir;
    public float progression;

    public MagicType[] elements;

    public MagicType immunityBarrier;
    public Renderer barrierRenderer;


    public void Init(ImmunityBarrier _immunityBarrier)
    {
        if (_immunityBarrier != ImmunityBarrier.None)
        {
            if (_immunityBarrier == ImmunityBarrier.Smart)
            {
                immunityBarrier = TowerManager.Instance.HighestMagicType();
            }
            barrierRenderer.gameObject.SetActive(true);
        }
    }

    public void TryHit(float damage)
    {
        incomingDamage += damage;
    }
    public void Hit(float damage)
    {
        health -= damage;
        incomingDamage -= damage;
        if (health < 0)
        {
            WaveManager.Instance.spawnedObj.Remove(this);
            Destroy(gameObject);
        }
    }
}
