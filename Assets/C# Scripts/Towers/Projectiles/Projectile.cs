using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

public class Projectile : MonoBehaviour
{
    [HideInInspector]
    public bool readyForSpawn;
    private TrailRenderer trail;
    private GameObject child;

    [HideInInspector]
    public int onHitEffectIndex = -1;

    public float trackTargetUpdateInterval;
    public int projectileId;
    public EnemyCore target;
    public ProjectileStats p;

    public int piercesDone;

    private Rigidbody rb;
    private Collider coll;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();

        TrailRenderer[] trails = GetComponentsInChildren<TrailRenderer>();
        if (trails.Length != 0)
        {
            float highestTime = -1;
            foreach (TrailRenderer _trail in trails)
            {
                if(_trail.time > highestTime)
                {
                    highestTime = _trail.time;
                    trail = _trail;
                }
            }
            trail.emitting = true;
        }
        child = transform.GetChild(0).gameObject;
    }
    public void Init(EnemyCore _target, ProjectileStats _s, int _onHitEffectIndex)
    {
        piercesDone = 0;
        onHitEffectIndex = _onHitEffectIndex;
        if (trail != null)
        {
            trail.emitting = true;
            child.SetActive(true);
        }
        target = _target;
        p = _s;
        trackTargetUpdateInterval = TowerManager.Instance.projectileTrackTargetUpdateInterval;

        //move projectile towards target, so it cant "miss"
        StartCoroutine(TrackTargetLoop());
    }


    private float elapsedTime;
    private float speedThisCycle;
    private IEnumerator TrackTargetLoop()
    {
        //loop and move bullet towards target. then check distance between this projectile and target and check if they collided (Vector3.Distance)
        while (true)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            if (elapsedTime < trackTargetUpdateInterval)
            {
                continue;
            }
            speedThisCycle = elapsedTime;
            elapsedTime = 0;


            if (target.gameObject.activeInHierarchy == false)
            {

                if (p.pierce != 0)
                {
                    StartCoroutine(StartPierceLogic());
                    yield break;
                }

                target.incomingDamage -= p.damage;
                Debug.LogWarning("killed self");

                //tracked target died, destroy (disable for pool) bullet
                if (trail != null)
                {
                    StartCoroutine(FadeTrail());
                }
                gameObject.SetActive(false);
                readyForSpawn = true;
                yield break;
            }


            //move projectile and lookat target.
            Vector3 targetPos = target.transform.position + new Vector3(0, target.transform.localScale.y / 3, 0);

            transform.position = Vector3.MoveTowards(transform.position, targetPos, p.speed * speedThisCycle);
            transform.LookAt(targetPos);

            if (Vector3.Distance(transform.position, targetPos) < p.projectileSize)
            {
                //call virtual void "HitTarget()"
                HitTarget();


                //onHit effect (explosions and stuff)
                if (onHitEffectIndex != -1)
                {
                    OnHitVFXPooling.Instance.GetPulledObj(onHitEffectIndex, transform.position, transform.rotation);
                }

                if (p.pierce != 0)
                {
                    StartCoroutine(StartPierceLogic());
                    yield break;
                }

                //destroy (disable for pool) bullet and let trail live for its duration
                if (trail != null)
                {
                    StartCoroutine(FadeTrail());
                }
                gameObject.SetActive(false);
                readyForSpawn = true;
                yield break;
            }
        }
    }
    public virtual void HitTarget()
    {
        if (p.areaEffect != null)
        {
            //spawn collider to hit multiple bullets
            ApplySplashDamage(target);
        }

        target.ApplyDamage(p.damageType, p.damage, p.damageOverTimeType, p.damageOverTime, p.time, p.confusionTime, p.slownessPercentage, p.slownessTime, p.maxSlowStacks);
    }
    public virtual void ApplySplashDamage(EnemyCore target)
    {
        AIO_AreaEffectPooling.Instance.GetPulledObj(p.areaEffect.areaEffectId, target.transform.position, Quaternion.identity, p);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (piercesDone == p.pierce)
        {
            if (trail != null)
            {
                StartCoroutine(FadeTrail());
            }
            else
            {
                gameObject.SetActive(false);
                readyForSpawn = true;
                coll.enabled = false;
            }
            rb.velocity = Vector3.zero;
        }

        if (other.gameObject.TryGetComponent(out EnemyCore target))
        {
            piercesDone += 1;


            if (onHitEffectIndex != -1)
            {
                OnHitVFXPooling.Instance.GetPulledObj(onHitEffectIndex, transform.position, transform.rotation);
            }

            if (p.areaEffect != null)
            {
                //spawn collider to hit multiple bullets
                ApplySplashDamage(target);
            }
            target.ApplyDamage(p.damageType, p.damage, p.damageOverTimeType, p.damageOverTime, p.time, p.confusionTime, p.slownessPercentage, p.slownessTime, p.maxSlowStacks);
        }
    }

    private IEnumerator StartPierceLogic()
    {
        transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
        coll.enabled = true;
        rb.velocity = transform.forward * p.speed;

        yield return new WaitForSeconds(20 / p.speed);

        if (trail != null)
        {
            yield return StartCoroutine(FadeTrail());
        }
        gameObject.SetActive(false);
        readyForSpawn = true;
        coll.enabled = false;
        rb.velocity = Vector3.zero;
    }
    private IEnumerator FadeTrail()
    {
        child.SetActive(false);
        trail.emitting = false;
        yield return new WaitForSeconds(trail.time);
        child.SetActive(true);
    }
}





[System.Serializable]
public struct ProjectileStats
{
    public float projectileSize;

    public float speed;
    public MagicType damageType;
    public float damage;
    public MagicType damageOverTimeType;
    public float damageOverTime;
    public float time;

    public int pierce;

    public float confusionTime;
    public int slownessPercentage;
    public float slownessTime;
    public int maxSlowStacks;

    [Header("Area effect on hit")]
    public AIO_AreaEffect areaEffect;
    public float AIO_duration;

    [Header("X cord for sphere radius")]
    public Vector3 AIO_areaEffectSize;
    public int AIO_maxSplashHits;

    public MagicType AIO_damageType;
    public float AIO_damage;
    public MagicType AIO_damageOverTimeType;
    public float AIO_damageOverTime;
    public float AIO_time;

    public float AIO_confusionTime;
    public int AIO_slownessPercentage;
    public float AIO_slownessTime;
    public int AIO_maxSlowStacks;
}