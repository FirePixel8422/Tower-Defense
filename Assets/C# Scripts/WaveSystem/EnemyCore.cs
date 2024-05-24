using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class EnemyCore : MonoBehaviour
{
    public int enemyId;

    public float essenseOnDamage;
    public float essenceOnDeath;

    public float maxHealth;
    //[HideInInspector]
    public float health;
    public float damage;
    public float incomingDamage;
    public float takenDamage;

    public bool IsNotAboutToDie
    {
        get
        {
            return health > incomingDamage && dead == false;
        }
    }

    public float moveSpeed;

    public int pointIndex;
    public int2 dir;
    public float progression;

    public MagicType[] elements;

    public bool smartBarrier;
    public MagicType immunityBarrierType;

    private ImmunityBarrier immunityBarrier;
    [Header("% chance of enemy spawning with barrier")]
    public float barrierChance;
    [Header("How many seconds untill barrier health has disappeared")]
    public float barrierHealthDrainTime;
    public float barrierStartHealth;

    private bool dead;




    private void Awake()
    {
        immunityBarrier = GetComponentInChildren<ImmunityBarrier>(true);
        gameObject.tag = "Enemy";
    }
    public void Init()
    {
        pointIndex = 0;
        health = maxHealth;
        incomingDamage = 0;
        takenDamage = 0;
        progression = 0;
        dead = false;

        if (immunityBarrier != null && (barrierChance == 0 || UnityEngine.Random.Range(0, 100f) < barrierChance))
        {
            if (smartBarrier)
            {
                MagicType highestMagicType = TowerManager.Instance.HighestMagicType();
                immunityBarrierType = highestMagicType != MagicType.Neutral ? highestMagicType : immunityBarrierType;
            }
            if (immunityBarrierType != MagicType.Neutral)
            {
                immunityBarrier.gameObject.SetActive(true);
                immunityBarrier.Init(barrierStartHealth, barrierHealthDrainTime);
            }
        }
    }

    public void TryHit(MagicType damageType, float damage, bool doSplashDamage, MagicType AIO_damageType, float AIO_damage)
    {
        if (immunityBarrierType == damageType && damageType != MagicType.Neutral)
        {
            damage = 0;
        }
        if (immunityBarrierType == AIO_damageType && AIO_damageType != MagicType.Neutral)
        {
            AIO_damage = 0;
        }
        incomingDamage += damage + (doSplashDamage ? AIO_damage : 0);
    }
    public void ApplyDamage(MagicType damageType, float damage, MagicType damageOverTimeType, float damageOverTime, float time)
    {
        takenDamage += damage;
        if (dead) return;

        
        if (immunityBarrier != null && immunityBarrier.barrierHealth > 0)
        {
            if (immunityBarrierType == damageType && damageType != MagicType.Neutral)
            {
                damage = 0;
            }
            incomingDamage -= damage;
            //give direction unless direction "projectilePos" == Vcetor3.zero being no direction.
            immunityBarrier.TakeDamage(damage);
            return;
        }

        health -= damage;
        incomingDamage -= damage;

        //generate essence
        float percentDamage = (damage + (health < 0 ? health : 0)) / maxHealth;
        EssenceManager.Instance.GenerateEssenceFromEnemy(percentDamage * essenseOnDamage, damageType);

        if (health <= 0)
        {
            WaveManager.Instance.spawnedObj.Remove(this);
            EssenceManager.Instance.GenerateEssenceFromEnemy(essenceOnDeath, damageType);
            
            dead = true;
            gameObject.SetActive(false);
        }
        else if (time != 0)
        {
            StartCoroutine(DamageOverTime(damageOverTimeType, damageOverTime, time));
        }
    }

    private IEnumerator DamageOverTime(MagicType damageType, float damage, float time)
    {
        if (immunityBarrier != null && immunityBarrier.barrierHealth > 0 && immunityBarrierType == damageType && damageType != MagicType.Neutral)
        {
            yield break;
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
                if (immunityBarrier != null && immunityBarrier.barrierHealth > 0)
                {
                    //give direction unless direction "projectilePos" == Vcetor3.zero being no direction.
                    immunityBarrier.TakeDamage(damage);
                }
                else
                {
                    health -= damage;
                }

                //generate essence
                float percentDamage = (damage + (health < 0 ? health : 0)) / maxHealth;
                EssenceManager.Instance.GenerateEssenceFromEnemy(percentDamage * essenseOnDamage, damageType);

                if (health <= 0)
                {
                    WaveManager.Instance.spawnedObj.Remove(this);
                    EssenceManager.Instance.GenerateEssenceFromEnemy(essenceOnDeath, damageType);
                    
                    dead = true;
                    gameObject.SetActive(false);
                    yield break;
                }
            }

            yield return null;
        }
        incomingDamage -= damage;
    }
}
