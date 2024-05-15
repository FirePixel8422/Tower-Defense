using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyCore : MonoBehaviour
{
    public float essenseOnDamage;
    public float essenceOnDeath;

    private float maxHealth;
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

    private bool dead;


    public void Init(ImmunityBarrier _immunityBarrier)
    {
        gameObject.tag = "Enemy";

        maxHealth = health;
        if (_immunityBarrier != ImmunityBarrier.None)
        {
            if (_immunityBarrier == ImmunityBarrier.Smart)
            {
                immunityBarrier = TowerManager.Instance.HighestMagicType();
            }
            barrierRenderer.gameObject.SetActive(true);
        }
    }

    public void TryHit(MagicType damageType, float damage)
    {
        if (immunityBarrier == damageType && damageType != MagicType.Neutral)
        {
            damage = 0;
        }
        incomingDamage += damage;
    }
    public void ApplyDamage(MagicType damageType, float damage, MagicType damageOverTimeType, float damageOverTime, float time)
    {
        if (dead) return;

        if (immunityBarrier == damageType && damageType != MagicType.Neutral)
        {
            damage = 0;
        }

        health -= damage;

        //generate essence
        float percentDamage = (damage + (health < 0 ? health : 0)) / maxHealth;
        EssenceManager.Instance.GenerateEssenceFromEnemy(percentDamage * essenseOnDamage, damageType);


        incomingDamage -= damage;
        if (health <= 0)
        {
            WaveManager.Instance.spawnedObj.Remove(this);
            EssenceManager.Instance.GenerateEssenceFromEnemy(essenceOnDeath, damageType);
            
            dead = true;
            Destroy(gameObject);
        }
        else if (time != 0)
        {
            StartCoroutine(DamageOverTime(damageOverTimeType, damageOverTime, time));
        }
    }

    private IEnumerator DamageOverTime(MagicType damageType, float damage, float time)
    {
        if (immunityBarrier == damageType && damageType != MagicType.Neutral)
        {
            damage = 0;
        }

        //temporary void update loop to apply damage over time, deal damage every .25 seconds until time is over.
        float timeLeft = time;
        float timer = 0f;

        damage = damage / time / 4;

        incomingDamage += damage;
        while (timeLeft > 0)
        {
            if (dead) yield break;

            timer += Time.deltaTime;

            if (timer >= 0.25f)
            {
                timer -= 0.25f;
                timeLeft -= 0.25f;
                health -= damage;

                //generate essence
                float percentDamage = (damage + (health < 0 ? health : 0)) / maxHealth;
                EssenceManager.Instance.GenerateEssenceFromEnemy(percentDamage * essenseOnDamage, damageType);

                if (health <= 0)
                {
                    WaveManager.Instance.spawnedObj.Remove(this);
                    EssenceManager.Instance.GenerateEssenceFromEnemy(essenceOnDeath, damageType);
                    
                    dead = true;
                    Destroy(gameObject);
                    yield break;
                }
            }

            yield return null;
        }
    }
}
