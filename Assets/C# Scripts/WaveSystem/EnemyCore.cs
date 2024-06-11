using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyCore : MonoBehaviour
{
    private Animator anim;

    public int enemyId;

    public float essenseOnDamage;
    public float essenceOnDeath;

    public float startStunEffectMultiplier;
    [SerializeField]
    private float stunEffectMultiplier;
    public float stunEffectDecrease;

    public float maxHealth;
    //[HideInInspector]
    public float health;
    public float damage;
    public float incomingDamage;


    public bool IsNotAboutToDie
    {
        get
        {
            return health > incomingDamage && dead == false;
        }
    }

    public float startMoveSpeed;


    [SerializeField]
    private float moveSpeed;
    public float MoveSpeed
    {
        get
        {
            if (confused)
            {
                return 0;
            }

            float speed = 100;
            foreach (float slowness in slownessEffectsList)
            {
                speed -= slowness;
            }
            return moveSpeed * speed * 0.01f;
        }
        private set
        {
            moveSpeed = value;
        }
    }
    public List<int> slownessEffectsList;


    public int pointIndex;
    public int2 dir;
    public float progression;

    public MagicType[] elements;

    public bool smartBarrier;
    public MagicType immunityBarrierType;

    public ImmunityBarrier immunityBarrier;
    [Header("% chance of enemy spawning with barrier")]
    public float barrierChance;
    [Header("How many seconds untill barrier health has disappeared")]
    public float barrierHealthDrainTime;
    public float barrierStartHealth;

    public bool confused;
    public float confusionTime;
    private bool dead;




    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        anim.speed = MoveSpeed;

        startMoveSpeed = moveSpeed;
        immunityBarrier = GetComponentInChildren<ImmunityBarrier>(true);
        gameObject.tag = "Enemy";
    }
    public void Init()
    {
        if (anim != null)
        {
            anim.speed = MoveSpeed;
        }
        if (immunityBarrier == null)
        {
            immunityBarrier = GetComponentInChildren<ImmunityBarrier>(true);
        }

        confused = false;
        confusionTime = 0;
        stunEffectMultiplier = startStunEffectMultiplier;

        slownessEffectsList.Clear();

        pointIndex = 0;
        health = maxHealth;
        incomingDamage = 0;
        progression = 0;
        dead = false;

        if (immunityBarrier != null && UnityEngine.Random.Range(0, 100f) <= barrierChance)
        {
            if (smartBarrier)
            {
                MagicType highestMagicType = TowerManager.Instance.HighestMagicType();
                immunityBarrierType = highestMagicType != MagicType.Neutral ? highestMagicType : immunityBarrierType;
            }
            if (immunityBarrierType != MagicType.Neutral)
            {
                immunityBarrier.gameObject.SetActive(true);
                immunityBarrier.Init(barrierStartHealth, barrierHealthDrainTime, (int)immunityBarrierType - 1);
            }
        }
    }

    public void TryHit(MagicType damageType, float damage, MagicType AIO_damageType, float AIO_damage)
    {
        if (immunityBarrierType == damageType && damageType != MagicType.Neutral)
        {
            damage = 0;
        }
        if (immunityBarrierType == AIO_damageType && AIO_damageType != MagicType.Neutral)
        {
            AIO_damage = 0;
        }
        incomingDamage += damage + AIO_damage;
    }
    public void ApplyDamage(MagicType damageType, float damage, MagicType damageOverTimeType, float damageOverTime, float time, float confusionTime, int slownessPercentage, float slownessTime, int maxSlowStacks)
    {
        if (dead || (gameObject.activeInHierarchy == false)) return;


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
        ResourceManager.Instance.AddRemoveEssence(percentDamage * essenseOnDamage, damageType);

        if (health <= 0)
        {
            WaveManager.Instance.spawnedObj.Remove(this);
            ResourceManager.Instance.AddRemoveEssence(essenceOnDeath, damageType);

            dead = true;
            gameObject.SetActive(false);
        }
        else
        {
            if (time != 0)
            {
                StartCoroutine(DamageOverTime(damageOverTimeType, damageOverTime, time));
            }
            if (confusionTime != 0)
            {
                StartCoroutine(ConfusionTimer(confusionTime));
            }
            if (slownessPercentage != 0)
            {
                StartCoroutine(SlownessTimer(slownessPercentage, slownessTime, maxSlowStacks));
            }
        }
    }

    private IEnumerator DamageOverTime(MagicType damageType, float damage, float time)
    {
        bool useImmunityBarrier = immunityBarrier != null && immunityBarrierType == damageType && damageType != MagicType.Neutral;


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
                if (useImmunityBarrier && immunityBarrier.barrierHealth > 0)
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
                ResourceManager.Instance.AddRemoveEssence(percentDamage * essenseOnDamage, damageType);

                if (health <= 0)
                {
                    WaveManager.Instance.spawnedObj.Remove(this);
                    ResourceManager.Instance.AddRemoveEssence(essenceOnDeath, damageType);
                    
                    dead = true;
                    gameObject.SetActive(false);
                    yield break;
                }
            }

            yield return null;
        }
        incomingDamage -= damage;
    }


    private float deltaTime;
    private IEnumerator ConfusionTimer(float _confusionTime)
    {
        if (confused == true)
        {
            confusionTime += _confusionTime * Mathf.Max(stunEffectMultiplier, 0);
            yield break;
        }
        else
        {
            confused = true;
            confusionTime += _confusionTime * Mathf.Max(stunEffectMultiplier, 0);
        }

        while (confusionTime > 0)
        {
            yield return null;
            deltaTime = Time.deltaTime;

            confusionTime -= deltaTime;
            print(deltaTime * stunEffectDecrease);
            stunEffectMultiplier -= deltaTime * stunEffectDecrease;
        }
        confusionTime = 0;
        confused = false;
        yield break;
    }
    private IEnumerator SlownessTimer(int slownessPercentage, float slownessTime, int maxStacks)
    {
        int amount = 0;
        foreach (float slownessEffect in slownessEffectsList)
        {
            if (slownessEffect == slownessPercentage)
            {
                amount += 1;
                if (amount == maxStacks)
                {
                    yield break;
                }
            }
        }



        slownessEffectsList.Add(slownessPercentage);

        anim.speed = MoveSpeed;

        yield return new WaitForSeconds(slownessTime);

        slownessEffectsList.Remove(slownessPercentage);
        anim.speed = MoveSpeed;
    }
}
