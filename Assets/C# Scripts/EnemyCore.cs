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
    public void ApplyDamage(float damage, float damageOverTime, float time)
    {
        health -= damage;
        incomingDamage -= damage;
        if (health <= 0)
        {
            WaveManager.Instance.spawnedObj.Remove(this);
            Destroy(gameObject);
        }
        else if(time != 0)
        {
            StartCoroutine(DamageOverTime(damageOverTime, time));
        }
    }

    private IEnumerator DamageOverTime(float damage, float time)
    {
        //temporary void update loop to apply damage over time, deal damage every .25 seconds until time is over.
        float timeLeft = time;
        float timer = 0f;

        damage = damage / time / 4;

        while (timeLeft > 0)
        {
            timer += Time.deltaTime;

            if (timer >= 0.25f)
            {
                timer -= 0.25f;
                timeLeft -= 0.25f;
                health -= damage;
                if (health <= 0)
                {
                    WaveManager.Instance.spawnedObj.Remove(this);
                    Destroy(gameObject);
                    yield break;
                }
            }

            yield return null;
        }
    }
}
