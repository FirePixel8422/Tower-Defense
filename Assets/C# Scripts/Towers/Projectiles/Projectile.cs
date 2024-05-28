using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Projectile : MonoBehaviour
{
    [HideInInspector]
    public bool readyForSpawn;
    private TrailRenderer trail;
    private GameObject child;

    public int onHitEffectIndex = -1;

    public float trackTargetUpdateInterval;
    public int projectileId;
    public EnemyCore target;
    public ProjectileStats projStats;


    private void Start()
    {
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
    public void Init(EnemyCore _target, ProjectileStats _s)
    {
        if (trail != null)
        {
            trail.emitting = true;
            child.SetActive(true);
        }
        target = _target;
        projStats = _s;
        trackTargetUpdateInterval = TowerManager.Instance.projectileTrackTargetUpdateInterval;

        //move projectile towards target, so it cant "miss"
        StartCoroutine(TrackTargetLoop());
    }


    private IEnumerator TrackTargetLoop()
    {
        //loop and move bullet towards target. then check distance between this projectile and target and check if they collided (Vector3.Distance)

        WaitForSeconds wait = new WaitForSeconds(trackTargetUpdateInterval);
        while (true)
        {
            yield return wait;
            if (target.gameObject.activeInHierarchy == false)
            {
                target.incomingDamage -= projStats.damage;
                Debug.LogWarning("killed self");

                //tracked target died, destroy (disable for pool) bullet
                if (trail != null)
                {
                    child.SetActive(false);
                    trail.emitting = false;
                    yield return new WaitForSeconds(trail.time * 2);
                    child.SetActive(true);
                }
                gameObject.SetActive(false);
                readyForSpawn = true;
                yield break;
            }


            //move projectile and lookat target.
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, projStats.speed * trackTargetUpdateInterval);
            transform.LookAt(target.transform);
            if (Vector3.Distance(transform.position, target.transform.position) < projStats.projectileSize)
            {
                //call virtual void "HitTarget()"
                HitTarget();


                //onHit effect (explosions and stuff)
                if (onHitEffectIndex != -1)
                {
                    OnHitVFXPooling.Instance.GetPulledObj(onHitEffectIndex, transform.position, transform.rotation);
                }

                //destroy (disable for pool) bullet and let trail live for its duration
                if (trail != null)
                {
                    child.SetActive(false);
                    trail.emitting = false;
                    yield return new WaitForSeconds(trail.time);
                    child.SetActive(true);
                }
                gameObject.SetActive(false);
                readyForSpawn = true;
                yield break;
            }
        }
    }
    public virtual void HitTarget()
    {
        if (projStats.areaEffect != null)
        {
            //spawn collider to hit multiple bullets
            ApplySplashDamage();
        }

        target.ApplyDamage(projStats.damageType, projStats.damage, projStats.damageOverTimeType, projStats.damageOverTime, projStats.time);
    }
    public virtual void ApplySplashDamage()
    {
        AIO_AreaEffect AIO_Effect = Instantiate(projStats.areaEffect, target.transform.position, Quaternion.identity);
        AIO_Effect.Init(projStats);
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
}